using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController controller) : base(controller)
    {

    }

    public override void Enter()
    {
        Player.Rb.velocity = Vector3.zero;

        View.SetBool(PlayerView.Parameter.Idle,true);
    }

    public override void Exit()
    {
        View.SetBool(PlayerView.Parameter.Idle, false);
    }
    public override void Update()
    {
        // 이동키 입력시 Run, 이동속도가 0보다는 클때
        if (MoveDir != Vector3.zero && Model.MoveSpeed > 0)
        {
            ChangeState(PlayerController.State.Run);
        }
        // 1번 공격키 입력시 근접공격
        else if (InputKey.GetButtonDown(InputKey.Melee))
        {
            ChangeState(PlayerController.State.MeleeAttack);
        }
        // 2번 공격키 입력시 투척 공격
        else if (InputKey.GetButtonDown(InputKey.Throw))
        {
            ChangeState(PlayerController.State.ThrowAttack);
        }
        // 특수공격 키 입력시 특수 공격
        else if (InputKey.GetButtonDown(InputKey.Special))
        {
            ChangeState(PlayerController.State.SpecialAttack);
        }
        // 지면에서 점프 키 입력 시 점프
        else if (Player.IsGround == true && InputKey.GetButtonDown(InputKey.Jump))
        {
            ChangeState(PlayerController.State.Jump);
        }
        // 공중에서 y축 물리값 음수일때 추락
        else if (Player.IsGround == false && Rb.velocity.y <= -1f && Player.IsNearGround == false)
        {
            ChangeState(PlayerController.State.Fall);
        }
        // 드레인 키를 눌렀을 경우
        else if (InputKey.GetButtonDown(InputKey.Drain))
        {
            ChangeState(PlayerController.State.Drain);
        }
    }
    public override void FixedUpdate()
    {
        
    }

}
