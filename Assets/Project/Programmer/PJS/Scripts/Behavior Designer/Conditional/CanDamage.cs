using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CanDamage : Conditional
{
	[SerializeField] BaseEnemy enemy;

	private bool checkAble;

    public override void OnStart()
    {
        enemy = GetComponent<BaseEnemy>();
    }

    public override void OnEnd()
    {
        checkAble = false;
    }

    public override TaskStatus OnUpdate()
	{
		checkAble = enemy.resultDamage > 0 ? true : false;

        return checkAble ? TaskStatus.Success : TaskStatus.Failure;
	}
}