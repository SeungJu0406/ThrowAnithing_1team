using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKH
{
    abstract public class InventoryBase : MonoBehaviour
    {
        [SerializeField] protected GameObject mInventoryBase;
        [SerializeField] protected GameObject mInventorySlotsParent;
        [SerializeField] public InventorySlot[] mSlots;

        protected void Awake()
        {
            //if (mInventoryBase.activeSelf)
            //{
            //    mInventoryBase.SetActive(false);
            //}

             mSlots = mInventorySlotsParent.GetComponentsInChildren<InventorySlot>();
        }
    }
}

