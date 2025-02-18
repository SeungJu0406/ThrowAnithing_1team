using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashBoom", menuName = "AdditionalEffect/Player/DashBoom")]
public class DashBoom : PlayerAdditional
{
    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
    }
    [SerializeField] EffectStrcut _effect;

    [Header("폭발 범위")]
    [SerializeField] float _range;
    [Header("기절 시간")]
    [SerializeField] float _stunTime;
    public override void Trigger()
    {
        if(Player.CurState == PlayerController.State.Dash)
        {
            Attack();
        }
    }

    private void Attack()
    {
        GameObject effect= ObjectPool.Get(_effect.EffectPrefab, transform.position, Quaternion.identity, 1f);
        effect.transform.localScale *= _range;

        int hitCount = Physics.OverlapSphereNonAlloc(Battle.HitPoint.position, _range, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            // TODO : 몬스터 기절 기능 
            Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stun, _stunTime);
        }
    }


    //public override void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, _range);
    //}
}
