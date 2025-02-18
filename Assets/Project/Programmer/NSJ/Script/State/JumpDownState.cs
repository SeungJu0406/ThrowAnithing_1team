using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDownState : PlayerState
{
    public JumpDownState(PlayerController controller) : base(controller)
    {
        UseStamina = true;
        StaminaAmount = Model.JumpDownStamina;
        IsIgnoreMonster = true;
    }
    public override void InitArm()
    {
        StaminaAmount = Model.JumpDownStamina;
    }
    public override void Enter()
    {
        Arm.Enter();
    }
    public override void Exit()
    {
        Arm.Exit();
    }
    public override void Update()
    {
        Arm.Update();
    }
    public override void FixedUpdate()
    {
        Arm.FixedUpdate();
    }
    public override void OnDrawGizmos()
    {
        Arm.OnDrawGizmos();
    }
    public override void OnTrigger()
    {
        Arm.OnTrigger();
    }
    public override void EndAnimation()
    {
        Arm.EndAnimation();
    }
    public override void OnCombo()
    {
        Arm.OnCombo();
    }
    public override void EndCombo()
    {
        Arm.EndCombo();
    }
}
