using UnityEngine;
using UnityEngine.EventSystems;

namespace MKH
{
    public class UIManager : BaseBinder
    {
        [Header("게임 오브젝트")]
        [SerializeField] GameObject _Inventory;                 // 인벤토리
        [SerializeField] GameObject _EquipInventory;            // 장비
        [SerializeField] GameObject _State;                     // 능력치
        [SerializeField] GameObject _BlueChipPanel;             // 블루칩 패널
        [SerializeField] GameObject _BlueChipChoice;            // 블루칩 먹기 전 UI
        [SerializeField] GameObject _BlueChipChoicePanel;       // 블루칩 선택 패널

        [Header("효과음")]
        [SerializeField] AudioClip open;                        // 열기 효과음
        [SerializeField] AudioClip close;                       // 닫기 효과음

        private PlayerController _player;
        PlayerController player
        {
            get
            {
                if (_player == null)
                {
                    _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                }
                return _player;
            }
            set { _player = value; }
        }

        private void Awake()
        {
            Bind();
            _Inventory = GetObject("Inventory");
            _EquipInventory = GetObject("EquipmentInventory");
            _State = GetObject("State");
            _BlueChipPanel = GetObject("BlueChipPanel");
            _BlueChipChoice = GetObject("BlueChipChoicePopUp");
            _BlueChipChoicePanel = GetObject("BlueChipChoicePanel");
        }

        private void Start()
        {
            _Inventory.SetActive(false);
            _EquipInventory.SetActive(false);
            _State.SetActive(false);
            _BlueChipChoice.SetActive(false);
            _BlueChipPanel.SetActive(false);
            _BlueChipChoicePanel.SetActive(false);
        }

        private void Update()
        {
            OpenBlueChip();
            Inventory();
            CloseBlueChip();
        }

        #region 인벤토리
        private void Inventory()
        {
            // 열기
            if (InputKey.GetButtonDown(InputKey.InvenOpen))
            {
                if (InputKey.GetActionMap() == InputType.UI)
                    return;

                SoundManager.PlaySFX(open);
                _Inventory.SetActive(true);
                _EquipInventory.SetActive(true);
                _State.SetActive(true);

                InputKey.SetActionMap(InputType.UI);
            }

            // 닫기
            if (InputKey.GetButtonDown(InputKey.InvenClose))
            {
                if (_BlueChipPanel.activeSelf)
                    return;
                if (_Inventory.activeSelf == false)
                    return;

                _Inventory.SetActive(false);
                _EquipInventory.SetActive(false);
                _State.SetActive(false);
                InputKey.SetActionMap(InputType.GAMEPLAY);
                SoundManager.PlaySFX(close);
            }
        }
        #endregion

        #region 블루칩
        // 열기
        private void OpenBlueChip()
        {
            if (!_Inventory.activeSelf)
                return;

            if (InputKey.GetButtonDown(InputKey.InvenOpen))
            {
                SoundManager.PlaySFX(open);
                _BlueChipPanel.SetActive(true);
                EventSystem.current.currentSelectedGameObject.SetActive(false);
            }
        }

        // 닫기
        private void CloseBlueChip()
        {
            if (!_Inventory.activeSelf)
                return;

            if (InputKey.GetButtonDown(InputKey.InvenClose))
            {
                _BlueChipPanel.SetActive(false);
                SoundManager.PlaySFX(close);
                EventSystem.current.currentSelectedGameObject.SetActive(true);
            }
        }
        #endregion
    }
}
