using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanMove : Conditional
{
    [SerializeField] SharedTransform playerPos;   // �÷��̾�
    [SerializeField] SharedFloat distance;    // ���Ͱ� �÷��̾ �ν��� �� �ִ� �ִ� �Ÿ�

    private float playerDistance;   // ���Ϳ� �÷��̾���� �Ÿ�

    public override TaskStatus OnUpdate()
    {
        // ���Ϳ� �÷��̾���� �Ÿ�üũ
        playerDistance = (playerPos.Value.position - transform.position).magnitude;
        
        if (playerDistance < distance.Value)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}