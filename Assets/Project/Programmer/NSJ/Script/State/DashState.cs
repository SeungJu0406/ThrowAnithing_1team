using System;
using System.Collections;
using UnityEngine;

public class DashState : PlayerState
{
    //float _timeBuffer;
    bool _isDashEnd;
    Vector3 _startPos;
    Vector3 _prevPos;
    bool canMove = true;

    GameObject _armEffect;
    GameObject _frontEffect;

    Coroutine _checkInputRoutine;
    Coroutine _checkCanMove;
    Coroutine _dashEffectRoutine;
    public DashState(PlayerController controller) : base(controller)
    {
        UseStamina = true;
        IsIgnoreMonster = true;
    }
    public override void InitArm()
    {
        StaminaAmount = Model.DashStamina;
    }
    public override void Enter()
    {
        _isDashEnd = false;

        _startPos = transform.position;
        Player.IsInvincible = true;
        Player.LookAtMoveDir();
        View.SetTrigger(PlayerView.Parameter.Dash);

        _checkCanMove = CoroutineHandler.StartRoutine(_checkCanMove, CheckCanMove());
        _dashEffectRoutine = CoroutineHandler.StartRoutine(_dashEffectRoutine, DashEffectRoutine());

        // 사운드
        SoundManager.PlaySFX(Player.Sound.Move.Dash);

    }
    public override void Exit()
    {
        _checkCanMove = CoroutineHandler.StopRoutine(_checkCanMove);    
        _dashEffectRoutine = CoroutineHandler.StopRoutine(_dashEffectRoutine);

        Player.IsInvincible = false;
        canMove = true;
    }
    public override void Update()
    {
        Dash();
    }
    public override void OnTrigger() 
    {

    }
    public override void EndAnimation()
    {
        if (Player.IsGround != true)
        {
            Rb.velocity /= 2;
            if (Player.IsDoubleJump == false)
                ChangeState(PlayerController.State.Fall);
            else
                ChangeState(PlayerController.State.DoubleJumpFall);
        }
        else
        {
            ChangeState(PlayerController.State.Idle);
        }
        _armEffect.transform.SetParent(null);

    }
    /// <summary>
    /// 대쉬
    /// </summary>
    public void Dash()
    {
        if (canMove == false && _isDashEnd == false)
        {
            EndDash();
            return;
        }
        else
        {
            if (Vector3.Distance(transform.position, _startPos) < Model.DashDistance && Player.IsWall == false)
            {
                Rb.velocity = transform.forward * Model.MoveSpeed * 2;
            }
            else if (_isDashEnd == false)
            {
                EndDash();
            }
        }
    }

    private void EndDash()
    {
        _isDashEnd = true;
        View.SetTrigger(PlayerView.Parameter.DashEnd);
        //Rb.velocity = transform.forward * Model.MoveSpeed;
        Player.StartCoroutine(DashEndRoutine());
    }

    IEnumerator DashEndRoutine()
    {
        float speed = Model.MoveSpeed * 2;
        while (true)
        {
            Rb.velocity = transform.forward * speed;
            speed -= Time.deltaTime * 20f;
            if (speed < 0)
            {
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CheckCanMove()
    {
        canMove = true;
        while (true)
        {
            yield return 0.1f.GetDelay();
            if (Vector3.Distance(_prevPos, transform.position) < 0.05f)
            {
                canMove = false;
            }
            _prevPos = transform.position;
        }
    }

    IEnumerator DashEffectRoutine()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        _armEffect = ObjectPool.Get(Effect.Dash_Arm, Player.DashArmPoint, 2f);
        yield return null;
        yield return null;
        _frontEffect = ObjectPool.Get(Effect.Dash_Front, Player.DashFrountPoint, 2f);
    }
}
