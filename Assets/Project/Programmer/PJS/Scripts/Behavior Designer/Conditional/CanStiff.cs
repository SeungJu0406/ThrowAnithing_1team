using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CanStiff : Conditional
{
	[SerializeField] SharedBool Stiff;
    public SharedBool hitAble;

    private bool able;

    public override void OnEnd()
    {
        Stiff.Value = false;
    }

    public override TaskStatus OnUpdate()
	{
        able = Stiff.Value == true && hitAble.Value == true;

		return able ? TaskStatus.Success : TaskStatus.Failure;
	}
}