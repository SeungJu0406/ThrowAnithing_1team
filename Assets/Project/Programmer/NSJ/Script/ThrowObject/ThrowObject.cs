using EPOOutline;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outlinable))]
[RequireComponent(typeof(ThrowObjectOutline))]
public class ThrowObject : MonoBehaviour
{
    [System.Serializable]
    protected partial struct EffectStruct
    {
        public GameObject Hit;
        public GameObject BoomHit;
    }
    // 이펙트
    [SerializeField] protected EffectStruct Effect;


    public ThrowObjectData Data;
    public bool CanAttack;
    [SerializeField] public List<ThrowAdditional> ThrowAdditionals = new List<ThrowAdditional>();

    // 오브젝트 자체 데미지
    public int ObjectDamage;
    // 플레이어의 추가 데미지
    [HideInInspector] public int PlayerDamage;
    public int Damage => ObjectDamage + PlayerDamage;
    // 데미지 배수
    public float DamageMultyPlier;
    [Space(10)]
    [HideInInspector] public bool IsBoom;
    // 공격 범위(폭발식)
    [HideInInspector] public float Radius;
    // CC기 종류
    [HideInInspector] public CrowdControlType CCType;
    // 넉백거리
    [HideInInspector] public float KnockBackDistance;
    // 스테미나 회복량
    [HideInInspector] public float SpecialRecovery;

    [Tooltip("클론형태의 투척물 인지?")]
    public bool IsClone;
    [Tooltip("본인에게서 파생된 모든 투척물(체인)")]
    public List<ThrowObject> ChainList;
    // 체인된것중에 하나라도 맞았는지?
    public bool IsChainHit;

    public List<GameObject> IgnoreTargets = new List<GameObject>();
    protected Collider[] _overlapCollider = new Collider[20];
    protected PlayerController Player;
    protected BattleSystem Battle => Player.Battle;



    [HideInInspector] public Rigidbody Rb;
    protected Collider _collider;

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        Player = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<PlayerController>();
        gameObject.layer = Layer.ThrowObject;

    }

    protected virtual void Start()
    {

    }
    protected virtual void OnEnable()
    {
        Rb.velocity = Vector3.zero;
        Rb.angularVelocity = Vector3.zero;
        IgnoreTargets.Clear();
        ChainList.Clear();
        CanAttack = true;
        _collider.isTrigger = true;
    }
    protected virtual void OnDisable()
    {
        ClearThrowAddtional();
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (IsClone == true)
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == Layer.Player)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.AddThrowObject(this);
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger == true)
            return;

        string tag = other.gameObject.tag;
        int layer = other.gameObject.layer;
        if (layer == Layer.Monster)
        {
            // 무시해야하는 타겟대상은 공격하지 않음
            if (IgnoreTargets.Contains(other.gameObject) == true)
                return;

            TriggerThrowAddtional();
            HitTarget(other);
            Player.ThrowObjectResultCallback(this, true);
            SetChainHit(true);

            RemoveChainList(this);
        }
        else if (tag != Tag.Player)
        {
            CanAttack = false;
            _collider.isTrigger = false;
            Player.ThrowObjectResultCallback(this, false);

            RemoveChainList(this);
        }
    }

    private void Update()
    {
        UpdateThrowAdditional();

        if (CanAttack == false)
        {
            if (Player.Model.CurThrowables >= Player.Model.MaxThrowables)
            {
                gameObject.layer = Layer.CantPickTrash;
            }
            else
            {
                gameObject.layer = Layer.CanPickTrash;
            }
        }
    }
    private void FixedUpdate()
    {
        FixedUpdateThrowAdditional();
    }
    #region Init
    public void Init(PlayerController player, CrowdControlType CCType, bool isBoom,List<ThrowAdditional> throwAdditionals)
    {
        Player = player;
        Radius = player.Model.BoomRadius;
        IsBoom = isBoom;

        this.CCType = CCType;
        // 적중시 회복 마나량
        SpecialRecovery = player.Model.RegainMana[player.Model.ChargeStep];
        SpecialRecovery += SpecialRecovery * player.Model.RegainAdditiveMana / 100;

        AddThrowAdditional(throwAdditionals, player);
    }
    public void Init(PlayerController player, CrowdControlType CCType,  bool isBoom,int addionalDamage,List<ThrowAdditional> throwAdditionals)
    {
        Player = player;
        PlayerDamage = addionalDamage;
        Radius = player.Model.BoomRadius;
        IsBoom = isBoom;

        this.CCType = CCType;
        // 적중시 회복 마나량
        SpecialRecovery = player.Model.RegainMana[player.Model.ChargeStep];
        SpecialRecovery += SpecialRecovery * player.Model.RegainAdditiveMana / 100;

        AddThrowAdditional(throwAdditionals, player);
    }
    #endregion
    public void Shoot(float throwPower)
    {
        Rb.AddForce(transform.forward * throwPower, ForceMode.Impulse);
    }
    /// <summary>
    /// 타겟 적중
    /// </summary>
    protected virtual void HitTarget(Collider other)
    {
        if (CanAttack == false)
            return;

        if(IsBoom== true)
        {
            PowerHitTarget(other);
        }
        else
        {
            BasicHitTarget(other);
        }
    }


    private void PowerHitTarget(Collider other)
    {

        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, Radius, Player.OverLapColliders, 1 << Layer.Monster);

        for (int i = 0; i < hitCount; i++)
        {
            int finalDamage = Player.GetFinalDamage(Damage, DamageMultyPlier, out bool isCritical);
            // 디버프 주기
            int hitDamage = Player.Battle.TargetAttackWithDebuff(Player.OverLapColliders[i], isCritical, finalDamage, false);

            Player.Battle.TargetCrowdControl(Player.OverLapColliders[i], CCType);

            if (KnockBackDistance > 0)
                Player.DoKnockBack(Player.OverLapColliders[i].transform, transform.forward, KnockBackDistance);

            // 사운드
            SoundManager.PlaySFX(isCritical == true ? Player.Sound.Hit.Critical : Player.Sound.Hit.Hit);
        }

        // 플레이어 특수공격 자원 획득
        Player.Model.CurMana += SpecialRecovery;
        // 이펙트 

        ObjectPool.Get(Effect.BoomHit, transform.position, transform.rotation, 1.5f);

        Destroy(gameObject);
    }

    private void BasicHitTarget(Collider other)
    {
        int finalDamage = Player.GetFinalDamage(Damage, DamageMultyPlier, out bool isCritical);
        // 디버프 주기
        int hitDamage = Player.Battle.TargetAttackWithDebuff(other, isCritical, finalDamage, false);

        Player.Battle.TargetCrowdControl(other, CCType);

        if (KnockBackDistance > 0)
            Player.DoKnockBack(other.transform, transform.forward, KnockBackDistance);

        // 플레이어 특수공격 자원 획득
        Player.Model.CurMana += SpecialRecovery;
        // 이펙트 
        ObjectPool.Get(Effect.Hit, transform.position, transform.rotation, 1.5f);

        // 사운드
        SoundManager.PlaySFX(isCritical == true ? Player.Sound.Hit.Critical : Player.Sound.Hit.Hit);

        Destroy(gameObject);
    }



    /// <summary>
    /// 공격 추가 효과 발동
    /// </summary>
    public virtual void EnterThrowAdditional()
    {
        foreach (ThrowAdditional throwAdditional in ThrowAdditionals)
        {
            throwAdditional.Enter();
        }
    }
    public virtual void ExitThrowAdditional()
    {
        foreach (ThrowAdditional throwAdditional in ThrowAdditionals)
        {
            throwAdditional.Exit();
        }
    }

    public virtual void UpdateThrowAdditional()
    {
        if (CanAttack == false)
            return;

        foreach (ThrowAdditional throwAdditional in ThrowAdditionals)
        {
            throwAdditional.Update();
        }
    }

    public virtual void FixedUpdateThrowAdditional()
    {
        if (CanAttack == false)
            return;

        foreach (ThrowAdditional throwAdditional in ThrowAdditionals)
        {
            throwAdditional.FixedUpdate();
        }
    }
    public virtual void TriggerThrowAddtional()
    {
        if (CanAttack == false)
            return;

        foreach (ThrowAdditional throwAdditional in ThrowAdditionals)
        {
            throwAdditional.Trigger();
        }
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }


    protected void AddThrowAdditional(List<ThrowAdditional> throwAdditionals, PlayerController player)
    {
        foreach (ThrowAdditional throwAdditional in throwAdditionals)
        {
            int index = ThrowAdditionals.FindIndex(origin => origin.Origin.Equals(throwAdditional.Origin));
            if (index >= ThrowAdditionals.Count)
                return;

            if (index == -1)
            {
                ThrowAdditional instance = Instantiate(throwAdditional);
                instance.Origin = throwAdditional.Origin;
                instance.Init(player, throwAdditional, this);
                ThrowAdditionals.Add(instance);
            }
        }
        EnterThrowAdditional();
    }
    private void RemoveThrowAddtional(ThrowAdditional throwAdditional)
    {
        throwAdditional.Exit();
        ThrowAdditionals.Remove(throwAdditional);
        Destroy(throwAdditional);
    }
    private void ClearThrowAddtional()
    {
        for (int i = ThrowAdditionals.Count - 1; i >= 0; i--)
        {
            RemoveThrowAddtional(ThrowAdditionals[i]);
        }
    }

    public void AddChainList(ThrowObject other)
    {
        if (ChainList.Count == 0)
        {
            ChainList.Add(this);
        }
        // 체인 리스트에 투척물 추가
        ChainList.Add(other);
        other.ChainList = ChainList;
    }
    private void SetChainHit(bool hitSuccess)
    {
        foreach (ThrowObject chainObject in ChainList)
        {
            chainObject.IsChainHit = hitSuccess;
        }
    }
    private void RemoveChainList(ThrowObject throwObject)
    {
        if (ChainList.Contains(throwObject))
            ChainList.Remove(throwObject);
    }
}

[System.Serializable]
public class ThrowObjectData
{
    public int ID;
    public string Name;
}
