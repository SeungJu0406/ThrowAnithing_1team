using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawnTrigger : MonoBehaviour
{

    [SerializeField] GameObject triggerObject;

    [SerializeField] GameObject monsterPrefabs;

    //트리거 프리팹 배치 후 자기자신을 인스펙터창에 매핑
    // 트리거에 플레이어가 닿으면 그때 몬스터 생성


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        {
            monsterPrefabs.SetActive(true);
        }

    }


}
