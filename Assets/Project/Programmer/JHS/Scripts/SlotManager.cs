using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Zenject;
using UnityEngine.SceneManagement;

public class SlotManager : MonoBehaviour
{
    public Button[] continueSlotButtons; // 컨티뉴 슬롯 버튼 배열
    public TextMeshProUGUI[] continueSlotTexts;     // 컨티뉴 슬롯 텍스트 배열

    public Button[] newGameSlotButtons;  // 뉴게임 슬롯 버튼 배열
    public TextMeshProUGUI[] newGameSlotTexts;      // 뉴게임 슬롯 텍스트 배열
    [Inject]
    private UserDataManager userDataManager;
    [Inject]
    private GlobalGameData globalPlayerData;
    [Inject]
    private LobbyUpGrade lobbyUpGrade;
    [Inject]
    private GlobalPlayerStateData globalPlayerState;

    public GameObject confirmDeleteUI; // 확인 UI
    public Button confirmButton; // 확인 버튼
    public Button cancelButton; // 취소 버튼

    private int selectedSlotIndex; // 선택된 슬롯 인덱스
    [SerializeField] private SceneField _lobbyScene; // 로비 씬
    
    //이재호가 추가한 변수
    // 네비게이션 끄기/켜기 용 변수
    [SerializeField] Button slot1;
    [SerializeField] Button slot2;
    [SerializeField] Button slot3;
    private void Start()
    {
        // 슬롯 UI 초기화
        UpdateSlotUI();

        // 확인/취소 버튼 이벤트 연결
        confirmButton.onClick.AddListener(OnConfirmDelete);
        cancelButton.onClick.AddListener(OnCancelDelete);

        // UI 초기 비활성화
        confirmDeleteUI.SetActive(false);
    }
    private void UpdateSlotUI()
    {
        // 슬롯 UI 갱신
        for (int i = 0; i < continueSlotButtons.Length; i++)
        {
            string slotPath = userDataManager.path + $"slot_{i}.json"; // 각 슬롯마다 파일 경로 다르게 설정
            if (File.Exists(slotPath))
            {
                string data = File.ReadAllText(slotPath);
                GlobalGameData playerData = JsonUtility.FromJson<GlobalGameData>(data);

                // 저장된 시간 표시
                string saveTime = string.IsNullOrEmpty(playerData.saveDateTime) ? "No Date" : playerData.saveDateTime;

                continueSlotTexts[i].text = $"Last Saved: {saveTime}";
                newGameSlotTexts[i].text = $"Last Saved: {saveTime}";
            }
            else
            {
                continueSlotTexts[i].text = $"Slot {i + 1}: Empty";
                newGameSlotTexts[i].text = $"Slot {i + 1}: Empty";
            }
        }
    }
    // 컨티뉴 게임 버튼
    public void OnContinueSlotClicked(int slotIndex)
    {
        string slotPath = userDataManager.path + $"slot_{slotIndex}.json"; // 각 슬롯마다 파일 경로 다르게 설정
        if (File.Exists(slotPath))
        {
            userDataManager.nowSlot = slotIndex;  // 슬롯 번호 설정
            userDataManager.LoadData(); // 데이터를 로드하고
            Debug.Log($"Slot {slotIndex + 1} 데이터 불러오기");
            // 플레이어 스탯 초기화
            globalPlayerState.NewPlayerSetting();
            // 로비 강화 스탯 세팅 (나중에 로딩 창으로 변경)
            lobbyUpGrade.ApplyUpgradeStats();

            UpdateSlotUI();
            SceneManager.LoadScene(_lobbyScene);
        }
        else
        {
            Debug.Log($"Slot {slotIndex + 1}에 데이터가 없습니다");
        }
    }
    // 뉴 게임 버튼 
    public void OnNewGameSlotClicked(int slotIndex)
    {
        // 각 슬롯마다 파일 경로 다르게 설정
        string slotPath = userDataManager.path + $"slot_{slotIndex}.json";
        selectedSlotIndex = slotIndex; // 선택된 슬롯 인덱스 저장
        if (File.Exists(slotPath))
        {
            // 데이터가 있는 경우 확인 UI 활성화
            confirmDeleteUI.SetActive(true);
            // 이재호 추가한 코드
            //confirmDeleteUI 활성화 시 슬롯 버튼에서 네비게이션 제거
            NaviBlock();
        }
        else
        {
            // 데이터가 없는 경우 바로 새 게임 시작
            StartNewGame(slotIndex);
        }
    }

    /// <summary>
    /// 이재호 코드 : 네비게이션 막아줘서 뒷배경 버튼 이동막아줌
    /// </summary>
    void NaviBlock()
    {
        Navigation navi = slot1.navigation;
        navi.selectOnUp = null;
        navi.selectOnDown = null;
        slot1.navigation = navi;
        slot2.navigation = navi;
        slot3.navigation = navi;
    }

    /// <summary>
    /// 이재호 코드 : 네비 다시 연결
    /// </summary>
    void NaviUnBlock()
    {
        Navigation navi = slot1.navigation;
        navi.selectOnUp = slot3;
        navi.selectOnDown = slot2;
        slot1.navigation = navi;

        Navigation navi2 = slot2.navigation;
        navi2.selectOnUp = slot1;
        navi2.selectOnDown = slot3;
        slot2.navigation = navi2;

        Navigation navi3 = slot3.navigation;
        navi3.selectOnUp = slot2;
        navi3.selectOnDown = slot1;
        slot3.navigation = navi3;
    }
    private void StartNewGame(int slotIndex)
    {
        userDataManager.nowSlot = slotIndex; // 슬롯 번호 설정
        userDataManager.DataClear(); // 데이터 초기화
        userDataManager.SaveData(); // 새 게임 저장
        Debug.Log($"새 게임 시작 {slotIndex + 1}");
        UpdateSlotUI(); // 슬롯 UI 갱신
        SceneManager.LoadScene(_lobbyScene);
    }

    public void OnConfirmDelete()
    {
        string slotPath = userDataManager.path + $"slot_{selectedSlotIndex}.json";
        if (File.Exists(slotPath))
        {
            File.Delete(slotPath); // 슬롯 데이터 삭제
            Debug.Log($"슬롯 {selectedSlotIndex + 1} 데이터 삭제 완료");
        }

        // 새 게임 시작
        StartNewGame(selectedSlotIndex);

        // 확인 UI 비활성화
        confirmDeleteUI.SetActive(false);
        NaviUnBlock();
    }
    // 확인 UI 취소 버튼 
    public void OnCancelDelete()
    {
        // 확인 UI 비활성화
        confirmDeleteUI.SetActive(false);
        NaviUnBlock();
    }
    // 전부 삭제 함수
    public void DeleteButtonClicked()
    {
        userDataManager.DeleteData(); // 모든 데이터 삭제
        UpdateSlotUI(); // 슬롯 UI 갱신
        Debug.Log("모든 게임 데이터가 삭제되었습니다.");
    }
}
