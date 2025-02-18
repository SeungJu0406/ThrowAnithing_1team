using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ContinuousAttack", menuName = "AdditionalEffect/Player/ContinuousAttack")]
public class ContinuousAttackAdditional : PlayerAdditional
{

    [Header("공격력 증가량")]
    [SerializeField] private int _attackPower;
    [Header("지속시간")]
    [SerializeField] private float _duration;

    private bool _isUseSpecial;
    private bool _isTimerStart;

    private float _timer;
    Coroutine _increaseAttackPowerRoutine;

    public override void Enter()
    {
        _isTimerStart = false;
        _timer = _duration;
    }
    public override void Exit()
    {
        // 타이머 돌아가는 중일때 (공격력 증가 상태)
        if(_isTimerStart == true)
        {
            // 다시 공격력 감소
            Model.AttackPower = GetPlayerAttackPower(-_attackPower);
        }
    }

    public override void Update()
    {
        ProcessBuff();
    }

    public override void Trigger()
    {
        // 스킬 사용
        if (CurState != PlayerController.State.SpecialAttack)
            return;

        _isUseSpecial = true;
    }
    private void ProcessBuff()
    {
        // 스킬 사용 시 타이머 5초 재 시작
        if (_isUseSpecial)
        {
            _isUseSpecial = false;

            // 타이머가 돌아가기 시작했을 때 공격력 더해줌(중복 방지)
            if (_isTimerStart == false)
            {
                Model.AttackPower = GetPlayerAttackPower(_attackPower);
            }
            _timer = _duration;
            _isTimerStart = true;

        }

        _timer -= Time.deltaTime;
        if (_timer < 0 && _isTimerStart == true)
        {
            _isTimerStart = false;

            // 다시 공격력 감소
            Model.AttackPower = GetPlayerAttackPower(-_attackPower);
        }
    }
}
