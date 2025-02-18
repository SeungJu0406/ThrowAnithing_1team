
using UnityEngine;

namespace MKH
{
    public class EquipActionManager : MonoBehaviour
    {
        [Header("장비 인벤토리")]
        [SerializeField] private EquipmentInventory mEquipmentInventory;
        [SerializeField] private InventoryMain minventoryMain;

        // 장비 교체
        public bool UseEquip(Item item)
        {
            InventorySlot equipmentSlot = mEquipmentInventory.GetEquipmentSlot(item.Type);
            InventorySlot inventorySlot = minventoryMain.IsCanAquireItem(item);

            Item tempItem = equipmentSlot.Item;

            equipmentSlot.AddItem(item);

            if (tempItem != null)
            {
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
                InventorySlot equipmentSlot = mEquipmentInventory.GetEquipmentSlot(item.Type);

                equipmentSlot.ClearSlot();
            }

            mEquipmentInventory.CalculateEffect();
            return true;
        }
    }
}
