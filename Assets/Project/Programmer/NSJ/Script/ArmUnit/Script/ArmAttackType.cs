using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.SpaceFighter;

public class ArmAttackType : ScriptableObject
{
    [HideInInspector] public PlayerController Player;
    public PlayerModel Model => Player.Model;
    public PlayerView View => Player.View;
    protected Transform transform => Player.transform;
    protected GameObject gameObject => Player.gameObject;
    protected Transform _muzzlePoint => Player.MuzzletPoint;
    protected Rigidbody Rb => Player.Rb;
    protected BattleSystem Battle => Player.Battle;
    protected PlayerBinder Binder => View.Binder;

    protected ArmUnit arm;

    private int m_index;
    protected int _index { get {  return m_index; } set { m_index = value; Model.ChargeStep = m_index; } }
    protected bool _isSpecialCharge;
    public virtual void Init(PlayerController player, ArmUnit arm)
    {
        Player = player;
        this.arm = arm;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnDrawGizmos() { }
    public virtual void OnTrigger() { }
    public virtual void EndAnimation() { }
    public virtual void OnCombo() { }
    public virtual void EndCombo() { }
    protected void ChangeState(PlayerController.State state)
    {
        Player.ChangeState(state);
    }
}   
