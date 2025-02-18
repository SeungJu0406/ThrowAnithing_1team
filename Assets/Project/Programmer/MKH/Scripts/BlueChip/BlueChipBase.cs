using UnityEngine;

namespace MKH
{
    abstract public class BlueChipBase : MonoBehaviour
    {
        [SerializeField] protected GameObject mBlueChipBase;
        [SerializeField] protected GameObject mBlueChipSlotsParent;
        [SerializeField] public BlueChipSlot[] mSlots;

        protected void Awake()
        {
            if (mBlueChipBase.activeSelf)
            {
                mBlueChipBase.SetActive(false);
            }

            mSlots = mBlueChipSlotsParent.GetComponentsInChildren<BlueChipSlot>();
        }      
    }
}
