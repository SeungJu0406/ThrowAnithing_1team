using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class NewUpgrade : BaseUI
{
    [Inject]
    private GlobalGameData _gameData;

    PlayerInput playerInput;

    [SerializeField] GameObject pause;

    //플레이어 / 카메라 제어용
    PlayerController player;
    float cameraSpeed;

    //Comment : Infomation > name
    [SerializeField] TMP_Text itemName;
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemInfo;
    [SerializeField] TMP_Text infotext;


    //Comment : for Test
    [SerializeField] int usedCost;

    int costLimit1 = 5000;
    int costLimit2 = 30000;
    int costLimit3 = 80000;
    int costLimit4 = 200000;

    // Comment : Cost Tier
    int tier;
    Color lockedColor = new(0.1f, 0, 0.2f);

    //텍스트 처리용
    [SerializeField] GameObject upText;

    int slot;

    List<List<Button>> buttons = new();
    List<Button> tier1 = new();
    List<Button> tier2 = new();
    List<Button> tier3 = new();
    List<Button> tier4 = new();
    List<Button> tier5 = new();

    [SerializeField] List<Button> buttonIndex = new();
    [SerializeField] List<Image> slotImage = new();
    List<Navigation> buttonNavigation = new();

    Button curButton;
    [SerializeField] GameObject textPos;

    [SerializeField] GameObject maxCoin;
    [SerializeField] GameObject zeroCoin;
    [SerializeField] GameObject save;

    [SerializeField] AudioClip upgradeSound;
    [SerializeField] AudioClip upgradeOpen;
    [SerializeField] AudioClip upgradeClose;

    private void Awake()
    {
        Bind();
        Init();
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(buttonIndex[0].gameObject);
        playerInput.SwitchCurrentActionMap(InputType.UI);
        SoundManager.PlaySFX(upgradeOpen);
    }

    private void OnDisable()
    {
        playerInput.SwitchCurrentActionMap(InputType.GAMEPLAY);
        SoundManager.PlaySFX(upgradeClose);
    }

    private void Update()
    {
        curButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

        //맥스코인, 제로코인, 세이브 눌렀을때 1번째 버튼으로 강제 이동
        if (EventSystem.current.currentSelectedGameObject == maxCoin ||
            EventSystem.current.currentSelectedGameObject == zeroCoin || 
            EventSystem.current.currentSelectedGameObject == save)
        {
            EventSystem.current.SetSelectedGameObject (buttonIndex[0].gameObject);
        }


        if (pause.activeSelf)
            return;

        MaxSlot();

        Slot_Selected();

        if (playerInput.actions["UIMove"].WasPressedThisFrame())
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.NaviMove);
        }
    }




    /// <summary>
    /// Comment : if usedCost greater than costLimit Method
    /// </summary>
    void TierCal()
    {
        tier = 1;
        if (_gameData.usingCoin >= costLimit1)
        {
            tier = 2;
        }
        if (_gameData.usingCoin >= costLimit2)
        { 
            tier = 3;
        }
        if (_gameData.usingCoin >= costLimit3)
        { 
            tier = 4; 
        }
        if (_gameData.usingCoin >= costLimit4)
        { 
            tier = 5; 
        }

        int i = (tier - 1) * 4;
        switch (tier)
        {
            // 티어 별로 해당하는 열의 버튼 잠금
            // 티어 별로 위아래로 잠금 해제
            case 1:
                for (int j = 0; j < 4; j++)
                {
                    NavigationLock(buttonIndex[(i + j)]);
                }
                break;

            case 2:
                for (int j = 0; j < 4; j++)
                {
                    NavigationLock(buttonIndex[(i + j)]);
                    NavigationUnLock(buttonIndex[j], buttonIndex[j + 4], buttonIndex[j + 4]);
                    NavigationUnLock(buttonIndex[j + 4], buttonIndex[j], buttonIndex[j]);
                    for (int index = 0; index < 8; index++)
                    {
                        buttonIndex[index].interactable = true;
                    }
                }
                break;

            case 3:
                for (int j = 0; j < 4; j++)
                {
                    NavigationLock(buttonIndex[(i + j)]);
                    NavigationUnLock(buttonIndex[j], buttonIndex[j + 8], buttonIndex[j + 4]);
                    NavigationUnLock(buttonIndex[j + 4], buttonIndex[j], buttonIndex[j + 8]);
                    NavigationUnLock(buttonIndex[j + 8], buttonIndex[j + 4], buttonIndex[j]);
                    for (int index = 0; index < 12; index++)
                    {
                        buttonIndex[index].interactable = true;
                    }
                }
                break;

            case 4:
                for (int j = 0; j < 4; j++)
                {
                    NavigationLock(buttonIndex[(i + j)]);
                    NavigationUnLock(buttonIndex[j], buttonIndex[j + 12], buttonIndex[j + 4]);
                    NavigationUnLock(buttonIndex[j + 8], buttonIndex[j + 4], buttonIndex[j + 12]);
                    NavigationUnLock(buttonIndex[j + 12], buttonIndex[j + 8], buttonIndex[j]);
                    for (int index = 0; index < 16; index++)
                    {
                        buttonIndex[index].interactable = true;
                    }
                }
                break;

            case 5:
                for (int j = 0; j < 4; j++)
                {
                    NavigationUnLock(buttonIndex[j + 12], buttonIndex[j + 8], buttonIndex[j + 16]);
                    NavigationUnLock(buttonIndex[j], buttonIndex[j + 16], buttonIndex[j + 4]);
                    for (int index = 0; index < 20; index++)
                    {
                        buttonIndex[index].interactable = true;
                    }
                }
                break;

        }
    }

    /// <summary>
    /// 티어에 맞게 네비게이션 이동 제한
    /// </summary>
    /// <param name="buttonIndex">버튼 인덱스</param>
    void NavigationLock(Button buttonIndex)
    {
        Navigation navi = buttonIndex.navigation;
        navi.selectOnUp = null;
        navi.selectOnDown = null;
        buttonIndex.navigation = navi;
    }

    /// <summary>
    /// 티어에 맞게 네비게이션 이동 제한 해제
    /// </summary>
    /// <param name="buttonIndex">버튼 인덱스</param>
    /// <param name="upbutton">selectOnUp 에 넣어줄 버튼</param>
    /// <param name="downButton">selectOnDown 에 넣어줄 버튼</param>
    void NavigationUnLock(Button buttonIndex, Button upbutton, Button downButton)
    {
        Navigation navi = buttonIndex.navigation;
        navi.selectOnUp = upbutton;
        navi.selectOnDown = downButton;
        buttonIndex.navigation = navi;
    }

    /// <summary>
    /// 사용한 코인 계산하여 버튼 활성화 / 비활성화
    /// </summary>
    void SlotLimit()
    {
        TierCal();

        for (int i = tier; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                //Todo : Change Color
                buttons[i][j].GetComponent<Image>().color = new(0.1f, 0, 0.2f);
                buttons[i][j].interactable = false;
            }
        }

    }


    /// <summary>
    /// Comment : 슬롯 처리
    /// </summary>
    void Slot_Selected()
    {
        Debug.Log("슬롯 셀렉티드 실행중");
        // Comment : 다른 슬롯 색 리셋
        ColorReset();
        // Comment : 선택한 슬롯 노란색으로
        curButton.GetComponent<Image>().color = new(0.7f, 0.7f, 0.1f);

        itemName.text = curButton.name;
        itemInfo.text = curButton.name;

        for (int i = 0; i < buttonIndex.Count; i++)
        {
            if (curButton == buttonIndex[i])
            {
                itemImage.sprite = slotImage[i].sprite;
                infotext.text = $"{_gameData.upgradeLevels[i]} / 5";
            }
        }

        //테스트용
        //MaxSlot();

        SlotLimit();
    }

    void MaxSlot()
    {
        //테스트용
        for (int i = 0; i < buttonIndex.Count; i++)
        {
            if (_gameData.upgradeLevels[i] == 5)
            {
                buttonIndex[i].transform.transform.GetChild(1).GetComponent<TMP_Text>().gameObject.SetActive(true);
                buttonIndex[i].onClick.RemoveAllListeners();

                SaveMaxSlot();
            }
        }
    }

    bool[] slotMaxCheck = new bool[20];

    /// <summary>
    /// 5강이 찍힌 특성에 강화 완료 표시 저장
    /// </summary>
    void SaveMaxSlot()
    {
        for (int i = 0; i < buttonIndex.Count; i++)
        {
            slotMaxCheck[i] = buttonIndex[i].transform.GetChild(1).GetComponent<TMP_Text>().gameObject.activeSelf;

        }
    }

    /// <summary>
    /// 모든 버튼 색상 초기화
    /// </summary>
    void ColorReset()
    {
        Debug.Log("컬러리셋 실행됨");
        for (int i = 0; i < buttonIndex.Count; i++)
            buttonIndex[i].GetComponent<Image>().color = new(0.2f, 0.25f, 0.6f);
    }

    public void UpgradeText()
    {
        // 업그레이드 완료 문구 없을때만 동작
        int cost = _gameData.upgradeCosts[slot];

        if (_gameData.coin >= cost)
            Instantiate(upText, textPos.transform.position, Quaternion.identity);
    }

    void Init()
    {
        playerInput = InputKey.PlayerInput;


        // 0 1 2 3  tier 1
        tier1.Add(GetUI<Button>("근접 공격력 증가"));
        tier1.Add(GetUI<Button>("원거리 공격력 증가"));
        tier1.Add(GetUI<Button>("이동 속도 증가"));
        tier1.Add(GetUI<Button>("최대 체력 증가"));
        // 4 5 6 7  tier 2
        tier2.Add(GetUI<Button>("스테미너 증가"));
        tier2.Add(GetUI<Button>("공격 속도 증가"));
        tier2.Add(GetUI<Button>("치명타 확률 증가"));
        tier2.Add(GetUI<Button>("장비 발견 확률 증가"));
        // 8 9 10 11 tier 3
        tier3.Add(GetUI<Button>("공격력 증가"));
        tier3.Add(GetUI<Button>("오브젝트 보유량 증가"));
        tier3.Add(GetUI<Button>("방어력 증가"));
        tier3.Add(GetUI<Button>("마나 회복력 증가"));
        // 12 13 14 15 tier 4
        tier4.Add(GetUI<Button>("스테미너 감소량 증가"));
        tier4.Add(GetUI<Button>("원거리 공격력 증가2"));
        tier4.Add(GetUI<Button>("근접 공격력 증가2"));
        tier4.Add(GetUI<Button>("오브젝트 획득량 증가"));
        // 16 17 18 19 tier 5
        tier5.Add(GetUI<Button>("마나 감소량 증가"));
        tier5.Add(GetUI<Button>("체력 흡수량 증가"));
        tier5.Add(GetUI<Button>("방어력 증가2"));
        tier5.Add(GetUI<Button>("장비 발견 확률 증가2"));

        buttons.Add(tier1);
        buttons.Add(tier2);
        buttons.Add(tier3);
        buttons.Add(tier4);
        buttons.Add(tier5);

        for (int i = 0; i < buttonIndex.Count; i++)
        {
            slotImage.Add(buttonIndex[i].transform.GetChild(0).GetComponent<Image>());
            buttonNavigation.Add(buttonIndex[i].navigation);

            int row = i;

            buttonIndex[i].onClick.AddListener(() => { UpgradeText(); SoundManager.PlaySFX(upgradeSound); });

        }

        player = GameObject.FindWithTag(Tag.Player).GetComponent<PlayerController>();


    }
}