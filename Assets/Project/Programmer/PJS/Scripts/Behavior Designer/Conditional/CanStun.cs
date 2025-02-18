using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CanStun : Conditional
{
	public SharedBool stun;

    public override void OnEnd()
    {
        stun.SetValue(false);
    }
    public override TaskStatus OnUpdate()
	{
		return stun.Value ? TaskStatus.Success : TaskStatus.Failure;
	}
}