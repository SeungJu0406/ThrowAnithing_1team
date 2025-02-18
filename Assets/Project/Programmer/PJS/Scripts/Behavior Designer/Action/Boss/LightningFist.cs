using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class LightningFist : BossAction
{
    [SerializeField] BossSkillState skillState;
    [SerializeField] GlobalState globalState;
    [SerializeField] Transform createPos;

    private RaycastHit[] hits;

    public override TaskStatus OnUpdate()
    {
        hits = Physics.BoxCastAll(createPos.position, transform.lossyScale / 2f, transform.forward, transform.rotation, skillState.range);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.name.CompareTo("Boss") == 0 || hit.collider.tag != Tag.Player)
                continue;

            //enemy.Battle.TargetAttack(hits[i].transform, skillState.damage);
            bossEnemy.Battle.TargetAttackWithDebuff(hit.transform, skillState.damage);
            bossEnemy.Battle.TargetCrowdControl(hit.transform, CrowdControlType.Stiff);
        }

        StartCoroutine(bossEnemy.CoolTimeRoutine(skillState.atkAble, skillState.coolTime));
        StartCoroutine(bossEnemy.CoolTimeRoutine(globalState.Able, globalState.coolTime.Value));

        return TaskStatus.Success;
    }
}