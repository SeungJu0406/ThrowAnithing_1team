using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace MKH
{
    public class Drop : MonoBehaviour
    {
        public DropList dropList;

        private void DropItem()
        {
            GameObject dropPrefab = dropList.itemList[Random.Range(0, dropList.Count)];
            Instantiate(dropPrefab, transform.position, Quaternion.identity);

        }
    }
}
