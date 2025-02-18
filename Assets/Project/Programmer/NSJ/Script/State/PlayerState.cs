using Assets.Project.Programmer.NSJ.RND.Script.State;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Zenject.SpaceFighter;

public class PlayerState : BaseState
{
    public bool UseStamina;
    public float StaminaAmount;
    public bool CantChangeState;
    public bool IsIgnoreMonster;
    protected PlayerController Player;

    protected GameObject gameObject => Player.gameObject;
    protected Transform transform => Player.transform;
    protected PlayerModel Model =>Player.Model;
    protected PlayerView View => Player.View;
    protected ArmUnit Arm => Model.Arm;
    protected Rigidbody Rb => Player.Rb;
    protected PlayerEffectData Effect => Player.Effect;
    protected Vector3 MoveDir => Player.MoveDir;
    public PlayerState(PlayerController controller)
    {
        Player = controller;
    }
    public virtual void OnTrigger() { }
    public virtual void EndAnimation() { }
    public virtual void OnCombo() { }
    public virtual void EndCombo() { }
    public virtual void TriggerCantOperate() { }
    public virtual void InitArm() { }
    protected void ChangeState(PlayerController.State state)
    {
        Player.ChangeState(state);
    }
}
