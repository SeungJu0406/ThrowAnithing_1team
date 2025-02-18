using UnityEngine;

public class DoubleJumpState : PlayerState
{
    public DoubleJumpState(PlayerController controller) : base(controller)
    {
        UseStamina = true;
       IsIgnoreMonster = true;
    }
    public override void InitArm()
    {
        StaminaAmount = Model.DoubleJumpStamina;
    }
    public override void Enter()
    {
        View.SetTrigger(PlayerView.Parameter.DoubleJump);

        SoundManager.PlaySFX(Player.Sound.Move.DoubleJump);
    }
    public override void Exit()
    {

    }

    public override void Update()
    {
        // Debug.Log("PrevJump");
    }

    public override void OnTrigger()
    {
        Jump();
    }

    private void Jump()
    {
        
        // 임시 물리량 저장
        Rb.velocity = new Vector3(Rb.velocity.x, 0, Rb.velocity.z); // y값 물리량 제거
        if (MoveDir != Vector3.zero)
        {
            Player.LookAtMoveDir();
            Player.ChangeVelocityPlayerFoward();
        }
        
        Rb.AddForce(Vector3.up * Model.JumpPower, ForceMode.Impulse);
        // 파티클 실행
        ObjectPool.Get(Effect.DoubleJump, transform.position, Effect.DoubleJump.transform.rotation, 1f);
        // 점프 이후 바로 추락 모드 실행
        ChangeState(PlayerController.State.DoubleJumpFall);
    }

    private void Temp()
    {

    }
}
