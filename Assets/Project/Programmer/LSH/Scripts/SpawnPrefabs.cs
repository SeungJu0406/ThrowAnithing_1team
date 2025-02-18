using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnPrefabs : MonoBehaviour
{
    [SerializeField] GameObject prefabs; //스폰될 프리팹
    [SerializeField] List<Transform> spawnPos; //스폰 지점들
    [SerializeField] int spwanCount; //지점들 중에서 몇개를 스폰시킬지

    [SerializeField] int trashCount; // 쓰레기 몇개 스폰?
    ThrowObject[] childCube; //prefabs가 Tag.Trash일때의 자식들
    [SerializeField] bool isAlreadySpawn;

    private void Start()
    {
        SpawnPrefabPos();
    }


    public void SpawnPrefabPos()
    {

        if (spawnPos.Count < spwanCount)
        {
            return;
        }
        else
        {

            while (spwanCount != 0)
            {

                int num = Random.Range(0, spawnPos.Count);
                GameObject instance = Instantiate(prefabs, spawnPos[num]);

                if (prefabs.tag == Tag.Trash)
                {
                    SpawnTrashPos(instance);
                }

                spawnPos.Remove(spawnPos[num]);
                spwanCount -= 1;

            }
        }


    }


    public void SpawnTrashPos(GameObject instance)
    {

        //for (int i = 0; i < childCube.Length; i++)
        //{
        //    if (childCube[i] == prefabs.transform)
        //        childCube[i].gameObject.SetActive(true);
        //    else
        //        childCube[i].gameObject.SetActive(Random.value > 0.5f);

        //}

        childCube = instance.GetComponentsInChildren<ThrowObject>(true);

        if (trashCount >= childCube.Length)
        {
            trashCount = childCube.Length;
        }

        foreach (ThrowObject child in childCube) 
        {
            child.gameObject.SetActive(false);
        }


        for (int i = 0; i < trashCount; i++)
        {
            while (true)
            {
                int randomNum = Random.Range(0, childCube.Length);

                if (childCube[randomNum].gameObject.activeSelf == true) 
                {
                    continue;
                }
                else
                {
                    childCube[randomNum].gameObject.SetActive (true);
                    break;
                }
            }


        }


    }

}




