using System;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "RichPower", menuName = "AdditionalEffect/Player/RichPower")]
public class RichPowerAdditional : PlayerAdditional
{
    [Header("투척물 당 공격력 증가량(%)")]
    [SerializeField] private float _increaseDamage;


    private int _prevThrowableCount;
    IDisposable _disposable;
    public override void Enter()
    {    
        _disposable = Model.CurThrowCountSubject
            .DistinctUntilChanged()
            .Subscribe(x => { ChangeDamage(x); });

        _prevThrowableCount = Model.CurThrowables;
        Model.AttackPowerMultiplier += _increaseDamage * Model.CurThrowables;
    }
    public override void Exit()
    {
        _disposable.Dispose();
        Model.AttackPowerMultiplier -= _increaseDamage * Model.CurThrowables;
    }

    private void ChangeDamage(int count)
    {
        if(_prevThrowableCount > count)
        {
            Model.AttackPowerMultiplier -= _increaseDamage;
        }
        else if( _prevThrowableCount < count)
        {
            Model.AttackPowerMultiplier += _increaseDamage;
        }
        _prevThrowableCount = Model.CurThrowables;
    }
}
