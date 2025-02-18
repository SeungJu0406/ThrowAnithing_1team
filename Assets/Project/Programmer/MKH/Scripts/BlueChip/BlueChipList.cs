using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    [CreateAssetMenu]
    public class BlueChipList : ScriptableObject
    {
        public List<AdditionalEffect> blueChipList;

        public AdditionalEffect this[int index]
        {
            get
            {
                return blueChipList[index];
            }
        }

        public int Count => blueChipList.Count;
    }
}
