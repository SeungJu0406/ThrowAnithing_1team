using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsStun : Conditional
{
    private BossEnemy enemy;

    public override void OnStart()
    {
        enemy = GetComponent<BossEnemy>();
    }

    public override void OnEnd()
    {
        enemy.breakShield = false;
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