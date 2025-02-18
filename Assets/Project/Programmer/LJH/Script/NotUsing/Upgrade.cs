/*using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class Upgrade : BaseUI
{
    [Inject]
    private GlobalGameData _gameData;

    [SerializeField] GameObject pause;

    //플레이어 / 카메라 제어용
    PlayerController player;
    float cameraSpeed;

    Button[,] slots;
    Image[,] slotImages;

    int ho;
    int ver;

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

    bool axisInUse; // 연속 조작 방지용

    //텍스트 처리용
    [SerializeField] GameObject upText;

    int slot;

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

        cameraSpeed = player.setting.cameraSpeed;
        player.setting.cameraSpeed = 0;
    }

    private void OnDisable()
    {
        ver = 0;
        ho = 0;

        player.setting.cameraSpeed = cameraSpeed;
    }

    private void Update()
    {


        if (pause.activeSelf)
            return;

        MaxSlot();

        Slot_Selected();

        //Comment : For test
        if (InputKey.GetButtonDown(InputKey.PrevInteraction))
        {
            slots[ver, ho].onClick.Invoke();
        }
        
    

    }



    //Comment : if usedCost greater than costLimit Method
    void TierCal()
    {
        tier = 1;
        if (_gameData.usingCoin >= costLimit1)
            tier = 2;
        if (_gameData.usingCoin >= costLimit2)
            tier = 3;
        if (_gameData.usingCoin >= costLimit3)
            tier = 4;
        if (_gameData.usingCoin >= costLimit4)
            tier = 5;
    }

    void SlotLimit()
    {
        TierCal();

        for (int i = tier; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                //Todo : Change Color
                slots[i, j].GetComponent<Image>().color = new(0.1f, 0, 0.2f);
                slots[i, j].interactable = false;
            }
        }

    }


    //Comment : 슬롯 이동 함수
    void Slot_Selected()
    {

        float x = InputKey.GetAxisRaw(InputKey.Horizontal);
        float y = -InputKey.GetAxisRaw(InputKey.Vertical);

        if (x > 0)
        {
            if (axisInUse == false)
            {
                ho += 1;
                axisInUse = true;
            }
        }
        else if (x < 0)
        {
            if (axisInUse == false)
            {
                ho -= 1;
                axisInUse = true;
            }
        }

        else if (y > 0)
        {
            if (axisInUse == false)
            {
                ver += 1;
                axisInUse = true;
            }
        }
        else if (y < 0)
        {
            if (axisInUse == false)
            {
                ver -= 1;
                axisInUse = true;
            }
        }
        else
        {
            axisInUse = false;
        }


        ho = ho == -1 ? 3 : ho == 4 ? 0 : ho;
        ver = ver == -1 ? tier - 1 : ver == tier ? 0 : ver;

        if (ver == -1)
            ver = 0;

        // Comment : 다른 슬롯 색 리셋
        ColorReset();
        // Comment : 선택한 슬롯 노란색으로

        slots[ver, ho].GetComponent<Image>().color = new(0.7f, 0.7f, 0.1f);

        itemName.text = slots[ver, ho].name;
        itemImage.sprite = slotImages[ver, ho].sprite;
        itemInfo.text = slots[ver, ho].name;

        
        int slotNum = ver * 4 + ho; // 1차원 배열의 인덱스를 계산
        infotext.text = $"{_gameData.upgradeLevels[slotNum]} / 5";
        slot = slotNum;

        //테스트용
        //MaxSlot();

        SlotLimit();
    }

    void MaxSlot()
    {
        //테스트용
        for (int i = 0; i < slotMaxCheck.Length; i++)
        {
            if (_gameData.upgradeLevels[i] == 5)
            {
                int ver;
                int ho;

                ver = i / 4;
                ho = i % 4;
                slots[ver, ho].transform.transform.GetChild(1).GetComponent<TMP_Text>().gameObject.SetActive(true);
                slots[ver, ho].onClick.RemoveAllListeners();

                SaveMaxSlot();
            }
        }
    }

    bool[] slotMaxCheck = new bool[20];

    //5강이 찍힌 특성에 강화 완료 표시 저장
    void SaveMaxSlot()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                int index = i * 4 + j;
                slotMaxCheck[index] = slots[i, j].transform.GetChild(1).GetComponent<TMP_Text>().gameObject.activeSelf;
            }
        }
    }

    
    //슬롯 클릭시
    void ClickedSlots(Button button)
    {
        if (button.GetComponent<Image>().color != lockedColor)
        {
            slots[ver, ho].GetComponent<Image>().color = new(0.2f, 0.25f, 0.6f);

            if (EventSystem.current.currentInputModule != InputKey.GetButtonDown(InputKey.PrevInteraction))
            {
                (ver, ho) = FindButton(EventSystem.current.currentSelectedGameObject.GetComponent<Button>());
            }

            itemName.text = button.name;
            itemInfo.text = button.name;
            slotImages[ver, ho] = button.transform.GetChild(0).GetComponent<Image>();
            slots[ver, ho].GetComponent<Image>().color = new(0.7f, 0.7f, 0.1f);

            if (button.GetComponent<Image>().color != lockedColor)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        slots[i, j].interactable = true;
                    }
                }
            }
        }
    }

    // 선택한 버튼의 인덱스로 slots 인덱스 교체
    (int, int) FindButton(Button button)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (slots[i, j].name == button.name)
                {
                    return (i, j);
                }
            }
        }
        //인식 못했을 때 0,0 으로 초기화
        return (0, 0);

    }

    void ColorReset()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                slots[i, j].GetComponent<Image>().color = new(0.2f, 0.25f, 0.6f);
            }
        }
    }

    public void UpgradeText()
    {
        // 업그레이드 완료 문구 없을때만 동작
        int cost = _gameData.upgradeCosts[slot];

        if (_gameData.coin >= cost)
            Instantiate(upText, slots[2,3].transform.position, Quaternion.identity);
    }

    void Init()
    {
        slots = new Button[5, 4];
        slotImages = new Image[5, 4];

        slots[0, 0] = GetUI<Button>("근접 공격력 증가");
        slots[0, 1] = GetUI<Button>("원거리 공격력 증가");
        slots[0, 2] = GetUI<Button>("이동 속도 증가");
        slots[0, 3] = GetUI<Button>("최대 체력 증가");

        slots[1, 0] = GetUI<Button>("스테미너 증가");
        slots[1, 1] = GetUI<Button>("공격 속도 증가");
        slots[1, 2] = GetUI<Button>("치명타 확률 증가");
        slots[1, 3] = GetUI<Button>("장비 발견 확률 증가");

        slots[2, 0] = GetUI<Button>("공격력 증가");
        slots[2, 1] = GetUI<Button>("오브젝트 보유량 증가");
        slots[2, 2] = GetUI<Button>("방어력 증가");
        slots[2, 3] = GetUI<Button>("마나 회복력 증가");

        slots[3, 0] = GetUI<Button>("스테미너 감소량 증가");
        slots[3, 1] = GetUI<Button>("원거리 공격력 증가2");
        slots[3, 2] = GetUI<Button>("근접 공격력 증가2");
        slots[3, 3] = GetUI<Button>("오브젝트 획득량 증가");

        slots[4, 0] = GetUI<Button>("마나 감소량 증가");
        slots[4, 1] = GetUI<Button>("체력 흡수량 증가");
        slots[4, 2] = GetUI<Button>("방어력 증가2");
        slots[4, 3] = GetUI<Button>("장비 발견 확률 증가2");

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                slotImages[i, j] = slots[i, j].transform.GetChild(0).GetComponent<Image>();

                int row = i;
                int col = j;

                slots[i, j].onClick.AddListener(() => ClickedSlots(slots[row, col]));
                slots[i, j].onClick.AddListener(() => UpgradeText());
                
            }
        }

        player = GameObject.FindWithTag(Tag.Player).GetComponent<PlayerController>();
    }
}*/