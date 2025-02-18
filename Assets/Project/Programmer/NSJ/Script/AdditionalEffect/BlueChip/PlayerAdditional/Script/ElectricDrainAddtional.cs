using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.SpaceFighter;

[CreateAssetMenu(fileName = "ElectricDrain", menuName = "AdditionalEffect/Player/ElectricDrain")]
public class ElectricDrainAddtional : PlayerAdditional
{
    [System.Serializable]
    struct EffectStruct
    {
        public GameObject Effect;
        public GameObject Particle;
        [HideInInspector] public GameObject OriginEffect;
    }
    [SerializeField] EffectStruct _effect;
    [SerializeField] ElectricShockAdditonal _electricShockOrigin;
    private ElectricShockAdditonal _electricShock;
    [Header("데미지")]
    [SerializeField] private int _damage;

    private float _drainDistance => Model.DrainDistance * 2;
    private float _curDrainDistance;

    private bool _isStop;

    Coroutine _additonalRoutine;
    Coroutine _drainRangeRoutine;
    public override void Enter()
    {
        // 드레인 이펙트 교체
        _effect.OriginEffect = Player.Effect.Drain_Range;
        Player.Effect.Drain_Range = _effect.Effect;
       // 디버프 클론 생성
       _electricShock = Instantiate(_electricShockOrigin);
        _electricShock.Probability = 100;


        if (_additonalRoutine == null)
            _additonalRoutine = CoroutineHandler.StartRoutine(AdditonalRoutine());
        if (_drainRangeRoutine == null)
            _drainRangeRoutine = CoroutineHandler.StartRoutine(DrainRangeRoutine());
    }
    public override void Exit()
    {
        // 클론 디버프 삭제
        Destroy(_electricShock);
        // 드레인 이펙트 원상복구
        Player.Effect.Drain_Range = _effect.OriginEffect;

        if (_additonalRoutine != null)
        {
            CoroutineHandler.StopRoutine(_additonalRoutine);
            _additonalRoutine = null;
        }
        if (_drainRangeRoutine != null)
        {
            CoroutineHandler.StopRoutine(_drainRangeRoutine);
            _drainRangeRoutine = null;
        }
    }

    public override void Trigger()
    {
        if (Player.CurState != PlayerController.State.Drain)
            return;

        _isStop = true;
        _curDrainDistance = 0;
    }

    IEnumerator AdditonalRoutine()
    {
        while (true)
        {
            if (_isStop == true)
            {
                yield return null;
                continue;
            }
            if (Player.CurState != PlayerController.State.Drain)
            {
                yield return null;
                continue;
            }
            // 타겟 스캔
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _curDrainDistance, Player.OverLapColliders, 1 << Layer.Monster);
            for (int i = 0; i < hitCount; i++)
            {
                // 데미지, 디버프 주기
                int finalDamage = Player.GetDamage(_damage);
                Player.Battle.TargetAttack(Player.OverLapColliders[i], finalDamage, false);
                Player.Battle.TargetDebuff(Player.OverLapColliders[i], _electricShock);
            }

            // 파티클 이펙트 생성
            if (_curDrainDistance >= Model.DrainDistance)
            {
                GameObject effect = ObjectPool.Get(_effect.Particle, transform, 1f);
            }
            yield return 1f.GetDelay();
        }
    }
    IEnumerator DrainRangeRoutine()
    {
        while (true)
        {
            if (_isStop == true)
            {
                if (Player.CurState != PlayerController.State.Drain)
                    _isStop = false;
                yield return null;
                continue;
            }
            if (Player.CurState != PlayerController.State.Drain)
            {
                yield return null;
                continue;
            }

            // 점차 증가
            _curDrainDistance += Time.deltaTime * 10;
            if (_curDrainDistance > Model.DrainDistance)
                _curDrainDistance = Model.DrainDistance;
            yield return null;
        }
    }
}
