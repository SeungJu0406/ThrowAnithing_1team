using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class SetStun : BossAction
{
	public SharedBool able;
	public bool onStun;
	 
	public override void OnStart()
	{
		able.SetValue(true);
	}

	public override TaskStatus OnUpdate()
	{
        bossEnemy.breakShield = onStun;
		return TaskStatus.Success;
	}
}