using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MKH
{
    public class InventorySlot : MonoBehaviour
    {
        // 아이템
        private Item mItem;
        public Item Item { get { return mItem; } set { mItem = value; } }

        [Header("해당 슬롯에 들어갈 수 있는 타입")]
        [SerializeField] private ItemType mSlotMask;

        [Header("슬롯에 있는 UI 오브젝트")]
        [SerializeField] private Image mItemImage;
        public Image ItemImage { get {  return mItemImage; } set { mItemImage = value; } }

        [Header("장비 교체 매니저")]
        [SerializeField] private EquipActionManager equipActionManager;

        public bool isEquip;

        public InventorySlotButton SlotButton;

        private void Awake()
        {
            //SlotButton = GetComponentInChildren<InventorySlotButton>();
        }
        // 아이템 이미지 투명도 조절
        private void SetColor(float _alpha)
        {
            Color color = mItemImage.color;
            color.a = _alpha;
            mItemImage.color = color;
        }

        // 아이템 들어갈 타입
        public bool IsMask(Item item)
        {
            return ((int)item.Type & (int)mSlotMask) == 0 ? false : true;
        }

        // 아이템 습득
        public void AddItem(Item item)
        {
            mItem = item;
            mItemImage.sprite = mItem.Image;
            SetColor(1);
        }

        // 아이템 제거
        public void ClearSlot()
        {
            mItem = null;
            mItemImage.sprite = null;
            SetColor(0);
        }

        // 아이템 사용
        public void UseItem()
        {
            if (mItem != null)
            {
                if(mItem.Type >= ItemType.Helmet && mItem.Type <= ItemType.Necklace)
                {
                    ChangeEquipmentSlot();
                }
            }
        }

        // 장비 교체
        public void ChangeEquipmentSlot()
        {
            Item item = mItem;
            ClearSlot();
            equipActionManager.UseEquip(item);
        }

        // 장비 제거
        public void RemoveEquipmentSlot()
        {
            equipActionManager.RemoveEquip(mItem);
        }

        
    }
}
