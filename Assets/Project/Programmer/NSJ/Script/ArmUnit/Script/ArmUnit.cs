using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ArmUnit : ScriptableObject
{
    [SerializeField] public string Name;
    [SerializeField] public GlobalGameData.AmWeapon ArmType;
    protected enum Type { Throw, Melee, Special, JumpDown, JumpAttack, Size }
    [HideInInspector] public PlayerController Player;
    public PlayerModel Model => Player.Model;
    public PlayerView View => Player.View;
    protected Transform transform => Player.transform;
    protected GameObject gameObject => Player.gameObject;
    protected Transform _muzzlePoint => Player.MuzzletPoint;

    [SerializeField] protected AttackTypeStruct _attackType;
    [SerializeField] protected ArmAttackType[] _types;

    [System.Serializable]
    protected struct AttackTypeStruct
    {
        public ArmThrowAttack ThrowAttack;
        public ArmMeleeAttack MeleeAttack;
        public ArmSpecialAttack SpecialAttack;
        public ArmJumpDown JumpDown;
        public ArmJumpAttack JumpAttack;
    }
    public ArmThrowAttack ThrowAttack { get { return _attackType.ThrowAttack; } set { _attackType.ThrowAttack = value; } }
    public ArmMeleeAttack MeleeAttack { get { return _attackType.MeleeAttack; } set { _attackType.MeleeAttack = value; } }
    public ArmSpecialAttack SpecialAttack { get { return _attackType.SpecialAttack; } set { _attackType.SpecialAttack = value; } }
    public ArmJumpDown JumpDown { get { return _attackType.JumpDown; } set { _attackType.JumpDown = value; } }
    public ArmJumpAttack JumpAttack { get { return _attackType.JumpAttack; } set { _attackType.JumpAttack = value; } }

    [System.Serializable]
    struct InitStruct
    {
        [Header("대쉬 스테미나(%)")]
        public float DashStamina;
        [Header("대쉬 거리(%)")]
        public float DashDistance;
        [Header("드레인 스테미나(%)")]
        public float DrainStamina;
        [Header("드레인 거리(%)")]
        public float DrainDistance;
        [Header("최대 마나")]
        public float MaxMana;
    }
    [SerializeField] InitStruct _init;
    public virtual void Init(PlayerController player)
    {
        Player = player;
        _types = new ArmAttackType[(int)Type.Size];

        Model.DashStamina = (int)(Model.GlobalStateData.dashConsumesStamina * (_init.DashStamina / 100f));
        Model.DashDistance = (int)(Model.GlobalStateData.dashDistance * (_init.DashDistance / 100f));

        Model.DrainDistance = Model.Drain.Default.DrainDistance * (_init.DrainDistance / 100f);
        Model.DrainStamina = Model.Drain.Default.DrainStamina * (_init.DrainStamina / 100f);

        Model.MaxMana = Model.GlobalStateData.maxMana + _init.MaxMana - Model.GlobalStateData.maxMana;

        InitAllType();
    }
    public virtual void OnDestroy()
    {
        foreach (ArmAttackType attackType in _types)
        {
            Destroy(attackType);
        }
    }


    public virtual void Enter()
    {
        SelectType().Enter();
    }
    public virtual void Exit()
    {
        SelectType().Exit();
    }
    public virtual void Update()
    {
        SelectType().Update();
    }
    public virtual void FixedUpdate()
    {
        SelectType().FixedUpdate();
    }
    public virtual void OnDrawGizmos()
    {
        SelectType().OnDrawGizmos();
    }
    public virtual void OnTrigger()
    {
        SelectType().OnTrigger();
    }
    public virtual void EndAnimation()
    {
        SelectType().EndAnimation();
    }
    public virtual void OnCombo()
    {
        SelectType().OnCombo();
    }
    public virtual void EndCombo()
    {
        SelectType().EndCombo();
    }

    private ArmAttackType SelectType()
    {
        ArmAttackType attackType = null;
        switch (Player.CurState)
        {
            case PlayerController.State.ThrowAttack:
                attackType = _types[(int)Type.Throw];
                break;
            case PlayerController.State.MeleeAttack:
                attackType = _types[(int)Type.Melee];
                break;
            case PlayerController.State.SpecialAttack:
                attackType = _types[(int)Type.Special];
                break;
            case PlayerController.State.JumpDown:
                attackType = _types[(int)Type.JumpDown];
                break;
            case PlayerController.State.JumpAttack:
                attackType = _types[(int)Type.JumpAttack];
                break;
        }
        return attackType;
    }
    protected void InitType(Type type, ArmAttackType armAttackType)
    {
        _types[(int)type] = Instantiate(armAttackType);
        _types[(int)type]?.Init(Player, this);
    }
    protected void InitAllType()
    {
        InitType(Type.Throw, ThrowAttack);
        InitType(Type.Melee, MeleeAttack);
        InitType(Type.Special, SpecialAttack);
        InitType(Type.JumpDown, JumpDown);
        InitType(Type.JumpAttack, JumpAttack);
    }
}
