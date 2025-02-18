using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NSJ_TesterPanel
{
    public class ButtonBundle : BaseUI
    {
        enum Bundle { Monster, BlueChip, EquipMent, Object, Size }
        struct ButtonStruct
        {
            [HideInInspector] public GameObject Bundle;
            [HideInInspector] public GameObject ButtonOn;
        }
        [SerializeField] GameObject _monster;
        [SerializeField] GameObject _blueChip;
        [SerializeField] GameObject _equipment;
        [SerializeField] GameObject _object;

        private Button _monsterButton => GetUI<Button>("MonsterButton");
        private Button _blueChipButton => GetUI<Button>("BlueChipButton");
        private Button _EquipButton => GetUI<Button>("EquipmentButton");
        private Button _objectButton => GetUI<Button>("ObjectButton");


        private ButtonStruct[] _bundles = new ButtonStruct[(int)Bundle.Size];
        private Bundle _curBundle;
        private bool _canControl;

        private void Awake()
        {
            Bind();
            Init();
        }
        private void Start()
        {
            SubscribesEvent();
            ChangeBundle(Bundle.Monster);

        }
        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(GetUI<Button>("MonsterButton").gameObject);
        }

        private void OnDisable()
        {

        }

        private void Update()
        {
            CheckInput();
        }

        private void ChangeBundle(Bundle bundle)
        {
            _curBundle = bundle;
            for (int i = 0; i < _bundles.Length; i++)
            {
                if (i == (int)bundle)
                {
                    _bundles[i].Bundle.SetActive(true);
                    _bundles[i].ButtonOn.SetActive(true);
                }
                else
                {
                    _bundles[i].Bundle.SetActive(false);
                    _bundles[i].ButtonOn.SetActive(false);
                }
            }
        }

        private void Init()
        {
            _bundles[(int)Bundle.Monster] = GetStruct(_monster, GetUI("MonsterButtonOn"));
            _bundles[(int)Bundle.BlueChip] = GetStruct(_blueChip, GetUI("BlueChipButtonOn"));
            _bundles[(int)Bundle.EquipMent] = GetStruct(_equipment, GetUI("EquipmentButtonOn"));
            _bundles[(int)Bundle.Object] = GetStruct(_object, GetUI("ObjectButtonOn"));
        }
        private void SubscribesEvent()
        {
            _monsterButton.onClick.AddListener(() => ChangeBundle(Bundle.Monster));
            _blueChipButton.onClick.AddListener(() => ChangeBundle(Bundle.BlueChip));
            _EquipButton.onClick.AddListener(() => ChangeBundle(Bundle.EquipMent));
            _objectButton.onClick.AddListener(() => ChangeBundle(Bundle.Object));
        }

        private ButtonStruct GetStruct(GameObject buddle, GameObject buttonOn)
        {
            ButtonStruct instance = new ButtonStruct();
            instance.Bundle = buddle;
            instance.ButtonOn = buttonOn;
            return instance;
        }

        private void CheckInput()
        {
            GameObject curSelect = EventSystem.current.currentSelectedGameObject;
            if (curSelect == _monsterButton.gameObject)
            {
                ChangeBundle(Bundle.Monster);
            }
            else if(curSelect == _blueChipButton.gameObject)
            {
                ChangeBundle(Bundle.BlueChip);
            }
            else if (curSelect == _EquipButton.gameObject)
            {
                ChangeBundle(Bundle.EquipMent);
            }
            else if (curSelect == _objectButton.gameObject)
            {
                ChangeBundle(Bundle.Object);
            }
        }
    }
}