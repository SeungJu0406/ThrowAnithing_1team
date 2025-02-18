using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MKH
{
    public class UIManagerTest : MonoBehaviour
    {
        [SerializeField] GameObject _Inventory;
        [SerializeField] GameObject _EquipInventory;
        [SerializeField] GameObject _State;
        [SerializeField] GameObject _BlueChipPanel;
        InventorySlotTest[] ivSlots;
        InventorySlotTest[] eqSlots;
        InventoryMainTest _Main;
        [SerializeField] GameObject[] buttons;
        int selectedButtonsIndex;
        int buttonCount;

        [Header("아이템 설명")]
        [SerializeField] TMP_Text ivName;                       // 인벤토리 아이템 이름
        [SerializeField] TMP_Text ivDescription;                // 인벤토리 아이템 설명
        [SerializeField] TMP_Text eqName;                       // 장비 아이템 이름
        [SerializeField] TMP_Text eqDescription;                // 장비 아이템 설명

        private void Awake()
        {
            _Inventory = GameObject.Find("InventoryUI").transform.Find("Inventory").gameObject;
            _EquipInventory = GameObject.Find("InventoryUI").transform.Find("EquipmentInventory").gameObject;
            _State = GameObject.Find("InventoryUI").transform.Find("State").gameObject;
            _BlueChipPanel = GameObject.Find("InventoryUI").transform.Find("BlueChipPanel").gameObject;
            ivSlots = _Inventory.GetComponentsInChildren<InventorySlotTest>();
            eqSlots = _EquipInventory.GetComponentsInChildren<InventorySlotTest>();
            _Main = GetComponent<InventoryMainTest>();
        }

        private void Start()
        {
            _Inventory.SetActive(false);
            _EquipInventory.SetActive(false);
            _BlueChipPanel.SetActive(false);
            _State.SetActive(false);

            buttonCount = buttons.Length;
        }

        private void Update()
        {
            OpenBlueChip();
            Inventory();
            CloseBlueChip();
        }

        #region 인벤토리, 장비, 블루칩 열고 닫기
        private void Inventory()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (_Inventory.activeSelf)
                    return;

                Time.timeScale = 0;
                _Inventory.SetActive(true);
                _EquipInventory.SetActive(true);
                _State.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                if (_BlueChipPanel.activeSelf)
                    return;

                Time.timeScale = 1;
                _Inventory.SetActive(false);
                _EquipInventory.SetActive(false);
                _State.SetActive(false);
            }
        }

        private void OpenBlueChip()
        {
            if (!_Inventory.activeSelf)
                return;

            if (Input.GetKeyDown(KeyCode.B))
            {
                _BlueChipPanel.SetActive(true);
            }
        }

        private void CloseBlueChip()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                _BlueChipPanel.SetActive(false);
            }

        }
        #endregion

        public void Click()
        {
            //if (input.GetKeyDown(KeyCode.E))
            //{
            //    _Slot.UseItem();
            //    _Main.Sorting();
            //}
            //if (input.GetKeyDown(KeyCode.E))
            //{
            //for (int i = 0; i < _Slot.Length; i++)
            //{
            //    _Slot[i].UseItem();
            //    _Main.Sorting();
            //    return;
            //}
            // }
        }

        #region 아이템 버튼 조작
        private void Use(int index)
        {
            if (_Inventory.activeSelf && !_BlueChipPanel.activeSelf)
            {
                // 아이템 장착 - 인벤토리만 착용 가능
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // 인벤토리
                    if (index >= 9)
                    {
                        if (ivSlots[index - 9].Item != null)
                        {
                            ivSlots[index - 9].UseItem();
                            _Main.Sorting();
                            Debug.Log($"인벤토리 {index - 9}번 장비 장착");
                        }
                        else if (ivSlots[index - 9].Item == null)
                        {
                            Debug.Log("장착 할 장비가 없습니다.");
                            return;
                        }
                    }
                }

                // 아이템 삭제 - 인벤토리, 장비 둘 다 삭제 가능
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    // 장비
                    //if (index < 9)
                    //{
                    //    if (eqSlots[index].Item != null)
                    //    {
                    //        eqSlots[index].RemoveEquipmentSlot();
                    //        Debug.Log($"장비 {index}번 삭제");
                    //    }
                    //    else if(eqSlots[index].Item == null)
                    //    {
                    //        Debug.Log("분해 할 장비가 없습니다");
                    //        return;
                    //    }

                    //}
                    // 인벤토리
                    if (index >= 9)
                    {
                        if (ivSlots[index - 9].Item != null)
                        {
                            ivSlots[index - 9].ClearSlot();
                            _Main.Sorting();
                            Debug.Log($"인벤토리 {index - 9}번 장비 분해");
                        }
                        else if (ivSlots[index - 9].Item == null)
                        {
                            Debug.Log("분해 할 장비가 없습니다.");
                            return;
                        }
                    }
                }
            }
        }
        #endregion

        #region 아이템 정보
        private void Info()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (i == selectedButtonsIndex)
                {
                    // 장비창 설명
                    if (i < 9)
                    {
                        // 장비 슬롯에 아이템이 있는 상태
                        if (eqSlots[i].Item != null)
                        {
                            eqName.text = eqSlots[i].Item.Name;
                            eqDescription.text = eqSlots[i].Item.Description;
                            ivName.text = "-";
                            ivDescription.text = "";
                        }
                        // 장비 슬롯에 아이템이 없는 상태
                        else if (eqSlots[i].Item == null)
                        {
                            eqName.text = "-";
                            eqDescription.text = "";
                            ivName.text = "-";
                            ivDescription.text = "";
                        }
                    }
                    // 인벤토리 설명
                    else if (i >= 9)
                    {
                        // 인벤토리 슬롯에 아이템 있는 상태
                        if (ivSlots[i - 9].Item != null)
                        {
                            ivName.text = ivSlots[i - 9].Item.Name;
                            ivDescription.text = ivSlots[i - 9].Item.Description;

                            // 아이템 타입 별 인벤토리와 장비 비교
                            switch (ivSlots[i - 9].Item.Type)
                            {
                                case ItemType.Helmet:
                                    if (eqSlots[0].Item != null)
                                    {
                                        eqName.text = eqSlots[0].Item.Name;
                                        eqDescription.text = eqSlots[0].Item.Description;
                                    }
                                    else if (eqSlots[0].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Shirts:
                                    if (eqSlots[1].Item != null)
                                    {
                                        eqName.text = eqSlots[1].Item.Name;
                                        eqDescription.text = eqSlots[1].Item.Description;
                                    }
                                    else if (eqSlots[1].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Glasses:
                                    if (eqSlots[2].Item != null)
                                    {
                                        eqName.text = eqSlots[2].Item.Name;
                                        eqDescription.text = eqSlots[2].Item.Description;
                                    }
                                    else if (eqSlots[2].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Gloves:
                                    if (eqSlots[3].Item != null)
                                    {
                                        eqName.text = eqSlots[3].Item.Name;
                                        eqDescription.text = eqSlots[3].Item.Description;
                                    }
                                    else if (eqSlots[3].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Pants:
                                    if (eqSlots[4].Item != null)
                                    {
                                        eqName.text = eqSlots[4].Item.Name;
                                        eqDescription.text = eqSlots[4].Item.Description;
                                    }
                                    else if (eqSlots[4].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Earring:
                                    if (eqSlots[5].Item != null)
                                    {
                                        eqName.text = eqSlots[5].Item.Name;
                                        eqDescription.text = eqSlots[5].Item.Description;
                                    }
                                    else if (eqSlots[5].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Ring:
                                    if (eqSlots[6].Item != null)
                                    {
                                        eqName.text = eqSlots[6].Item.Name;
                                        eqDescription.text = eqSlots[6].Item.Description;
                                    }
                                    else if (eqSlots[6].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Shoes:
                                    if (eqSlots[7].Item != null)
                                    {
                                        eqName.text = eqSlots[7].Item.Name;
                                        eqDescription.text = eqSlots[7].Item.Description;
                                    }
                                    else if (eqSlots[7].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                                case ItemType.Necklace:
                                    if (eqSlots[8].Item != null)
                                    {
                                        eqName.text = eqSlots[8].Item.Name;
                                        eqDescription.text = eqSlots[8].Item.Description;
                                    }
                                    else if (eqSlots[8].Item == null)
                                    {
                                        eqName.text = "-";
                                        eqDescription.text = "";
                                    }
                                    break;
                            }
                        }
                        // 인벤토리 슬롯에 아이템이 없는 상태
                        else if (ivSlots[i - 9].Item == null)
                        {
                            ivName.text = "-";
                            ivDescription.text = "";
                            eqName.text = "-";
                            eqDescription.text = "";
                        }
                    }
                }
            }
        }
        #endregion
    }
}
