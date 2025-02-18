using UnityEngine;

namespace MKH
{
    public class BlueChipPanel : BlueChipBase
    {
        new private void Awake()
        {
            base.Awake();
            Setting();
        }

        public bool AcquireEffect(AdditionalEffect effect)
        {
            for (int i = 0; i < mSlots.Length; i++)
            {
                Debug.Log(i);
                if (mSlots[i].Effect == null)
                {
                    mSlots[i].AddEffect(effect);
                    Debug.Log(effect);
                    return true;
                }
            }
            return false;
        }

        public void Setting()
        {
            for (int i = 0; i < mSlots.Length; i++)
            {
                mSlots[i].SetSlot();
            }
        }
    }
}
