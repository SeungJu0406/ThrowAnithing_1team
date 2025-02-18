using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewPause : BaseUI
{
    PlayerInput playerInput;

    MainSceneBinding binding;


    List<Button> SlotList = new();
    [HideInInspector] public Button continueButton;
    Button optionButton;
    Button lobbyButton;

    GameObject pause;
    GameObject optionPanel;

    GameObject exitPopUp;

    Coroutine firstCo;

    Color color;

    GameObject preButton;
    GameObject defaultButton;
    GameObject currentSelected;

    Navigation conOriginalNavi;
    Navigation opOriginalNavi;
    Navigation lobbyOriginalNavi;


    private void Awake()
    {
        Bind();
        Init();

    }

    private void OnEnable()
    {
        // 로비씬 첫 진입때 액션맵 바꿔줌
        playerInput.SwitchCurrentActionMap(InputType.GAMEPLAY);
    }

    private void Start()
    {
        SoundManager.StopBGM();
        if (SceneManager.GetActiveScene().name == SceneName.MainScene)
            return;
        if (SceneManager.GetActiveScene().name == SceneName.LobbyScene)
            SoundManager.PlayBGM(SoundManager.Data.BGM.Lobby);
        else
            SoundManager.PlayBGM(SoundManager.Data.BGM.InGame);
    }

    private void Update()
    {
        if(!exitPopUp.activeSelf)
        {
            lobbyButton.navigation = lobbyOriginalNavi;
        }
            

        //퍼즈 열기
        if (playerInput.actions["Open_Settings"].WasPressedThisFrame())
        {
            Time.timeScale = 0f;
            playerInput.SwitchCurrentActionMap(InputType.UI);
            
            pause.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            if (firstCo == null)
                firstCo = StartCoroutine(FirstRoutine());
        }

        ButtonMissClick();

        if (!optionPanel.activeSelf && pause.activeSelf)
        {
            SelectedSlotColorChange();
        }

        if (playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            if(pause.activeSelf)
                if (playerInput.actions["UIMove"].ReadValue<Vector2>().y != 0)
                    SoundManager.PlaySFX(SoundManager.Data.UI.NaviMove);
        }

        

    }

    /// <summary>
    /// 네비게이션 막아줘서 뒷배경 버튼 이동막아줌
    /// </summary>
    void NaviBlock()
    {
        Navigation navi = lobbyButton.navigation;
        navi.selectOnUp = null;
        navi.selectOnDown = null;
        lobbyButton.navigation = navi;
    }

    /// <summary>
    /// 버튼 허공에 클릭했을 때, 기본 버튼 복구용 메서드
    /// </summary>
    void ButtonMissClick()
    {
        //선택된 버튼이 없을때 기본 버튼으로 복구하기 위한 변수 할당
        if (defaultButton == null)
            defaultButton = continueButton.gameObject;
        //현재 버튼 저장
        currentSelected = EventSystem.current.currentSelectedGameObject;

        if (playerInput.actions["LeftClick"].WasPressedThisFrame() || playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            // 빈 공간을 클릭했을 때
            if (currentSelected == null)
            {
                RestoreButton();
            }
            else
            {
                // 현재 선택된 버튼을 저장
                preButton = currentSelected;
            }
        }
    }

    /// <summary>
    /// 빈공간 눌렀을 때 버튼 복구용 함수
    /// </summary>
    public void RestoreButton()
    {
        if (playerInput.actions["LeftClick"].WasPressedThisFrame())
        {
            if (preButton != null)
            {
                if (EventSystem.current.currentSelectedGameObject == null)
                    EventSystem.current.SetSelectedGameObject(preButton);
            }
            else if (preButton == null)
                EventSystem.current.SetSelectedGameObject(defaultButton);
        }
    }

    /// <summary>
    /// 진입시 첫 버튼 선택해주는 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FirstRoutine()
    {

        while (true)
        {
            binding.ButtonFirstSelect(continueButton.gameObject);

            yield return 0.1f.GetDelay();
        }
    }

    /// <summary>
    /// 선택된 버튼의 투명도 제어
    /// </summary>
    void SelectedSlotColorChange()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            for (int i = 0; i < SlotList.Count; i++)
            {
                color = SlotList[i].GetComponent<Image>().color;
                color.a = 0.1f;
                SlotList[i].GetComponent<Image>().color = color;
            }

            color = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color;
            color.a = 1f;
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = color;
        }

    }

    /// <summary>
    /// 계속하기 눌렀을 때
    /// </summary>
    void ContinueGame()
    {
        //Todo : 퍼즈창 닫아야함
        playerInput.SwitchCurrentActionMap(InputType.GAMEPLAY);
        Time.timeScale = 1f;
        firstCo = null;
        pause.SetActive(false);
    }

    /// <summary>
    /// 환경설정 눌렀을 때
    /// </summary>
    void OptionButton()
    {
        //Todo : 옵션 캔버스 열어줘야함
        firstCo = null;
        EventSystem.current.SetSelectedGameObject(null);
        optionPanel.SetActive(true);
        
    }

    /// <summary>
    /// 돌아가기 눌렀을 때
    /// </summary>
    void ReturnLobby()
    {
        firstCo = null;
        exitPopUp.SetActive(true);
        NaviBlock();

    }


    void Init()
    {

        playerInput = InputKey.PlayerInput;
        binding = GetComponent<MainSceneBinding>();

        pause = GetUI("pause");
        SlotList.Add(continueButton = GetUI<Button>("ContinueImage"));
        SlotList.Add(optionButton = GetUI<Button>("OptionImage"));
        SlotList.Add(lobbyButton = GetUI<Button>("ExitImage"));

        conOriginalNavi = continueButton.navigation;
        opOriginalNavi = optionButton.navigation;
        lobbyOriginalNavi = lobbyButton.navigation;

        optionPanel = GetUI("OptionCanvas");
        exitPopUp = GetUI("ExitPopUp");

        continueButton.onClick.AddListener(ContinueGame);
        optionButton.onClick.AddListener(OptionButton);
        lobbyButton.onClick.AddListener(ReturnLobby);
    }
}
