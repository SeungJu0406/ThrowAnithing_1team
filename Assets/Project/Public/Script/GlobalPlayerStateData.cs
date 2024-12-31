using System;
using System.Collections.Generic;
using UnityEngine.Playables;
using Zenject;

/// <summary>
/// 글로벌 플레이어 스텟 데이터
/// 담당자: 정현수
/// 사용 시 허락 맡으시오
/// </summary>
[System.Serializable]
public class GlobalPlayerStateData
{
    // GlobalGameData의 로비 특성 강화 단계 확인 후 플레이어의 스탯 변동
    // 굳이 json으로 저장할 필요 없이 업그레이드 단계를 저장하여 그에 따라 스탯 적용
    // 로비 업그레이드 로직은 따로 제작

    // 로비에서 조작되는 플레이어 스탯
    // 최대 체력 기본 : 60
    public float maxHp;
    // 공용 공격력 기본 : 0
    public float commonAttack;
    // 근거리 공격력 기본 : 1타 25 2타 40 3타 60
    public float[] shortRangeAttack = new float[3];
    // 원거리 공격력 기본 : 1타 10 2타 40 3타 70 4타 110
    public float[] longRangeAttack = new float[4];
    // 공격 속도 기본 : 1
    public float attackSpeed;
    // 이동 속도 기본 : 100
    public float movementSpeed;
    // 크리티컬 확률 기본 : 10
    public float criticalChance;
    // 방어력 기본 : 0
    public float defense;
    // 장비 획득 확률 증가 기본 0 으로 하고 장비쪽 확률에 플러스
    public float equipmentDropUpgrade;
    // 생명력 흡수 기본 0
    public float drainLife;
    // 스테미나 최대치 기본 : 100
    public float maxStamina;
    // 스테미나 회복 기본 : 20
    public float regainStamina;
    // 던지기 공격당 마나 회복 기본 : 1타 3 2타 8 3타 13 4타 20
    public float[] regainMana = new float[4];
    // 마나 소모량 감소 기본 : 1타 30 2타 70 3타 100
    public float[] manaConsumption = new float[3];
    // 스테미나 소모량 감소 기본 : 0 모든 스테미나 소모에 적용
    public float consumesStamina;
    // 투척물 추가 획득 확률 증가 기본 : 0
    public float gainMoreThrowables;
    // 보유 투척물 제한 증가 / 기본 50
    public float maxThrowables;

    // 로비에서 조작되지 않는 플레이어 스탯

    // 받는 피해 감소 (확정 아님)
    // 최대 마나 기본 : 100
    public float maxMana;
    // 최대 점프 횟수 기본 : 2
    public float maxJumpCount;
    // 점프력 기본 : 100
    public float jumpPower;
    // 점프 소모 스테미나 기본 : 20
    public float jumpConsumesStamina;
    // 더블 점프 소모 스테미나 기본 : 10
    public float doubleJumpConsumesStamina;
    // 대쉬 거리 기본 : 200
    public float dashDistance;
    // 대쉬 소모 스테미나 기본 : 50
    public float dashConsumesStamina;
    // 근접 공격 스테미나 기본 : 1타 20 2타 30 3타 50
    public float[] shortRangeAttackStamina = new float[3];
    // 특수 공격력 수치 기본 : 1타 75 2타 150 3타 225
    public float[] specialAttack = new float[3];
    public void NewPlayerSetting()
    {
        maxHp = 60;
        commonAttack = 0;
        shortRangeAttack[0] = 25;
        shortRangeAttack[1] = 40;
        shortRangeAttack[2] = 60;
        longRangeAttack[0] = 10;
        longRangeAttack[1] = 40;
        longRangeAttack[2] = 70;
        longRangeAttack[3] = 110;
        attackSpeed = 1;
        movementSpeed = 100;
        criticalChance = 10;
        defense = 0;
        equipmentDropUpgrade = 0;
        drainLife = 0;
        maxStamina = 100;
        regainStamina = 20;
        regainMana[0] = 3;
        regainMana[1] = 8;
        regainMana[2] = 13;
        regainMana[3] = 20;
        manaConsumption[0] = 30;
        manaConsumption[1] = 70;
        manaConsumption[2] = 100;
        consumesStamina = 0;
        gainMoreThrowables = 0;
        maxThrowables = 50;
        maxMana = 100;
        maxJumpCount = 2;
        jumpPower = 100;
        jumpConsumesStamina = 20;
        doubleJumpConsumesStamina = 10;
        dashDistance = 200;
        dashConsumesStamina = 50;
        shortRangeAttackStamina[0] = 20;
        shortRangeAttackStamina[1] = 30;
        shortRangeAttackStamina[2] = 50;
        specialAttack[0] = 75;
        specialAttack[1] = 150;
        specialAttack[2] = 225;
    }
}
