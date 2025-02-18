using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Overload", menuName = "AdditionalEffect/Player/Overload")]
public class OverloadAdditional : PlayerAdditional
{
    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
    }
    [SerializeField] EffectStrcut _effect;
    [Header("데미지")]
    [SerializeField] private int _damage;
    [Header("범위")]
    [SerializeField] private float _range;
    [Header("데미지 간격")]
    [SerializeField] private float _damageInterval = 0.3f;

    Coroutine _attackRoutine;

    public override void Enter()
    {
        if (_attackRoutine == null)
            _attackRoutine = CoroutineHandler.StartRoutine(AttackRoutine());

        CreateEffect();
    }

    public override void Exit()
    {
        if( _attackRoutine != null)
        {
            CoroutineHandler.StopRoutine(_attackRoutine);
            _attackRoutine = null;
        }

        Destroy(_effect.Effect);
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _range, Player.OverLapColliders, 1 << Layer.Monster);
            for(int i = 0;  i < hitCount; i++)
            {
                int finalDamage = Player.GetDamage(_damage);
                Player.Battle.TargetAttack(Player.OverLapColliders[i], finalDamage, true);
            }
            yield return _damageInterval.GetDelay();
        }
    }

    private void CreateEffect()
    {
        _effect.Effect = Instantiate(_effect.EffectPrefab, transform);

        Vector3 effectScale = _effect.Effect.transform.localScale;
        _effect.Effect.transform.localScale = new Vector3(_range * 2, effectScale.y, _range * 2);
    }
    
}
