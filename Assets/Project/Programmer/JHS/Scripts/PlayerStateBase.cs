using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStateBase", menuName = "Game/PlayerStateBase")]
public class PlayerStateBase : ScriptableObject
{
    [Header("플레이어 기본 스탯")]
    [Tooltip("최대 체력")]
    public float maxHp = 60;
    [Tooltip("공용 공격력")]
    public float commonAttack = 0;
    [Tooltip("근거리 공격력")]
    public float[] shortRangeAttack = new float[] { 25, 40, 60 };
    [Tooltip("원거리 공격력")]    
    public float[] longRangeAttack = new float[] { 10, 40, 70, 110 };
    [Tooltip("공격 속도")]
    public float attackSpeed = 1;
    [Tooltip("이동 속도")]
    public float movementSpeed = 100;
    [Tooltip("크리티컬 확률")]
    public float criticalChance = 10;
    [Tooltip("방어력")]
    public float defense = 0;
    [Tooltip("장비획득 확률 증가")]
    public float equipmentDropUpgrade = 0;
    [Tooltip("생명력 흡수")]
    public float drainLife = 0;
    [Tooltip("스테미나 최대치")]
    public float maxStamina = 100;
    [Tooltip("스테미나 회복")]
    public float regainStamina = 20;
    [Tooltip("던지기 공격당 마나 회복")]
    public float[] regainMana = new float[] { 3, 8, 13, 20 };
    [Tooltip("마나 소모량 감소")]
    public float[] manaConsumption = new float[] { 30, 70, 100 };
    [Tooltip("스테미나 소모량 감소")]
    public float consumesStamina = 0;
    [Tooltip("투척물 추가획득 증가")]
    public float gainMoreThrowables = 0;
    [Tooltip("보유 투척물 제한")]
    public float maxThrowables = 50;
    [Tooltip("최대 마나")]
    public float maxMana = 100;
    [Tooltip("최대 점프 횟수")]
    public float maxJumpCount = 2;
    [Tooltip("점프력")]
    public float jumpPower = 100;
    [Tooltip("점프 소모 스테미나")]
    public float jumpConsumesStamina = 20;
    [Tooltip("더블점프 소모 스테미나")]
    public float doubleJumpConsumesStamina = 10;
    [Tooltip("대쉬 거리")]
    public float dashDistance = 200;
    [Tooltip("대쉬 소모 스테미나")]
    public float dashConsumesStamina = 50;
    [Tooltip("근접공격 소모 스테미나")]
    public float[] shortRangeAttackStamina = new float[] { 20, 30, 50 };
    [Tooltip("특수 공격력 수치")]
    public float[] specialAttack = new float[] { 75, 150, 225 };
}
