using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KillingDrain", menuName = "AdditionalEffect/Player/KillingDrain")]
public class KillingDrainAdditonal : PlayerAdditional
{
    [Header("°ø°Ý·Â ¹èÀ²(%)")]
    [SerializeField] private float _attackMultiplier;
    [Header("ÇÇÇØ ÈíÇ÷(%)")]
    [SerializeField] private float _lifeDrainAmount;

    private float _drainDistance => Model.DrainDistance * 2;
    private float _curDrainDistance;

    private bool _isStop;

    Coroutine _additonalRoutine;
    Coroutine _drainRangeRoutine;
    public override void Enter()
    {
        if (_additonalRoutine == null)
            _additonalRoutine = CoroutineHandler.StartRoutine(AdditonalRoutine());
        if (_drainRangeRoutine == null)
            _drainRangeRoutine = CoroutineHandler.StartRoutine(DrainRangeRoutine());
    }
    public override void Exit()
    {
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
            // Å¸°Ù ½ºÄµ
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _curDrainDistance, Player.OverLapColliders, 1 << Layer.Monster);
            for (int i = 0; i < hitCount; i++)
            {
                // µ¥¹ÌÁöÁÖ±â
                int drainDamage = (int)(Model.AttackPower * _attackMultiplier / 100f);
                int finalDamage = Player.GetDamage(drainDamage);

                int hitDamage = Player.Battle.TargetAttack(Player.OverLapColliders[i], finalDamage,  false );

                // ÇÇÈí
                float drainAmount = (hitDamage * _lifeDrainAmount / 100f);
                if(0 <drainAmount  && drainAmount < 1) 
                    drainAmount = 1;
                else if( drainAmount <= 0)
                    drainAmount = 0;

                Model.CurHp += (int)drainAmount;
            }
            yield return 0.5f.GetDelay();
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

            // Á¡Â÷ Áõ°¡
            _curDrainDistance += Time.deltaTime * 10;
            if (_curDrainDistance > Model.DrainDistance)
                _curDrainDistance = Model.DrainDistance;
            yield return null;
        }
    }
}
