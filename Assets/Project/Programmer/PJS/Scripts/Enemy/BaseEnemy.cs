using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State
{
    public string Name;
    [Range(50, 10000)] public int MaxHp;  // 체력
    [Range(0, 100)] public int Atk;       // 공격력
    [Range(0, 100)] public float Def;    // 방어력
    [Range(0, 100)] public float Speed;    // 이동 속도
    [Range(0, 100)] public float AtkDelay;   // 공격 속도
    [Range(0, 100)] public float AttackDis;  // 공격 사거리
    [Range(0, 100)] public float TraceDis;   // 인식 사거리
}

[RequireComponent(typeof(BattleSystem))]
public class BaseEnemy : MonoBehaviour, IHit, IDebuff
{
    public enum MonsterType { Nomal, Mutant, Elite, SubBoss, Boss }
    public MonsterType curMonsterType;
    [SerializeField] protected BehaviorTree tree;

    [Header("몬스터 기본 스테이터스")]
    [SerializeField] protected State state;
    [Header("아이템 드랍 확률(100 단위)"), Range(0, 100)]
    [SerializeField] float reward;
    [Header("현재 체력")]
    [SerializeField] int curHp;
    [Header("라운드별 적용 체력"), Range(0, 100)]
    [SerializeField] int roundHp;
    [Header("이동 및 사망 파티클")]
    [SerializeField] protected ParticleSystem stepMoveParticle;
    [SerializeField] protected ParticleSystem dieParticle;
    [Header("효과음")]
    [Tooltip("이동 관련")]
    [SerializeField] protected List<AudioClip> moveClips;
    [SerializeField] protected List<AudioClip> voiceClips;
    [Tooltip("사망 관련")]
    [SerializeField] protected List<AudioClip> deathCilps;
    [Tooltip("피격 관련")]
    [SerializeField] protected List<AudioClip> hitCilps;

    [HideInInspector] float jumpPower;  // 점프력

    [HideInInspector] public int resultDamage;  // 최종적으로 피해 입는 데미지
    [HideInInspector] public Collider[] overLapCollider = new Collider[100];
    [HideInInspector] public BattleSystem Battle;

    protected int randomMoveClip;
    protected int stepCount = 0;

    public int Damage { get { return state.Atk; } }
    public int MaxHp { get { return state.MaxHp; } set { state.MaxHp = value; } }
    public int CurHp { get { return curHp; } set { curHp = value; } }
    public float MoveSpeed { get { return state.Speed; } set { state.Speed = value; } }
    public float JumpPower { get { return jumpPower; } set { jumpPower = value; } }
    public float AttackSpeed { get { return state.AtkDelay; } set { state.AtkDelay = value; } }

    protected SharedGameObject playerObj;

    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        tree = GetComponent<BehaviorTree>();
        Battle = GetComponent<BattleSystem>();
    }

    private void Start()
    {
        BaseInit();
    }

    public State GetState()
    {
        return state;
    }

    public BehaviorTree GetBT()
    {
        return tree;
    }

    public List<AudioClip> GetDaethClips()
    {
        return deathCilps;
    }

    public AudioClip ChoiceAudioClip(List<AudioClip> audioClips)
    {
        return audioClips[Random.Range(0, audioClips.Count)];
    }

    protected void BaseInit()
    {
        SettingVariable();
        int getRoundHp = Mathf.RoundToInt(state.MaxHp * (roundHp / 100f));
        switch (Round.instance.curRound)
        {
            case 1:
                curHp = state.MaxHp;
                break;
            case 2:
                curHp = state.MaxHp + getRoundHp;
                break;
            default:
                curHp = state.MaxHp;
                break;
        }
        randomMoveClip = Random.Range(0, moveClips.Count);
    }

    private void SettingVariable()
    {
        tree.SetVariable("PlayerObj", playerObj);
        tree.SetVariable("PlayerTrans", (SharedTransform)playerObj.Value.transform);
        tree.SetVariable("Speed", (SharedFloat)MoveSpeed);
        tree.SetVariable("AtkDelay", (SharedFloat)state.AtkDelay);
        tree.SetVariable("TraceDis", (SharedFloat)state.TraceDis);
        tree.SetVariable("AttackDis", (SharedFloat)state.AttackDis);
        tree.SetVariable("Reward", (SharedFloat)reward);
    }

    // 이동 애니메이션 이벤트
    public void BeginStepMove()
    {
        if (stepCount >= 10)
        {
            SoundManager.PlaySFX(ChoiceAudioClip(voiceClips));
            stepCount = 0;
        }

        SoundManager.PlaySFX(moveClips[randomMoveClip]);
        stepMoveParticle.Play();
        stepCount++;
    }

    // 사망 애니메이션 이벤트
    public void DeadMotion()
    {
        dieParticle.Play();
    }

    public int TakeDamage(int damage, bool isIgnoreDef)
    {
        resultDamage = isIgnoreDef == true ? damage : damage - (int)state.Def;
        tree.SetVariableValue("TakeDamage", true);

        if (resultDamage <= 0)
            resultDamage = 0;
        if (resultDamage < curHp)
            SoundManager.PlaySFX(ChoiceAudioClip(hitCilps));
        
        curHp -= resultDamage;

        return resultDamage;
    }

    public void TakeCrowdControl(CrowdControlType type, float time)
    {
        if (type == CrowdControlType.Stun)
        {
            tree.SetVariableValue("Stun", type == CrowdControlType.Stun);
            tree.SetVariableValue("StunTime", time);
        }
        tree.SetVariableValue("Stiff", type == CrowdControlType.Stiff);
    }

    /// <summary>
    /// 반경 내 폭발 데미지 부여
    /// </summary>
    public void TakeChargeBoom(float range, int damage)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, range, overLapCollider);
        for (int i = 0; i < hitCount; i++)
        {
            IBattle hit = overLapCollider[i].GetComponent<IBattle>();
            if (hit != null)
            {
                if (overLapCollider[i].gameObject.name.CompareTo("Boss") == 0)
                    continue;

                hit.TakeDamage(damage, true);
            }
        }
    }

    /// <summary>
    /// 패턴 쿨타임 관련 코루틴
    /// </summary>
    /// <param name="atkAble">해당 스킬의 bool 타입</param>
    /// <param name="coolTime">쿨타임 시간</param>
    public IEnumerator CoolTimeRoutine(SharedBool atkAble, float coolTime)
    {
        atkAble.SetValue(false);
        yield return coolTime.GetDelay();
        atkAble.SetValue(true);
    }

    private void OnDrawGizmosSelected()
    {
        // 거리 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, state.TraceDis);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, state.AttackDis);
    }
}
