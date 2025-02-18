using MKH;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NSJ_TesterPanel
{
    public class EquipmentBundle : TesterBundle
    {
        [SerializeField] string[] _types = new string[9];

        [SerializeField] NSJBlueChip _equipmentItem;
        private TMP_Text _nameText => GetUI<TMP_Text>("NameText");

        private int m_equipmentIndex;
        private int _equipmentIndex
        {
            get { return m_equipmentIndex; }
            set
            {
                m_equipmentIndex = value;
                _equipmentIndexSubject.OnNext(m_equipmentIndex);
            }
        }
        private Subject<int> _equipmentIndexSubject = new Subject<int>();

        private GameObject[] _stepButtons = new GameObject[3];
        private int _itemStep;

        protected override void Awake()
        {
            base.Awake();
            InitButtons();
        }

        private void Start()
        {
            SubscribeEvent();
            ChangeIndex(_equipmentIndex);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            ChangeItemStep(0);
        }

        private void ChangeIndex(int index)
        {
            if (index >= _types.Length)
            {
                _equipmentIndex = 0;
                return;
            }
            else if (index < 0)
            {
                _equipmentIndex = _types.Length - 1;
                return;
            }
            _nameText.SetText(_types[index]);
        }
        private void ChangeItemStep(int step)
        {
            step--;
            _itemStep = step;

            if (_itemStep == -1)
            {
                foreach(GameObject button in _stepButtons)
                {
                    button.SetActive(false);
                }
                return;
            }

            for (int i = 0; i < _stepButtons.Length; i++) 
            {
                if(_itemStep == i)
                {
                    _stepButtons[i].SetActive(true);
                }
                else
                {
                    _stepButtons[i].SetActive(false);
                }
            }
        }

        private void Create()
        {
            GameObject instance = null;

            switch (_itemStep)
            {
                case 0:
                    instance = Instantiate(DataContainer.GetItemPrefab(DataContainer.Items.NormalItems));
                    break;
                case 1:
                    instance = Instantiate(DataContainer.GetItemPrefab(DataContainer.Items.MagicItems));
                    break;
                case 2:
                    instance = Instantiate(DataContainer.GetItemPrefab(DataContainer.Items.RareItems));
                    break;
                default:
                    return;
            }

            Vector3 createPos = new Vector3(
                _player.transform.position.x + (_player.transform.forward.x * Random.Range(createOffset.x - randomOffset.x, createOffset.x + randomOffset.x)),
                _player.transform.position.y + createOffset.y,
                _player.transform.position.z + (_player.transform.forward.z * Random.Range(createOffset.z - randomOffset.z, createOffset.z + randomOffset.z)));
            instance.transform.position = createPos;
        }

        private void SubscribeEvent()
        {
            _equipmentIndexSubject
                .DistinctUntilChanged()
                .Subscribe(x => ChangeIndex(x));

            GetUI<Button>("1StepButton").onClick.AddListener(() => ChangeItemStep(1));
            GetUI<Button>("2StepButton").onClick.AddListener(() => ChangeItemStep(2));
            GetUI<Button>("3StepButton").onClick.AddListener(() => ChangeItemStep(3));
            GetUI<Button>("LeftButton").onClick.AddListener(() => _equipmentIndex--);
            GetUI<Button>("RightButton").onClick.AddListener(() => _equipmentIndex++);
            GetUI<Button>("CreateButton").onClick.AddListener(Create);
        }

        private void InitButtons()
        {
            _buttons.Add(GetButtonStruct("1StepButton"));
            _buttons.Add(GetButtonStruct("2StepButton"));
            _buttons.Add(GetButtonStruct("3StepButton"));
            _buttons.Add(GetButtonStruct("LeftButton"));
            _buttons.Add(GetButtonStruct("RightButton"));
            _buttons.Add(GetButtonStruct("CreateButton"));


            _stepButtons[0] = GetUI("1StepButtonOn");
            _stepButtons[1] = GetUI("2StepButtonOn");
            _stepButtons[2] = GetUI("3StepButtonOn");
        }
    }
}