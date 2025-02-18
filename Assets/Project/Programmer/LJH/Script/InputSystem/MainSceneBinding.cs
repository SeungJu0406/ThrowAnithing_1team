using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class MainSceneBinding : BaseUI
{
    //캔버스들
    [HideInInspector] public Canvas mainCanvas;
    [HideInInspector] public Canvas continueCanvas;
    [HideInInspector] public Canvas newCanvas;
    [HideInInspector] public Canvas optionCanvas;


    private void Awake()
    {
        Bind();
        Init();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 캔버스 교체 함수
    /// </summary>
    /// <param name="loadCanvas">불러올 캔버스</param>
    /// <param name="curCanvas">꺼줄 캔버스(현재 켜져있는 캔버스)</param>
    public void CanvasChange(GameObject loadCanvas, GameObject curCanvas)
    {

        loadCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        curCanvas.SetActive(false);
    }

    /// <summary>
    /// 선택된 버튼 색상 변경
    /// </summary>
    /// <param name="buttons">포커싱된 버튼 배열</param>
    public void SelectedButtonHighlight(List<Button> buttons)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].GetComponent<TMP_Text>().color = Color.white;

                if (buttons[i] == EventSystem.current.currentSelectedGameObject.GetComponent<Button>())
                {
                    buttons[i].GetComponent<TMP_Text>().color = new Color(1, 0.5f, 0);
                }
            }
    }
    /// <summary>
    /// 선택된 슬롯 색상 변경
    /// </summary>
    /// <param name ="slots">지금 쓰는 버튼 리스트</param>
    public void SelectedSlotHighlight(List<Button> slots)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.1f);

                if (slots[i] == EventSystem.current.currentSelectedGameObject.GetComponent<Image>())
                    slots[i].GetComponent<Image>().color = new Color(1, 0.5f, 0, 1);
            }
    }

    /// <summary>
    /// 페이지 진입시 초기 버튼 선택
    /// </summary>
    /// <param name="firstButton">포커싱된 버튼 리스트의 첫번째 버튼</param>
    public void ButtonFirstSelect(GameObject firstButton)
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }

    void Init()
    {
        //캔버스 바인딩
        mainCanvas = GetUI<Canvas>("MainCanvas");
        continueCanvas = GetUI<Canvas>("ContinueCanvas");
        newCanvas = GetUI<Canvas>("NewCanvas");
        optionCanvas = GetUI<Canvas>("OptionCanvas");


    }
}
