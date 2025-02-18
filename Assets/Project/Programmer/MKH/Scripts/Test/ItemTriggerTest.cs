using System.Linq;
using UnityEngine;

namespace MKH
{
    public class ItemTriggerTest : MonoBehaviour
    {
        private ItemPickUp mCurrentItem;

        [SerializeField] GameObject particle;

        [SerializeField] private InventoryMainTest mInventory;      // 인벤토리

        private void OnTriggerEnter(Collider other)
        {
            // other의 오브젝트에서 ItemPickUp 불러오기
            mCurrentItem = other.transform.GetComponent<ItemPickUp>();

            // Item 태그 붙은 other 오브젝트
            if (other.gameObject.tag == Tag.Item)
            {
                for (int i = 0; i < mInventory.mSlots.Length; i++)
                {
                    // 아이템 타입이 None이 아닐 시, 슬롯이 다 안찼을 때
                    if (mCurrentItem.Item.Type != ItemType.None && mInventory.mSlots[i].Item == null)
                    {
                        // 인벤토리에 아이템 추가
                        mInventory.AcquireItem(mCurrentItem.Item);
                        Destroy(other.gameObject);
                        GameObject obj = Instantiate(particle, transform.position, Quaternion.Euler(-90f, 0, 0));
                        Destroy(obj,0.5f);
                        return;
                    }
                }
            }
        }
    }
}
