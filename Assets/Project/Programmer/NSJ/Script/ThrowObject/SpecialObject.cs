using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpecialObject : ThrowObject
{
    struct ExplosionStruct
    {
        public float Damage;
        public float MiddleDamage;
        public float Range;
        public float MiddleRange;
    }
    // 폭발 데미지, 범위 필드
    private ExplosionStruct _explosion;
    [System.Serializable]
    new struct EffectStruct
    {
        public GameObject EffectPrefab;
        public GameObject ThrowTailEffectPrefab;
        [HideInInspector]public GameObject ThrowTailEffect;
    }
    // 이펙트 필드
    [SerializeField] private EffectStruct _effect;

    private List<Transform> MiddleHittargets = new List<Transform>();
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        // 이펙트 생성
        _effect.ThrowTailEffect = ObjectPool.Get(_effect.ThrowTailEffectPrefab,transform);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // 땅에 닿아도 폭발
        ProcessExplosion();
        ObjectPool.Return(gameObject);
    }

    protected override void OnDisable()
    {


        base.OnDisable();
    }
    /// <summary>
    /// 폭발 필드 설정
    /// </summary>
    public void InitSpecial(float damage, float middleDamage, float range, float middleRange)
    {
        _explosion.Damage = damage;
        _explosion.MiddleDamage = middleDamage;
        _explosion.Range = range;
        _explosion.MiddleRange = middleRange;   
    }


    /// <summary>
    /// 타겟 적중
    /// </summary>
    protected override void HitTarget(Collider other)
    {
        if (CanAttack == false)
            return;
        // 꼬리 이펙트 제거

        ProcessExplosion();
        _effect.ThrowTailEffect.transform.SetParent(null);
        ObjectPool.Return(_effect.ThrowTailEffect, 0.5f);
        ObjectPool.Return(gameObject);
    }

    /// <summary>
    /// 폭발
    /// </summary>
    private void ProcessExplosion() 
    {
        // 중앙 맞은 몬스터 걸러내기
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _explosion.MiddleRange, Player.OverLapColliders, 1 << Layer.Monster);
        for(int i = 0; i < hitCount; i++)
        {
            MiddleHittargets.Add(Player.OverLapColliders[i].transform);
        }


        hitCount = Physics.OverlapSphereNonAlloc(transform.position, _explosion.Range, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++) 
        {
            int finalDamage = 0;
            bool isCritical = false;
            // 중앙에 맞았을 경우
            if (MiddleHittargets.Contains(Player.OverLapColliders[i].transform))
            {
                finalDamage = Player.GetFinalDamage((int)_explosion.MiddleDamage, out isCritical);
            }
            else
            {
                finalDamage = Player.GetFinalDamage((int)_explosion.Damage, out isCritical);
            }
         
            Battle.TargetAttack(Player.OverLapColliders[i], isCritical, finalDamage);
            Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stiff);
        }

        // 이펙트
        ObjectPool.Get(_effect.EffectPrefab, transform.position, transform.rotation, 1f);

        // 사운드
        SoundManager.PlaySFX(Player.Sound.Balance.Special2Hit);
    }

    #region 특수효과
    // 특수공격은 특수효과 터지면 안됨
    public override void EnterThrowAdditional() { }   
    public override void ExitThrowAdditional() { }       
    public override void UpdateThrowAdditional() { }           
    public override void FixedUpdateThrowAdditional() { }
    public override void TriggerThrowAddtional() { }
    #endregion
}
