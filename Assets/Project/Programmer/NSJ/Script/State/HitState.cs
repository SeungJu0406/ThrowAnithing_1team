using UnityEngine;

public class HitState : PlayerState
{
    public HitState(PlayerController controller) : base(controller)
    {
        Player.OnPlayerHitFuncEvent += TakeDamage;
        Player.OnPlayerCCHitEvent += TakeCrowdControl;
    }


    public override void Enter()
    {
        // 움직임 멈춤
        Rb.velocity = new Vector3(0, Rb.velocity.y, 0);

        // 플레이어 무적상태
        Player.IsInvincible = true;

        View.SetTrigger(PlayerView.Parameter.Hit);
    }

    public override void Exit()
    {
        // 플레이어 무적 해제
        Player.IsInvincible = false;

        if (Player.IsGround == true)
        {
            // 점프 중이었을때를 위한 조건 해제
            Player.IsJumpAttack = false;
            Player.IsDoubleJump = false;
        }
    }

    public override void EndAnimation()
    {
        ChangeState(PlayerController.State.Idle);
    }

    private int TakeDamage(int damage, bool isIgnoreDef)
    {
        if (Player.IsShield == true)
            return 0;
        if (isIgnoreDef == false)
        {
            if (Player.IsInvincible == true)
                return 0;
        }
        int finalDamage = 0;
        // 고정데미지 아닐 때 방어력 계산
        if (isIgnoreDef == false)
        {
            // 방어력 계산
            finalDamage = damage - Model.Defense;
            // 받는 피해 감소 계산
            finalDamage = (int)(finalDamage * (1 - Model.DamageReduction / 100f));
            // 0보다 작으면 값을 0으로 고정 시킴
            finalDamage = finalDamage <= 0 ? 0 : finalDamage;
        }
        else
        {
            finalDamage = damage;
        }
        // 입은 데미지가 없을때 Hit 상태 안들어감
        if (finalDamage == 0)
            return finalDamage;

        Model.CurHp -= finalDamage;


        if (Model.CurHp <= 0)
        {          
            // Die
            Player.Die();
        }
        return finalDamage;
    }

    private void TakeCrowdControl(CrowdControlType type) 
    {
        if (Player.IsShield == true)
            return;
        if (Player.IsInvincible == true)
            return;

        if (type == CrowdControlType.Stiff)
        {
            if (Player.CurState == PlayerController.State.Hit)
                return;

            ChangeState(PlayerController.State.Hit);
        }
    }
}
