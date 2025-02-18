using UnityEngine;

namespace MKH
{
    public class EquipActionManagerTest : MonoBehaviour
    {
        [Header("장비 인벤토리")]
        [SerializeField] private EquipmentInventoryTest mEquipmentInventory;
        [SerializeField] private InventoryMainTest mInventory;

        // 장비 교체
        public bool UseEquip(Item item)
        {
            InventorySlotTest equipmentSlot = mEquipmentInventory.GetEquipmentSlot(item.Type);
            InventorySlotTest inventorySlot = mInventory.IsCanAquireItem(item);

            Item tempItem = equipmentSlot.Item;

            equipmentSlot.AddItem(item);

            if (tempItem != null)
            {
                Item _itme = tempItem;
                equipmentSlot.ClearSlot();
                equipmentSlot.AddItem(item);
                inventorySlot.AddItem(tempItem);
            }

            mEquipmentInventory.CalculateEffect();
            return true;
        }

        // 장비 삭제
        public bool RemoveEquip(Item item)
        {
            if (item != null)
            {
                InventorySlotTest equipmentSlot = mEquipmentInventory.GetEquipmentSlot(item.Type);

                equipmentSlot.ClearSlot();
            }

            mEquipmentInventory.CalculateEffect();
            return true;
        }
    }
}
