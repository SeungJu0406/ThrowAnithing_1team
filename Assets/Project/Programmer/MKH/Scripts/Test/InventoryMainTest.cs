using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MKH
{
    public class InventoryMainTest : InventoryBaseTest
    {
        [SerializeField] GameObject state;

        [SerializeField] Button button;

        new private void Awake()
        {
            base.Awake();
            state.SetActive(false);
        }

        public void AcquireItem(Item item)
        {
            Item _item = item.Create();

            for (int i = 0; i < mSlots.Length; i++)
            {
                //마스크를 사용하여 해당 슬롯이 마스크에 허용되는 위치인경우에만 아이템을 집어넣도록 한다.
                if (mSlots[i].Item == null && mSlots[i].IsMask(_item))
                {
                    mSlots[i].AddItem(_item);
                    return;
                }
            }
        }
        public InventorySlotTest IsCanAquireItem(Item item)
        {
            foreach (InventorySlotTest slot in base.mSlots)
            {
                //비어있는 슬롯을 발견한경우
                if (slot.Item == null) { return slot; }

            }

            return null;
        }

        public void Sorting()
        {
            for (int i = 0; i < mSlots.Length - 1; i++)
            {
                if (mSlots[i].Item == null)
                {
                    Color color1 = mSlots[i + 1].ItemImage.color;
                    color1.a = 0f;

                    mSlots[i].ItemImage.sprite = mSlots[i + 1].ItemImage.sprite;
                    mSlots[i].ItemImage.color = mSlots[i + 1].ItemImage.color;
                    mSlots[i].Item = mSlots[i + 1].Item;
                    mSlots[i + 1].ItemImage.sprite = null;
                    mSlots[i + 1].ItemImage.color = color1;
                    mSlots[i + 1].Item = null;

                    if(mSlots[i].ItemImage.sprite == null)
                    {
                        Color color = mSlots[i].ItemImage.color;
                        color.a = 0f;
                        mSlots[i].ItemImage.color = color;
                    }
                }
            }
        }
    }
}


