using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : PlayerState
{
    private float _atttackBufferTime;
    private bool _isCombe;
    private bool _isChangeAttack;
    public ThrowState(PlayerController controller) : base(controller)
    {
        _atttackBufferTime = _player.AttackBufferTime;
    }
    public override void Enter()
    {
        _isChangeAttack = false;
        if (_player.View.GetBool(PlayerView.Parameter.ThrowCombo) == false)
        {
            _player.View.SetTrigger(PlayerView.Parameter.ThrowAttack);
        }
        else
        {
            _player.Model.ComboCount++;
        }
        CoroutineHandler.StartRoutine(MeleeAttackRoutine());
    }

    public override void Update()
    {
        //Debug.Log("Melee");
    }

    public override void Exit()
    {

    }

    IEnumerator MeleeAttackRoutine()
    {
        yield return null;
        float timeCount = _atttackBufferTime;
        while (_player.View.IsAnimationFinish == false)
        {
            // 공격 버퍼
            if (Input.GetButtonDown("Fire2"))
            {
                // 다음 공격 대기
                _isCombe = true;
                _player.View.SetBool(PlayerView.Parameter.ThrowCombo, true);
                timeCount = _atttackBufferTime;
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                // 근접 공격 전환
                _isCombe = false;
                _isChangeAttack = true;
                _player.View.SetBool(PlayerView.Parameter.ThrowCombo, false);
                timeCount = _atttackBufferTime;
            }
            timeCount -= Time.deltaTime;
            if (timeCount <= 0)
            {
                // 다음 공격 취소
                _isCombe = false;
                _player.View.SetBool(PlayerView.Parameter.ThrowCombo, false);
                timeCount = _atttackBufferTime;
            }

            yield return null;
        }

        if (_isCombe == true)
        {
            _player.ChangeState(PlayerController.State.ThrowAttack);
        }
        else if (_isChangeAttack == true)
        {
            _player.Model.ComboCount = 0;
            _player.ChangeState(PlayerController.State.MeleeAttack);
        }
        else
        {
            _player.Model.ComboCount = 0;
            _player.ChangeState(PlayerController.State.Idle);
        }

    }
}
