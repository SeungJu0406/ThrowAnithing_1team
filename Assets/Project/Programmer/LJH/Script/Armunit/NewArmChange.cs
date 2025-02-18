using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;
using Zenject.SpaceFighter;

public class NewArmChange : BaseUI
{
    PlayerInput playerInput;
    [Inject]
    GlobalGameData playerStateData;
    [Inject]
    PlayerData playerData;
    int arm_cur;
    List<GameObject> armUnits = new();
    List<Button> armButtons = new();

    [SerializeField] SaveSystem saveSystem;

    Forge _forge;

    Color color;

    [SerializeField] public AudioClip powerSelect;
    [SerializeField] public AudioClip balanceSelect;

    [SerializeField] AudioClip armOpen;
    [SerializeField] AudioClip armClose;

    private void Awake()
    {
        Bind();
        Init();
        _forge = GetComponentInParent<Forge>();
    }


    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap(InputType.UI);
        EventSystem.current.SetSelectedGameObject(armButtons[0].gameObject);
        SoundManager.PlaySFX(armOpen);
    }
    private void OnDisable()
    {
        if(playerInput.currentActionMap.name == InputType.UI)
        playerInput.SwitchCurrentActionMap(InputType.GAMEPLAY);
        SoundManager.PlaySFX(armClose);
    }
    private void Update()
    {
        ArmUnit_changeColor();

        if (playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            SoundPlay();
            
        }
    }
    

    void SoundPlay()
    {
        SoundManager.PlaySFX(SoundManager.Data.UI.NaviMove);
    }
    //Todo : 함수 이름 바꿔야함
    void ArmUnit_changeColor()
    {
        for (int i = 0; i < armUnits.Count; i++)
        {
            color = armUnits[i].GetComponent<Image>().color;
            color.a = 0.1f;
            armUnits[i].GetComponent<Image>().color = color;
        }

        color = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color;
        color.a = 1f;
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = color;

        saveSystem.SavePlayerData();

    }

    void SelectArm()
    {
        if (EventSystem.current.currentSelectedGameObject == armUnits[0].gameObject)
        {
            armButtons[0].onClick.Invoke();
        }
        else if(EventSystem.current.currentSelectedGameObject == armUnits[1].gameObject)
        {
            armButtons[1].onClick.Invoke();
        }
    }

    #region 테스트용 함수
    public void Power()
    {
        Debug.Log("파워 타입 선택");
        playerStateData.nowWeapon = GlobalGameData.AmWeapon.Power;
        playerData.NowWeapon = playerStateData.nowWeapon;
        gameObject.SetActive(false);
        // 데이터 세이브
        saveSystem.SavePlayerData();
    }

    public void Balance()
    {
        Debug.Log("밸런스 타입 선택");
        playerStateData.nowWeapon = GlobalGameData.AmWeapon.Balance;
        playerData.NowWeapon = playerStateData.nowWeapon;
        gameObject.SetActive(false);
        // 데이터 세이브
        saveSystem.SavePlayerData();
    }
    #endregion

    public void ArmUnitClose()
    {
        // gameObject.SetActive(false);
        _forge.SetUnActiveUI();
    }
    void Init()
    {
        playerInput = InputKey.PlayerInput;

        armUnits.Add(GetUI("PowerButton"));
        armUnits.Add(GetUI("BalanceButton"));

        armButtons.Add(GetUI<Button>("PowerButton"));
        armButtons.Add(GetUI<Button>("BalanceButton"));

        armButtons[0].onClick.AddListener(() => { Power(); SoundManager.PlaySFX(powerSelect); });
        armButtons[1].onClick.AddListener(() => { Balance(); SoundManager.PlaySFX(balanceSelect);  });

    }
}