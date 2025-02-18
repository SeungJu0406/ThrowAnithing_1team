using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Trance", menuName = "AdditionalEffect/Player/Trance")]
public class TranceAddtional : PlayerAdditional
{
    [Header("데미지 감소량 (%)")]
    [SerializeField] float DecreaseDamage;
    [Header("공격속도 증가량 (%)")]
    [SerializeField] float IncreaseAttackSpeed;
    public override void Enter()
    {
        Model.DamageMultiplier -= DecreaseDamage;
        Model.AttackSpeedMultiplier += IncreaseAttackSpeed;
    }
    public override void Exit()
    {
        Model.DamageMultiplier += DecreaseDamage;
        Model.AttackSpeedMultiplier -= IncreaseAttackSpeed;
    }
}
