using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "DashCritical", menuName = "AdditionalEffect/Player/DashCritical")]
public class DashCriticalAdditional : PlayerAdditional
{
    [Header("치명타 증가량(%)")]
    [SerializeField] private float _increaseCritical;
    [Header("크리티걸 공격 횟수")]
    [SerializeField] private int _maxCriticalCount;
    private int _criticalCount;
    public override void Enter()
    {
        // 투척오브젝트 결과 이벤트 구독
        Player.OnThrowObjectResult += CheckThrowObjectResult;
    }
    public override void Exit()
    {
        // 투척오브젝트 결과 이벤트 구독 해제
        Player.OnThrowObjectResult -= CheckThrowObjectResult;
    }

    public override void Trigger()
    {
        // 대쉬 했을 떄 치명타 증가
        if(CurState == PlayerController.State.Dash)
        {
            IncreaseCritical();
        }
        // 근접공격 시 공격 카운트 감소
        else if (CurState == PlayerController.State.MeleeAttack)
        {
            CheckAttackCount();
        }
        // 특수 공격시 공격 카운트 감소
        else if (CurState == PlayerController.State.SpecialAttack)
        {
            CheckAttackCount();
        }
    }

    /// <summary>
    /// 크리티컬 증가
    /// </summary>
    private void IncreaseCritical()
    {
        if (_criticalCount == 0)
        {
            Model.CriticalChance += _increaseCritical;
        }
        _criticalCount = _maxCriticalCount;
    }

    /// <summary>
    /// 크리티컬 감소
    /// </summary>
    private void DecreaseCritical()
    {
        Model.CriticalChance -= _increaseCritical;
    }

    private void CheckAttackCount()
    {
        // 공격카운트 1씩 감소, 0일때 다시 크리티컬 확률 정상화
        if (_criticalCount <= 0)
            return;

        _criticalCount--;
        if(_criticalCount <= 0)
        {
            DecreaseCritical();
        }
    }

    private void CheckThrowObjectResult(ThrowObject throwObject, bool success)
    {
        // 투척공격 적중시 공격카운트 감소
        if(success == true)
        {
            CheckAttackCount();
        }
    }
}
