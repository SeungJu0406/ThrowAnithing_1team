using UnityEngine;
using Zenject;

namespace MKH
{
    public class ItemTrigger : MonoBehaviour
    {
        [Inject]
        private PlayerData playerData;

        private ItemPickUp mCurrentItem;

        [SerializeField] private InventoryMain mInventory;      // 인벤토리

        [SerializeField] GameObject effect;
        [SerializeField] AudioClip clip;

        private void Awake()
        {
            mInventory = playerData.Inventory.InventoryMain;
        }

        private void OnTriggerEnter(Collider other)
        {
            // other의 오브젝트에서 ItemPickUp 불러오기
            mCurrentItem = other.transform.GetComponent<ItemPickUp>();

            // Item 태그 붙은 other 오브젝트
            if (other.gameObject.tag == Tag.Item)
            {
                for (int i = 0; i < mInventory.mSlots.Length; i++)
                {
                    // 아이템 타입이 None이 아니고 인벤토리 슬롯이 null 일 때
                    if (mCurrentItem.Item.Type != ItemType.None && mInventory.mSlots[i].Item == null)
                    {
                        // 인벤토리에 아이템 추가
                        ObjectPool.Get(effect, transform.position + new Vector3(0, 1, 0), Quaternion.identity, 0.3f);
                        SoundManager.PlaySFX(clip);
                        mInventory.AcquireItem(mCurrentItem.Item);
                        Destroy(other.gameObject);
                        return;
                    }
                }
            }
        }
    }
}
