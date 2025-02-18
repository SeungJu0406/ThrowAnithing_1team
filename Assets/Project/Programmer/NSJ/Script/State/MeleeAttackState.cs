using Assets.Project.Programmer.NSJ.RND.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttackState : PlayerState
{
    Coroutine _meleeRoutine;
    public MeleeAttackState(PlayerController controller) : base(controller)
    {
        UseStamina = true;
        StaminaAmount = Model.MeleeAttackStamina[0];
    }
    public override void InitArm()
    {
        if(Model.NowWeapon == GlobalGameData.AmWeapon.Balance)
        {
            BalanceMeleeAttack meleeAttack = Arm.MeleeAttack as BalanceMeleeAttack;
            StaminaAmount = meleeAttack.StaminaAmount;
        }
        else if (Model.NowWeapon == GlobalGameData.AmWeapon.Power)
        {
            StaminaAmount = Model.MeleeAttackStamina[0];
        }      
    }
    public override void Enter() => Arm.Enter();
    public override void Exit() => Arm.Exit();
    public override void Update() => Arm.Update();
    public override void FixedUpdate() => Arm.FixedUpdate();
    public override void OnDrawGizmos() => Arm.OnDrawGizmos();
    public override void OnTrigger() => Arm.OnTrigger();
    public override void EndAnimation() => Arm.EndAnimation();
    public override void OnCombo() => Arm.OnCombo();
    public override void EndCombo() => Arm.EndCombo();
}
