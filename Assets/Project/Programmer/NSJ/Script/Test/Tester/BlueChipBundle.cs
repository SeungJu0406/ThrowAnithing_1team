using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NSJ_TesterPanel
{
    public class BlueChipBundle : TesterBundle
    {
        [SerializeField] NSJBlueChip _blueChipItem;
        private TMP_Text _nameText => GetUI<TMP_Text>("NameText");
        private TMP_Text _descriptionText => GetUI<TMP_Text>("DescriptionText");

        private AdditionalEffect _curBlueChip;
        private int m_blueChipIndex;
        private int _blueChipIndex
        {
            get { return m_blueChipIndex; }
            set
            {
                m_blueChipIndex = value;
                _blueChipIndexSubject.OnNext(m_blueChipIndex);
            }
        }
        private Subject<int> _blueChipIndexSubject = new Subject<int>();

        protected override void Awake()
        {
            base.Awake();
            InitButtons();
        }

        private void Start()
        {
            SubscribeEvent();
            ChangeIndex(_blueChipIndex);
        }
        private void ChangeIndex(int index)
        {
            if (index >= DataContainer.BlueChips.Length)
            {
                _blueChipIndex = 0;
                return;
            }
            else if (index < 0)
            {
                _blueChipIndex = DataContainer.BlueChips.Length - 1;
                return;
            }

            _curBlueChip = DataContainer.BlueChips[index];
            _nameText.SetText(_curBlueChip.Name);
            _descriptionText.SetText(_curBlueChip.Description);
        }

        private void Create()
        {
            NSJBlueChip blueChipItem = Instantiate(_blueChipItem);
            Vector3 createPos = new Vector3(
                _player.transform.position.x + (_player.transform.forward.x * Random.Range(createOffset.x - randomOffset.x, createOffset.x + randomOffset.x)),
                _player.transform.position.y + createOffset.y,
                _player.transform.position.z + (_player.transform.forward.z * Random.Range(createOffset.z - randomOffset.z, createOffset.z + randomOffset.z)));
            blueChipItem.transform.position = createPos;
            blueChipItem.SetBlueChip(_curBlueChip);
        }

        private void ClearBlueChip()
        {
            // 플레이어 싹다 삭제
            _player.GetComponent<PlayerController>().ClearAdditional();
            // 블루칩 선택 UI 싺다 삭제
            Inventory.Instance.BlueChipChoiceController.ClearBlueChipForTester();
        }

        private void SubscribeEvent()
        {
            _blueChipIndexSubject
                .DistinctUntilChanged()
                .Subscribe(x => ChangeIndex(x));

            GetUI<Button>("LeftButton").onClick.AddListener(() => _blueChipIndex--);
            GetUI<Button>("RightButton").onClick.AddListener(() => _blueChipIndex++);
            GetUI<Button>("CreateButton").onClick.AddListener(Create);
            GetUI<Button>("RemoveButton").onClick.AddListener(ClearBlueChip);
        }

        private void InitButtons()
        {
            _buttons.Add(GetButtonStruct("LeftButton"));
            _buttons.Add(GetButtonStruct("RightButton"));
            _buttons.Add(GetButtonStruct("CreateButton"));
            _buttons.Add(GetButtonStruct("RemoveButton"));
        }
    }
}