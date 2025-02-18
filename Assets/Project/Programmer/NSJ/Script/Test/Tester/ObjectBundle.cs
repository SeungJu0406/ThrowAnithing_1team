using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NSJ_TesterPanel
{
    public class ObjectBundle : TesterBundle
    {
        private ThrowObject _curObject;
        private TMP_Text _nameText => GetUI<TMP_Text>("NameText");
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
            if (index >= DataContainer.ThrowObjects.Length)
            {
                _blueChipIndex = 0;
                return;
            }
            else if (index < 0)
            {
                _blueChipIndex = DataContainer.ThrowObjects.Length - 1;
                return;
            }

            _curObject = DataContainer.ThrowObjects[index];
            _nameText.SetText(DataContainer.ThrowObjects[index].Data.Name);
        }

        private void Create()
        {

            ThrowObject blueChipItem = Instantiate(_curObject);
            Vector3 createPos = new Vector3(
                _player.transform.position.x + (_player.transform.forward.x * Random.Range(createOffset.x - randomOffset.x, createOffset.x + randomOffset.x)),
                _player.transform.position.y + createOffset.y,
                _player.transform.position.z + (_player.transform.forward.z * Random.Range(createOffset.z - randomOffset.z, createOffset.z + randomOffset.z)));
            blueChipItem.transform.position = createPos;
        }

        private void SubscribeEvent()
        {
            _blueChipIndexSubject
                .DistinctUntilChanged()
                .Subscribe(x => ChangeIndex(x));

            GetUI<Button>("LeftButton").onClick.AddListener(() => _blueChipIndex--);
            GetUI<Button>("RightButton").onClick.AddListener(() => _blueChipIndex++);
            GetUI<Button>("CreateButton").onClick.AddListener(Create);
        }

        private void InitButtons()
        {
            _buttons.Add(GetButtonStruct("LeftButton"));
            _buttons.Add(GetButtonStruct("RightButton"));
            _buttons.Add(GetButtonStruct("CreateButton"));
        }
    }
}