using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class NewOption : BaseUI
{
    [Inject]
    OptionSetting setting;

    MainSceneBinding binding;

    PlayerInput playerInput;
    // 현재 상태
    // 0 = 옵션
    // 1 = 게임플레이
    // 2 = 소리
    int curDepth;

    // 뎁스1 버튼 목록
    Button gamePlayButton;
    Button soundButton;
    Button inputButton;
    Button exitButton;

    List<Button> optionButtons = new();

    //뎁스2 버튼 목록 - 게임플레이
    Button minimapAct;
    Button minimapFix;
    Button LanguageChange;
    Button sens;
    Slider sensSlider;

    Button accept_Gameplay;
    Button cancel_Gameplay;
    Button default_Gameplay;

    List<Button> gameplayButtons = new();

    //미니맵 관리용 불변수
    bool preAct;
    bool newAct;
    bool defaultAct;

    bool preFix;
    bool newFix;
    bool defaultFix;

    float preSens;
    float newSens;
    float defaultSens;

    GameObject actChecked; //미니맵 활성화 체크 여부
    GameObject fixChecked; //미니맵   고정 체크 여부
    GameObject actUnChecked;
    GameObject fixUnChecked;


    //뎁스2 버튼 목록 - 소리
    Button totalVolume;
    Button bgmVolume;
    Button sfxVolume;

    Slider totalVolumeBar;
    Slider bgmVolumeBar;
    Slider sfxVolumeBar;

    Button accept_Sound;
    Button cancel_Sound;
    Button default_Sound;

    List<Button> soundButtons = new();

    //사운드 관리용

    float preTotal;
    float newTotal;
    float defaultTotal;

    float preBgm;
    float newBgm;
    float defaultBgm;

    float preEffect;
    float newEffect;
    float defaultEffect;

    //조작키 관리용
    GameObject inputPC;
    GameObject inputConsole;

    //패널
    GameObject gameplayPanel;
    GameObject soundPanel;
    GameObject inputPanel;

    //코루틴
    Coroutine firstCo;

    [SerializeField] NewPause pausePanel;

    //클릭 허공에 했을때
    GameObject preButton;
    GameObject defaultButton;
    GameObject currentSelected;

    private void Awake()
    {
        Bind();
        Init();
    }

    private void OnEnable()
    {
        actChecked.SetActive(setting.miniMapOnBool);
        fixChecked.SetActive(setting.miniMapFixBool);
        sensSlider.value = setting.cameraSpeed;

        totalVolumeBar.maxValue = 1f;
        bgmVolumeBar.maxValue = 1f;
        sfxVolumeBar.maxValue = 1f;

        //볼륨바를 세팅된 값을 넣어줌
        totalVolumeBar.value = setting.wholesound;
        bgmVolumeBar.value = setting.backgroundSound;
        sfxVolumeBar.value = setting.effectSound;



        firstCo = null;
        //Todo : 자연스럽게 처리해야함
        binding.ButtonFirstSelect(gamePlayButton.gameObject);
        //현재 선택된 버튼 없을 때, 첫번째 버튼 설정
        if (firstCo == null)
        {
            firstCo = StartCoroutine(FirstRoutine());
        }
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (pausePanel != null)
            EventSystem.current.SetSelectedGameObject(pausePanel.continueButton.gameObject);

    }

    void Start()
    {
        //옵션창 열었을때 현재 버튼 목록 0으로 설정
        curDepth = 0;

    }

    IEnumerator FirstRoutine()
    {
        while (true)
        {
            Debug.Log("첫번째 버튼 선택되는중");
            binding.ButtonFirstSelect(gamePlayButton.gameObject);
            yield return 0.1f.GetDelay();
        }
    }

    void Update()
    {
        ButtonMissClick();
        CurDepthReset();
        DepthCal();

        if((EventSystem.current.currentSelectedGameObject != null))
        curTabChecker(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());

        SliderControl(sens.gameObject, sensSlider);
        SliderControl(totalVolume.gameObject, totalVolumeBar);
        SliderControl(bgmVolume.gameObject, bgmVolumeBar);
        SliderControl(sfxVolume.gameObject, sfxVolumeBar);

        SoundManager.SetVolumeMaster(setting.wholesound);
        SoundManager.SetVolumeBGM(setting.backgroundSound);
        SoundManager.SetVolumeSFX(setting.effectSound);

        if (playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.NaviMove);
        }

        if (playerInput.actions["LeftClick"].WasPressedThisFrame())
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.ClickNull);
        }
    }

    /// <summary>
    /// 허공에 클릭시 커뎁스 리셋
    /// </summary>
    void CurDepthReset()
    {
        if (curDepth > 0)
            Invoke("Term", 0.5f);
    }

    void Term()
    {
        if (EventSystem.current.currentSelectedGameObject == gamePlayButton.gameObject)
            curDepth = 0;

        if (EventSystem.current.currentSelectedGameObject == soundButton.gameObject)
            curDepth = 0;
    }

    /// <summary>
    /// 버튼 허공에 클릭했을 때, 기본 버튼 복구용 메서드
    /// </summary>
    void ButtonMissClick()
    {
        //선택된 버튼이 없을때 기본 버튼으로 복구하기 위한 변수 할당
        if (defaultButton == null)
            defaultButton = gamePlayButton.gameObject;
        //현재 버튼 저장
        currentSelected = EventSystem.current.currentSelectedGameObject;

        if (playerInput.actions["LeftClick"].WasPressedThisFrame() || playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            // 빈 공간을 클릭했을 때
            if (currentSelected == null)
            {
                Debug.Log("빈공간 클릭함");
                RestoreButton();
            }
            else
            {
                Debug.Log("클릭잘했음");
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

    void SliderControl(GameObject button, Slider slider)
    {
        if (EventSystem.current.currentSelectedGameObject == button)
        {
            if (playerInput.actions["UIMove"].WasPressedThisFrame())
            {
                // 입력 방향 확인
                Vector2 input = playerInput.actions["UIMove"].ReadValue<Vector2>();

                // 오른쪽 입력
                if (input.x > 0)
                {
                    if (button == sens.gameObject)
                        slider.value += 0.3f;
                    else
                        slider.value += 0.1f;
                }
                // 왼쪽 입력
                else if (input.x < 0)
                {
                    if (button == sens.gameObject)
                        slider.value -= 0.3f;
                    else
                        slider.value -= 0.1f;
                }
            }
        }
    }

    //현재 뎁스에 맞게 버튼 하이라이트
    void DepthCal()
    {
        switch (curDepth)
        {
            case 0:
                binding.SelectedButtonHighlight(optionButtons);
                break;

            case 1:
                binding.SelectedButtonHighlight(gameplayButtons);
                break;

            case 2:
                binding.SelectedButtonHighlight(soundButtons);
                break;
        }
    }
    /// <summary>
    /// 현재 포커싱된 메뉴에 맞게 패널 체인지
    /// </summary>
    /// <param name="curButton">현재 선택된 버튼</param>
    void curTabChecker(Button curButton)
    {

        if (curDepth == 0)
        {
            if (curButton == gamePlayButton)
            {
                gameplayPanel.SetActive(true);
                soundPanel.SetActive(false);
                InputPanel(false);
            }
            else if (curButton == soundButton)
            {
                gameplayPanel.SetActive(false);
                soundPanel.SetActive(true);
                InputPanel(false);
            }
            else if (curButton == inputButton)
            {
                gameplayPanel.SetActive(false);
                soundPanel.SetActive(false);
                InputPanel(true);
            }
            else if (curButton == exitButton)
            {
                gameplayPanel.SetActive(false);
                soundPanel.SetActive(false);
                InputPanel(false);
            }
        }
    }

    void InputPanel(bool onOff)
    {
        inputPanel.SetActive(onOff);


        inputPC.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.PC);
        inputConsole.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.CONSOLE);
    }

    //코루틴을 사용해 UI 초기화할 시간 벌어줌
    IEnumerator SetSelectRoutine(GameObject obj)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(obj);
    }

    public void GamePlayButton()
    {
        StartCoroutine(SetSelectRoutine(minimapAct.gameObject));
        curDepth = 1;
        firstCo = null;
    }

    public void SoundButton()
    {
        StartCoroutine(SetSelectRoutine(totalVolume.gameObject));
        curDepth = 2;
        firstCo = null;
    }

    public void InputButton()
    {
        StartCoroutine(SetSelectRoutine(inputButton.gameObject));
        firstCo = null;
    }

    public void ExitButton()
    {
        firstCo = null;

        gameplayPanel.SetActive(true);
        soundPanel.SetActive(false);
        InputPanel(false);

        binding.CanvasChange(binding.mainCanvas.gameObject, gameObject);
        curDepth = 0;
    }

    public void ExitButton_Pause()
    {
        firstCo = null;
        curDepth = 0;

        gameplayPanel.SetActive(true);
        soundPanel.SetActive(false);
        InputPanel(false);

        gameObject.SetActive(false);
    }

    public void MinimapAct()
    {
        actChecked.SetActive(!actChecked.activeSelf);
        setting.miniMapOnBool = actChecked.activeSelf;
        StartCoroutine(SetSelectRoutine(minimapAct.gameObject));
        curDepth = 1;
    }

    public void MinimapFix()
    {
        fixChecked.SetActive(!fixChecked.activeSelf);
        setting.miniMapFixBool = fixChecked.activeSelf;
        StartCoroutine(SetSelectRoutine(minimapFix.gameObject));
        curDepth = 1;
    }

    /// <summary>
    /// 적용, 취소, 초기화 했을때 버튼 색상 초기화
    /// </summary>
    /// <param name="list"></param>
    void ButtonReset(List<Button> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            list[i].GetComponent<TMP_Text>().color = Color.white;
        }
    }

    public void AcceptButton_Gameplay()
    {
        ButtonReset(gameplayButtons);
        SoundManager.PlaySFX(SoundManager.Data.UI.SettingButton);
        EventSystem.current.SetSelectedGameObject(gamePlayButton.gameObject);

        curDepth = 0;
    }

    public void CancelButton_Gameplay()
    {
        ButtonReset(gameplayButtons);
        SoundManager.PlaySFX(SoundManager.Data.UI.SettingButton);
        EventSystem.current.SetSelectedGameObject(gamePlayButton.gameObject);

        curDepth = 0;
    }

    public void DefaultButton_Gameplay()
    {
        ButtonReset(gameplayButtons);
        SoundManager.PlaySFX(SoundManager.Data.UI.SettingButton);
        EventSystem.current.SetSelectedGameObject(gamePlayButton.gameObject);

        curDepth = 0;
    }

    public void AcceptButton_Sound()
    {
        ButtonReset(soundButtons);
        SoundManager.PlaySFX(SoundManager.Data.UI.SettingButton);
        EventSystem.current.SetSelectedGameObject(soundButton.gameObject);
        curDepth = 0;
    }


    public void CancelButton_Sound()
    {
        ButtonReset(soundButtons);
        SoundManager.PlaySFX(SoundManager.Data.UI.SettingButton);
        EventSystem.current.SetSelectedGameObject(soundButton.gameObject);

        curDepth = 0;
    }

    public void DefaultButton_Sound()
    {
        ButtonReset(soundButtons);
        SoundManager.PlaySFX(SoundManager.Data.UI.SettingButton);
        EventSystem.current.SetSelectedGameObject(soundButton.gameObject);

        curDepth = 0;
    }
    void Init()
    {
        binding = GetComponentInParent<MainSceneBinding>();

        playerInput = InputKey.PlayerInput;

        optionButtons.Add(gamePlayButton = GetUI<Button>("GamePlay"));
        optionButtons.Add(soundButton = GetUI<Button>("Sound"));
        optionButtons.Add(inputButton = GetUI<Button>("Input"));
        optionButtons.Add(exitButton = GetUI<Button>("Exit"));


        gameplayPanel = GetUI("GamePlayPackage");
        soundPanel = GetUI("SoundPackage");
        inputPanel = GetUI("InputGuide");

        gameplayButtons.Add(minimapAct = GetUI<Button>("MinimapAct"));
        gameplayButtons.Add(minimapFix = GetUI<Button>("MinimapFix"));
        gameplayButtons.Add(LanguageChange = GetUI<Button>("Language"));
        gameplayButtons.Add(sens = GetUI<Button>("Sens"));
        gameplayButtons.Add(accept_Gameplay = GetUI<Button>("GamePlay_Accept"));
        gameplayButtons.Add(cancel_Gameplay = GetUI<Button>("GamePlay_Cancel"));
        gameplayButtons.Add(default_Gameplay = GetUI<Button>("GamePlay_Default"));

        actChecked = GetUI("ActChecked");
        fixChecked = GetUI("FixChecked");
        actUnChecked = GetUI("ActUnChecked");
        fixUnChecked = GetUI("FixUnChecked");

        soundButtons.Add(totalVolume = GetUI<Button>("TotalSound"));
        soundButtons.Add(bgmVolume = GetUI<Button>("BGMSound"));
        soundButtons.Add(sfxVolume = GetUI<Button>("SFXSound"));
        soundButtons.Add(accept_Sound = GetUI<Button>("Sound_Accept"));
        soundButtons.Add(cancel_Sound = GetUI<Button>("Sound_Cancel"));
        soundButtons.Add(default_Sound = GetUI<Button>("Sound_Default"));

        sensSlider = GetUI<Slider>("SensSlider");
        totalVolumeBar = GetUI<Slider>("TotalSoundBar");
        bgmVolumeBar = GetUI<Slider>("BGMSoundBar");
        sfxVolumeBar = GetUI<Slider>("SFXSoundBar");

        gamePlayButton.onClick.AddListener(GamePlayButton);
        soundButton.onClick.AddListener(SoundButton);
        inputButton.onClick.AddListener(InputButton);


        // 메인씬일 경우와 포즈 > 옵션인 경우 구분
        if (SceneManager.GetActiveScene().name == SceneName.MainScene)
            exitButton.onClick.AddListener(ExitButton);
        else
            exitButton.onClick.AddListener(ExitButton_Pause);

        minimapAct.onClick.AddListener(MinimapAct);
        minimapFix.onClick.AddListener(MinimapFix);
        actChecked.GetComponent<Button>().onClick.AddListener(MinimapAct);
        fixChecked.GetComponent<Button>().onClick.AddListener(MinimapFix);
        actUnChecked.GetComponent<Button>().onClick.AddListener(MinimapAct);
        fixUnChecked.GetComponent<Button>().onClick.AddListener(MinimapFix);

        preAct = setting.miniMapOnBool;
        preFix = setting.miniMapFixBool;

        accept_Gameplay.onClick.AddListener(AcceptButton_Gameplay);
        cancel_Gameplay.onClick.AddListener(CancelButton_Gameplay);
        default_Gameplay.onClick.AddListener(DefaultButton_Gameplay);

        inputPC = GetUI("inputPC");
        inputConsole = GetUI("inputConsole");



        accept_Sound.onClick.AddListener(AcceptButton_Sound);
        cancel_Sound.onClick.AddListener(CancelButton_Sound);
        default_Sound.onClick.AddListener(DefaultButton_Sound);

    }
}
