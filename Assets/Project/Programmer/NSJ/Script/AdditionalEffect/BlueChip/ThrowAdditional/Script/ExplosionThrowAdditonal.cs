using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionThrow", menuName = "AdditionalEffect/PrevThrow/ExplosionThrow")]
public class ExplosionThrowAdditonal : ThrowAdditional
{
    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
        public float EffectDuration;
    }
    [SerializeField] EffectStrcut _effect;
   

    [Header("폭발 범위")]
    [SerializeField] private float _explosionRange;
    [Header("폭발 데미지(%)")]
    [SerializeField] private float _explosionDamage;

    private int damage => (int)(_throwObject.Damage * _explosionDamage/ 100);

    public override void Exit()
    {
        if (_throwObject.CanAttack == false)
            return;

        Explosion();
    }

    private void Explosion()
    {
        // 가까운 적(이미 맞은 적) 탐지
        TargetInfo nearTarget = FindNearTarget();

        int hitCount = Physics.OverlapSphereNonAlloc(_throwObject.transform.position, _explosionRange, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++) 
        {
            Transform target = Player.OverLapColliders[i].transform;
            // 맞은 적은 또 안때림
            if (target == nearTarget.transform)
                continue;
            Battle.TargetAttack(target, damage, false);
        }

        ShowEffect();
    }

    private void ShowEffect()
    {
        CoroutineHandler.StartRoutine(ShowEffectRoutine());
    }

    IEnumerator ShowEffectRoutine()
    {
         _effect.Effect = ObjectPool.Get(_effect.EffectPrefab ,_throwObject.transform.position, _throwObject.transform.rotation);
        yield return _effect.EffectDuration.GetDelay();
        ObjectPool.Return( _effect.Effect);
    }
}
