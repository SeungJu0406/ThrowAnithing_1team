using System.Collections;
using UnityEngine;

public class ThrowState : PlayerState
{
    private Transform _muzzlePoint;

  
    public ThrowState(PlayerController controller) : base(controller)
    {
        _muzzlePoint = controller.MuzzletPoint;
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
