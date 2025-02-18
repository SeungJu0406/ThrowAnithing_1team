using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LobbyUpGradeState", menuName = "Game/LobbyUpGradeState")]
public class LobbyUpGradeState : ScriptableObject
{
    [Header("로비 강화 수치")]
    [Tooltip("근거리 공격 1")]
    public float shortRangeAttackUPO = 1;
    [Tooltip("원거리 공격 1")]
    public float longRangeAttackUPO = 1;
    [Tooltip("이동속도 증가")]
    public float movementSpeedUP = 4;
    [Tooltip("최대 체력")]
    public float maxHpUP = 6;
    [Tooltip("스테미나 최대치")]
    public float maxStaminaUP = 10;
    [Tooltip("공격 속도")]
    public float attackSpeedUP = 5;
    [Tooltip("크리티컬 확률")]
    public float criticalChanceUP = 2;
    [Tooltip("장비획득 확률 증가 1")]
    public float equipmentDropUpgradeUPO = 2;
    [Tooltip("공용 공격력")]
    public float commonAttackUP = 2;
    [Tooltip("보유 투척물 제한")]
    public float maxThrowablesUP = 6;
    [Tooltip("방어력 1")]
    public float defenseUPO = 0.4f;
    [Tooltip("마나회복량 증가")]
    public float regainManaUP = 10;
    [Tooltip("스테미나 소모량 감소")]
    public float consumesStaminaUP = 6;
    [Tooltip("원거리 공격 2")]
    public float longRangeAttackUPT = 2;
    [Tooltip("근거리 공격 2")]
    public float shortRangeAttackUPT = 2;
    [Tooltip("투척물 추가획득 확률")]
    public float gainMoreThrowablesUP = 20;
    [Tooltip("마나 소모량 감소")]
    public float manaConsumptionUP = 6;
    [Tooltip("생명력 흡수")]
    public float drainLifeUP = 0.6f;
    [Tooltip("방어력 2")]
    public float defenseUPT = 0.6f;
    [Tooltip("장비획득 확률 증가 2")]
    public float equipmentDropUpgradeUPT = 3;
}
