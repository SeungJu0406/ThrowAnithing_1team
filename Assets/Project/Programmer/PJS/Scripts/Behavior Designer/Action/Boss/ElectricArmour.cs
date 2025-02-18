using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class ElectricArmour : BossAction
{
	[SerializeField] BossSkillState skillState;
	[SerializeField] GlobalState globalState;

    public override TaskStatus OnUpdate()
	{
        StartCoroutine(bossEnemy.CoolTimeRoutine(skillState.atkAble, skillState.coolTime));
        StartCoroutine(bossEnemy.CoolTimeRoutine(globalState.Able, globalState.coolTime.Value));
        
        return TaskStatus.Success;
	}
}