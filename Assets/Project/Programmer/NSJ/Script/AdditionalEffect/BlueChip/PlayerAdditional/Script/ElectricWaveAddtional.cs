using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[CreateAssetMenu(fileName = "ElectricWave", menuName = "AdditionalEffect/Player/ElectricWave")]
public class ElectricWaveAddtional : PlayerAdditional
{

    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
        public float EffectDuration;
    }
    [SerializeField] EffectStrcut _effect;
    [SerializeField] private ElectricShockAdditonal _electricShockOrigin;
    private ElectricShockAdditonal _electricShock;
    [Header("발동 시간 간격")]
    [SerializeField] private float _intervalTime;
    [Header("데미지")]
    [SerializeField] private int _damage;
    [Header("범위")]
    [SerializeField] private float _range;


    Coroutine _attackRoutine;

    public override void Enter()
    {
        _electricShock = Instantiate(_electricShockOrigin);
        _electricShock.Probability = 100;

        if (_attackRoutine == null)
            _attackRoutine = CoroutineHandler.StartRoutine(AttackRoutine());

       
    }

    public override void Exit()
    {
        Destroy(_electricShock);

        if (_attackRoutine != null)
        {
            CoroutineHandler.StopRoutine(_attackRoutine);
            _attackRoutine = null;
        }     
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return _intervalTime.GetDelay();
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _range, Player.OverLapColliders, 1 << Layer.Monster);
            for (int i = 0; i < hitCount; i++)
            {
                int finalDamage = Player.GetDamage(_damage);
                Player.Battle.TargetAttack(Player.OverLapColliders[i], finalDamage,false);
                Player.Battle.TargetDebuff(Player.OverLapColliders[i], _electricShock);
            }
            CreateEffect();
        }
    }

    private void CreateEffect()
    {
        _effect.Effect = ObjectPool.Get(_effect.EffectPrefab, transform.position, Quaternion.identity, 1.5f);

        Vector3 effectScale = _effect.Effect.transform.localScale;
        _effect.Effect.transform.localScale = new Vector3(_range * 2, effectScale.y, _range * 2);
    }

}
