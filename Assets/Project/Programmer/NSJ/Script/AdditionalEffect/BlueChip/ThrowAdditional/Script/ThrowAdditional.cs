using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAdditional : AdditionalEffect
{
    protected PlayerController Player;
    protected PlayerModel Model => Player.Model;
    protected BattleSystem Battle => Player.Battle;
    protected ThrowObject _throwObject;
    [System.Serializable]
    protected struct TargetInfo
    {
        public Transform transform;
        public float Distance;
    }

    public void Init(PlayerController player, AdditionalEffect addtional,ThrowObject throwObject)
    {
        Origin = addtional.Origin;
        Player = player;
        _throwObject = throwObject;
    }

    protected TargetInfo FindNearTarget()
    {
        TargetInfo minNearTarget = SetTargetInfo(null, 10000f);
        int nearTarhetCount = Physics.OverlapSphereNonAlloc(_throwObject.transform.position, 0.5f, Player.OverLapColliders, 1 << Layer.Monster);
        if (nearTarhetCount > 0)
        {      
            // 주변에 몬스터가 있는지 스캔
            for (int i = 0; i < nearTarhetCount; i++)
            {
                Transform targetTransform = Player.OverLapColliders[i].transform;
                // 조건에 부합하면 해당 타겟을 거리와 함께 저장
                TargetInfo targetInfo = SetTargetInfo(targetTransform, Vector3.Distance(_throwObject.transform.position, targetTransform.position));

                // 거리가 더 가깝다면 교체
                if (targetInfo.Distance < minNearTarget.Distance)
                {
                    minNearTarget = targetInfo;
                }
            }
        }
        return minNearTarget;
    }
    //TargetInfo 설정
    protected TargetInfo SetTargetInfo(Transform target, float distance)
    {
        TargetInfo info = new TargetInfo();
        info.transform = target;
        info.Distance = distance;
        return info;
    }
}
