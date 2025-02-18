using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "Adrenaline", menuName = "AdditionalEffect/Player/Adrenaline")]
public class AdrenalineAddtional : PlayerAdditional
{

    [Header("공격력 증가량")]
    [SerializeField] private int _increaseDamage;
    [Header("최대 스택")]
    [SerializeField] private int _maxCount;

    private int _curCount;
    public override void Enter()
    {
        Player.OnThrowObjectResult += CheckHitEnemy;
    }
    public override void Exit()
    {
        Player.OnThrowObjectResult -= CheckHitEnemy;

        // 값 다시 초기화
        Model.AttackPower = GetPlayerAttackPower(-(_increaseDamage * _curCount));
        _curCount = 0;
    }

    private void CheckHitEnemy(ThrowObject throwObject,bool success)
    {
        if(success == true)
        {
            CountUp();
        }
        else
        {
            ResetCount(throwObject);
        }
    }

    private void CountUp()
    {
        // 최대 스택 초과 불가
        if (_curCount >= _maxCount)
            return;

        // 스택 증가
        _curCount++;
        Model.AttackPower = GetPlayerAttackPower(_increaseDamage);
    }

    private void ResetCount(ThrowObject throwObject)
    {
        // 체인 리스트중 하나이상(본인 말고 다른 체인된 오브젝트도 존재할 때
        if (throwObject.ChainList.Count > 1)
            return;
        // 체인 리스트중 하나라도 맞추는데 성공했을 때
        if (throwObject.IsChainHit == true)
            return;

        Model.AttackPower = GetPlayerAttackPower(-(_increaseDamage * _curCount));
        _curCount = 0;
    }
}
