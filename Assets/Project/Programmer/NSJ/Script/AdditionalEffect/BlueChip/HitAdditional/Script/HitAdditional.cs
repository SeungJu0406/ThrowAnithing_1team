using BehaviorDesigner.Runtime;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

public class HitAdditional : AdditionalEffect
{
    [HideInInspector] public BattleSystem Battle;
    [Header("지속 시간")]
    public float Duration;

    protected int _damage;
    protected bool _isCritical;
    protected float _remainDuration;
    protected Coroutine _debuffRoutine;
    public Transform transform => Battle.transform;
    protected GameObject gameObject => transform.gameObject;

    protected enum TargetType { Player, Enemy}
    protected TargetType _targetType;
    protected PlayerController Player;
    protected BaseEnemy Enemy;

    public void Init(int damage, bool isCritical, float remainDuration)
    {
        _damage = damage;
        _isCritical = isCritical;
        _remainDuration = remainDuration;

        if(transform.tag == Tag.Player)
        {
            Player = transform.GetComponent<PlayerController>();
            _targetType = TargetType.Player;
        }
        else if(transform.tag == Tag.Monster)
        {
            Enemy = transform.GetComponent<BaseEnemy>();
            _targetType = TargetType.Enemy;
        }
    }

    public float GetEnemyMoveSpeed()
    {
        SharedFloat moveSpeed= Enemy.GetBT().GetVariable("Speed") as SharedFloat;
        return moveSpeed.Value;
    }
    public void SetEnemyMoveSpeed(float moveSpeed)
    {
        Enemy.GetBT().SetVariable("Speed", (SharedFloat)moveSpeed);
    }
    public float GetEnemyAttackSpeed()
    {
        SharedFloat attackSpeed = Enemy.GetBT().GetVariable("AtkDelay") as SharedFloat;
        return attackSpeed.Value;
    }
    public void SetEnemyAttackSpeed(float attackSpeed)
    {
        Enemy.GetBT().SetVariable("AtkDelay", (SharedFloat)attackSpeed);
    }
}
