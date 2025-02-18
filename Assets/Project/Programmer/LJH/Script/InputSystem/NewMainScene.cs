using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class NewMainScene : BaseUI
{
    [Inject]
    OptionSetting setting;

    PlayerInput playerInput;
    MainSceneBinding binding;
    
    //버튼들
    Button continueButton;
    Button newButton;
    Button optionButton;
    Button exitButton;

    List<Button> buttons = new List<Button>();


    private void Awake()
    {
        Bind();
        Init();
    }
    private void OnEnable()
    {
    }

    void Start()
    {
        setting.OptionLoad();
        SoundManager.SetVolumeMaster(setting.wholesound);
        SoundManager.SetVolumeBGM(setting.backgroundSound);
        SoundManager.SetVolumeSFX(setting.effectSound);

        EventSystem.current.SetSelectedGameObject(continueButton.gameObject);

        SoundManager.StopBGM();
        SoundManager.PlayBGM(SoundManager.Data.BGM.Main);
        
    }

    void Update()
    {
        binding.ButtonFirstSelect(continueButton.gameObject);
        binding.SelectedButtonHighlight(buttons);

        if (playerInput.actions["Choice"].WasPressedThisFrame())
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }

        if (playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            if (playerInput.actions["UIMove"].ReadValue<Vector2>().y != 0)
            SoundManager.PlaySFX(SoundManager.Data.UI.NaviMove);
        }

        if (playerInput.actions["LeftClick"].WasPressedThisFrame())
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.ClickNull);
        }
    }



    public void EnterContinue()
    {
        binding.CanvasChange(binding.continueCanvas.gameObject, gameObject);
    }

    public void EnterNew()
    {
        binding.CanvasChange(binding.newCanvas.gameObject, gameObject);
    }

    public void EnterOption()
    {
        binding.CanvasChange(binding.optionCanvas.gameObject, gameObject);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        //Comment : 유니티 에디터상에서 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //Comment : 빌드 상에서 종료
        Application.Quit();
#endif
    }
    void Init()
    {
        binding = GetComponentInParent<MainSceneBinding>();

        playerInput = GameObject.FindWithTag("InputKey").GetComponent<PlayerInput>();

        //버튼 바인딩 및 버튼 주입
        buttons.Add(continueButton = GetUI<Button>("Continue"));
        buttons.Add(newButton = GetUI<Button>("New"));
        buttons.Add(optionButton = GetUI<Button>("Option"));
        buttons.Add(exitButton = GetUI<Button>("Exit"));

        //버튼 주입
        continueButton.onClick.AddListener(EnterContinue);
        newButton.onClick.AddListener(EnterNew);
        optionButton.onClick.AddListener(EnterOption);
        exitButton.onClick.AddListener(ExitGame);

    }
}
