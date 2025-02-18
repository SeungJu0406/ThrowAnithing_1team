using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanMove : Conditional
{
    [SerializeField] SharedTransform playerPos;   // 플레이어
    [SerializeField] SharedFloat distance;    // 몬스터가 플레이어를 인식할 수 있는 최대 거리
    public SharedBool onTakeHit;    // 플레이어에게 맞았는가?

    private float playerDistance;   // 몬스터와 플레이어와의 거리

    public override TaskStatus OnUpdate()
    {
        // 몬스터와 플레이어와의 거리체크
        playerDistance = (playerPos.Value.position - transform.position).magnitude;
        
        if (playerDistance <= distance.Value)
        {
            onTakeHit.SetValue(false);
        }

        if (playerDistance <= distance.Value || onTakeHit.Value == true)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}
