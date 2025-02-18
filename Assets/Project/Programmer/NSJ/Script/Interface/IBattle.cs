using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IBattle
{
    IHit Hit { get; set; }

    public event UnityAction<int, bool> OnTargetAttackEvent;
    public event UnityAction<int, bool> OnTakeDamageEvent;
    public event UnityAction OnDieEvent;
    public void TakeDebuff(HitAdditional debuff);
    public void TakeDebuff(List<HitAdditional> debuff);
    public void TakeCrowdControl(CrowdControlType type, float time);
    public int TakeDamage(int damage);
    public int TakeDamage(bool isCritical, int damage);
    public int TakeDamage(int damage, bool IsIgnoreDef);
    public int TakeDamage(bool isCritical, int damage, bool IsIgnoreDef);
    public int TakeDamageWithDebuff(bool isCritical, int damage, List<HitAdditional> hitAdditionals);
    public int TakeDamageWithDebuff(int damage, List<HitAdditional> hitAdditionals, bool IsIgnoreDef);
    public int TakeDamageWithDebuff(bool isCritical, int damage,  List<HitAdditional> hitAdditionals, bool IsIgnoreDef);
    public void Enter();
    public void Exit();
    public void Update();
    public void FixedUpdate();
    public void Trigger();
    public void EndDebuff(HitAdditional debuff);
}
