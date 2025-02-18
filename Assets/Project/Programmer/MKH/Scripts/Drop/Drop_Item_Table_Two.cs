using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    [CreateAssetMenu(fileName = "Item_Monster_", menuName = "DropItemTable/ItemList_Two")]
    public class Drop_Item_Table_Two : DropItemTable
    {
        int max;
        [Header("랜덤 아이템 나올 확률(합쳐서 100% 만들기)")]
        [SerializeField] int firstPercent;
        [SerializeField] int secondPercent;

        public override GameObject DropListTable1(Vector3 pos, Quaternion rot)
        {
            max = firstPercent + secondPercent;

            int randNum = Random.Range(0, max + 1);
            //Debug.Log(randNum);
            if (randNum <= firstPercent)
            {
                GameObject dropPrefab = dropLists[0].itemList[Random.Range(0, dropLists[0].itemList.Count)];
                GameObject _item = Instantiate(dropPrefab, pos, rot);
                return _item;
            }
            else if (firstPercent < randNum && randNum <= max)
            {
                GameObject dropPrefab = dropLists[1].itemList[Random.Range(0, dropLists[1].itemList.Count)];
                GameObject _item = Instantiate(dropPrefab, pos, rot);
                return _item;
            }
            return null;
        }
    }
}
