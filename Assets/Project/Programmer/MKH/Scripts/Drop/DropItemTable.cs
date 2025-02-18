using BehaviorDesigner.Runtime.Tasks.Unity.UnityTransform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    public class DropItemTable : ScriptableObject
    {
        [Header("DropList 스크립터블오브젝트")]
        [SerializeField] public DropList[] dropLists;

        public virtual GameObject DropListTable1(Vector3 pos, Quaternion rot)
        {
            return null;
        }

        public virtual GameObject DropListTable2(Vector3 pos, Quaternion rot)
        {
            return null;
        }
    }
}
