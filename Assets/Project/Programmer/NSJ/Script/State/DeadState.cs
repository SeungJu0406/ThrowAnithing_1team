using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : PlayerState
{
    public DeadState(PlayerController controller) : base(controller)
    {
        Player.OnPlayerDieEvent += Die;
        IsIgnoreMonster = true;
    }



    public override void Enter()
    {
        Player.IsInvincible = true;
        Model.Data.IsDead = true;
        View.SetTrigger(PlayerView.Parameter.Dead);
    }

    public override void Update()
    {
        Rb.velocity = new Vector3(0,Rb.velocity.y,0);
    }

    private void Die()
    {
        ChangeState(PlayerController.State.Dead);
    }
}
