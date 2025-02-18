using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using BehaviorDesigner.Runtime;

public class LightningNova : BossAction
{
    [SerializeField] BossSkillState skillState;
    [SerializeField] GlobalState globalState;

    public override TaskStatus OnUpdate()
	{
        bossEnemy.TakeChargeBoom(skillState.range, skillState.damage);

		StartCoroutine(bossEnemy.CoolTimeRoutine(skillState.atkAble, skillState.coolTime));
        StartCoroutine(bossEnemy.CoolTimeRoutine(globalState.Able, globalState.coolTime.Value));
        
        return TaskStatus.Success;
	}
}