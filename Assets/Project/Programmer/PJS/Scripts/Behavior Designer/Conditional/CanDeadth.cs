using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanDeadth : Conditional
{
    [SerializeField] BaseEnemy enemy;

    public override void OnAwake()
    {
        enemy = GetComponent<BaseEnemy>();
    }

    public override TaskStatus OnUpdate()
    {        
        return enemy.CurHp <= 0 ? TaskStatus.Success : TaskStatus.Failure;
    }
}