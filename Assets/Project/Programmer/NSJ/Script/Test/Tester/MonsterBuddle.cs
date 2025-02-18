using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NSJ_TesterPanel
{
    public class MonsterBuddle : TesterBundle
    {
        private TMP_Text _nameText => GetUI<TMP_Text>("NameText");

        private BaseEnemy _curMonster;
        private int m_monsterIndex;
        private int _monsterIndex
        {
            get { return m_monsterIndex; }
            set
            {
                m_monsterIndex = value;
                _monsterIndexSubject.OnNext(m_monsterIndex);
            }
        }
        private Subject<int> _monsterIndexSubject = new Subject<int>();
        private List<TesterMonster> _testerMonsters = new List<TesterMonster>();

        protected override void Awake()
        {
            base.Awake();
            InitButtons();
        }

        private void Start()
        {
            SubscribeEvent();
            ChangeIndex(_monsterIndex);
        }
        /// <summary>
        /// 현재 선택된 몬스터 인덱스 결정
        /// </summary>
        private void ChangeIndex(int index)
        {
            if(index >= DataContainer.Monsters.Length)
            {
                _monsterIndex = 0;
                return;
            }              
            else if( index < 0)
            {
                _monsterIndex = DataContainer.Monsters.Length - 1;
                return;
            }
                
            _curMonster = DataContainer.Monsters[index];
            _nameText.SetText(_curMonster.GetState().Name);
        }

        /// <summary>
        /// 좀비 생성
        /// </summary>
        private void Create()
        {
            BaseEnemy monster = Instantiate(DataContainer.Monsters[_monsterIndex]);
            Vector3 createPos = new Vector3(
                _player.transform.position.x + (_player.transform.forward.x * Random.Range(createOffset.x - randomOffset.x, createOffset.x + randomOffset.x)),
                _player.transform.position.y + createOffset.y,
                _player.transform.position.z + (_player.transform.forward.z * Random.Range(createOffset.z - randomOffset.z, createOffset.z + randomOffset.z)));
            monster.transform.position = createPos;
            monster.gameObject.AddComponent(typeof(TesterMonster));

            TesterMonster testerMonster = monster.GetComponent<TesterMonster>();
            _testerMonsters.Add(testerMonster);
        }

        /// <summary>
        /// 모든 좀비 죽이기
        /// </summary>
        private void DieTesterMonster()
        {
            foreach(TesterMonster testerMonster in _testerMonsters)
            {
                testerMonster.Die();
            }
            _testerMonsters.Clear();
        }

        private void SubscribeEvent()
        {
            _monsterIndexSubject
                .DistinctUntilChanged()
                .Subscribe(x => ChangeIndex(x));

            GetUI<Button>("LeftButton").onClick.AddListener(() => _monsterIndex--);
            GetUI<Button>("RightButton").onClick.AddListener(() => _monsterIndex++);
            GetUI<Button>("CreateButton").onClick.AddListener(Create);
            GetUI<Button>("DeleteButton").onClick.AddListener(DieTesterMonster); 
        }

        private void InitButtons()
        {
            _buttons.Add(GetButtonStruct("LeftButton"));
            _buttons.Add(GetButtonStruct("RightButton"));
            _buttons.Add(GetButtonStruct("CreateButton"));
            _buttons.Add(GetButtonStruct("DeleteButton"));
        }
    }

}