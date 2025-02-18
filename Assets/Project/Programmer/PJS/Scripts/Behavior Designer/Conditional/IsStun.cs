using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsStun : Conditional
{
    private BossEnemy enemy;

    public override void OnAwake()
    {
        enemy = GetComponent<BossEnemy>();
    }

    public override TaskStatus OnUpdate()
    {
        if (enemy.breakShield == true)
        {
            enemy.RecoveryStopCotoutine();
        }

        return enemy.breakShield ? TaskStatus.Success : TaskStatus.Failure;
    }
}