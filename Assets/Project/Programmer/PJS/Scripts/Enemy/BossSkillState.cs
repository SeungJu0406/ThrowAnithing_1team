using BehaviorDesigner.Runtime;

public struct BossSkillState
{
    public int damage;    // 공격력
    public float range;   // 사거리
    public float coolTime;    // 쿨타임
    public SharedBool atkAble;    // 공격가능 여부
}

public struct GlobalState
{
    public SharedBool Able;      // 사용 여부
    public SharedFloat coolTime; // 쿨타임
}
