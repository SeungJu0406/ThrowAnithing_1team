using UnityEngine;

[System.Serializable]
public class EnemyState
{
    [Range(50, 300)] public int MaxHp;  // 체력
    [Range(0, 20)] public int Atk;       // 공격력
    [Range(0, 10)] public float Def;    // 방어력
    [Range(0, 10)] public float Speed;    // 이동 속도
    [Range(0, 10)] public float AtkDelay;   // 공격 속도
    [Range(0, 10)] public float AttackDis;  // 공격 사거리
    [Range(0, 10)] public float TraceDis;   // 인식 사거리
}

[System.Serializable]
public class EliteEnemyState
{
    [Range(500, 1000)] public int MaxHp;  // 체력
    [Range(0, 20)] public int Atk;       // 공격력
    [Range(0, 20)] public float Def;    // 방어력
    [Range(0, 10)] public float Speed;    // 이동 속도
    [Range(0, 10)] public float AtkDelay;   // 공격 속도
    [Range(0, 10)] public float AttackDis;  // 공격 사거리
    [Range(0, 10)] public float TraceDis;   // 인식 사거리
}

[System.Serializable]
public class BossEnemyState
{
    [Range(1000, 5000)] public int MaxHp;  // 체력
    [Range(10, 20)] public int Atk;       // 공격력
    [Range(0, 10)] public float Def;    // 방어력
    [Range(0, 10)] public float Speed;    // 이동 속도
    [Range(0, 10)] public float AtkDelay;   // 공격 속도
    [Range(0, 10)] public float AttackDis;  // 공격 사거리
    [Range(0, 10)] public float TraceDis;   // 인식 사거리
}
