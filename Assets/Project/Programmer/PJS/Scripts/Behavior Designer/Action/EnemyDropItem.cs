using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;


public class EnemyDropItem : BaseAction
{
    const int maxPersent = 100;
    [SerializeField] SharedFloat reward;    // 드랍 확률
    private int randNum;

    public override void OnStart()
    {
        //몬스터가 죽었을 시 아이템 드랍
        randNum = Random.Range(0, maxPersent + 1);
    }

    public override TaskStatus OnUpdate()
    {
        // 확률 인스펙터에서 정해서 값 가져오기
        if (randNum <= reward.Value)
        {
            DataContainer.GetItemTablePrefab(transform.position, baseEnemy.curMonsterType);
        }

        return TaskStatus.Success;
    }
}