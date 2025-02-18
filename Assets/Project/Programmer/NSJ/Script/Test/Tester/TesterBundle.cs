using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NSJ_TesterPanel
{
    public class TesterBundle : BaseUI
    {
        protected struct ButtonStruct
        {
            public Button Button;
            public Image Image;
        }
        protected List<ButtonStruct> _buttons = new List<ButtonStruct>();

        protected int _buttonIndex;

        protected GameObject _player;
        protected bool _canControl;

        [SerializeField] protected Vector3 createOffset = new Vector3(5,0,5);
        [SerializeField] protected Vector3 randomOffset;
        protected virtual void Awake()
        {
            Bind();
            _player = GameObject.FindGameObjectWithTag(Tag.Player);
        }
        protected virtual void OnEnable()
        {

        }
        protected virtual void OnDisable()
        {

        }

        private void Update()
        {
            CheckSelect();
        }
        protected ButtonStruct GetButtonStruct(string name)
        {
            ButtonStruct buttonStruct = new ButtonStruct();
            buttonStruct.Button = GetUI<Button>(name);
            buttonStruct.Image = GetUI<Image>(name);
            return buttonStruct;
        }

        /// <summary>
        /// 하이라이트 결정 (-10은 모두 끄기)
        /// </summary>
        /// <param name="index"></param>
        private void ChangeHighlight(int index)
        {
            if(index == -10)
            {
                foreach(ButtonStruct button in _buttons)
                {
                    button.Image.color = Color.white;
                }
            }
            for (int i = 0; i < _buttons.Count; i++)
            {
                if(index == i)
                {
                    _buttons[i].Image.color = Color.yellow;
                }
                else
                {
                    _buttons[i].Image.color = Color.white;
                }
            }
        }

        private void InteractButton(int index)
        {
            _buttons[index].Button.onClick.Invoke();
        }

        private void CheckSelect()
        {
            GameObject curSelect = EventSystem.current.currentSelectedGameObject;

            int index = _buttons.FindIndex(button => button.Button.gameObject.Equals(curSelect));
            if (index == -1)
            {
                ChangeHighlight(-10);
            }
            else
            {
                ChangeHighlight(index);
            }              
        }
    }
}