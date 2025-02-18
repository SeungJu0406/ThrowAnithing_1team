using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LJH_UIInput : MonoBehaviour
{
    
    public GameObject firstButton;

    void Start()
    {
        // 첫 번째 버튼 포커스
        EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        // 키보드 입력에 따라 버튼 탐색
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current != null)
            {
                SelectNextButton(current);
            }

        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject current = EventSystem.current.currentSelectedGameObject;
            if (current != null)
            {
                ClickButton(current);
            }

        }
    }

    void SelectNextButton(GameObject current)
    {
        // 원하는 버튼 탐색 로직을 여기에 추가
        var nextButton = current.GetComponent<Selectable>().FindSelectableOnDown();
        if (nextButton != null)
        {
            EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
        }
    }

    void ClickButton(GameObject current)
    {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        
    }
}

