using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class JumpAttack : BossAction
{
    public SharedGameObject player; // 플레이어
    public float dis;  // 뛸 떄 자신의 위치와 플레이어의 위치 차이

    private Vector3 playerPos;   // 이동할 플레이어의 위치

    public override void OnStart()
    {
        playerPos = player.Value.transform.position;
        bossEnemy.SetPlayerPos(playerPos);
    }

    public override TaskStatus OnUpdate()
    {
        dis = (transform.position - playerPos).sqrMagnitude;

        return dis <= 0.1f ? TaskStatus.Success : TaskStatus.Running;
    }
}