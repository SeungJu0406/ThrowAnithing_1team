using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using static BossEnemy;

public class CanPhase : Conditional
{
	[SerializeField] PhaseType phase;

	private BossEnemy enemy;

    public override void OnStart()
    {
        enemy = GetComponent<BossEnemy>();
    }

    public override TaskStatus OnUpdate()
	{
		return enemy.curPhase == phase ? TaskStatus.Success : TaskStatus.Failure;
	}
}