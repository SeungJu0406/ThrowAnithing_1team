using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Result : BaseUI
{
    [Inject]
    PlayerData playerData;

    //플레이 타임
    TMP_Text timer;
    // 클리어 라운드 수
    TMP_Text round;
    // 죽인 좀비 수
    TMP_Text kill;
    // 획득한 라디언트 포인트
    TMP_Text rp;
    private void Awake()
    {
        //Bind();
        //InitInputManager();
    }

    void Update()
    {
        if (InputKey.GetButtonDown(InputKey.Interaction))
            LoadingToBase.LoadScene(SceneName.LobbyScene);
    }


    void Init()
    {
        // 연동 후 내용 채워야함
        // 프로토타입에선 제외, 추후 사용할 수 있으니 남겨두겠음
        timer = GetUI<TMP_Text>("Timer");
        round = GetUI<TMP_Text>("StageText");
        kill = GetUI<TMP_Text>("KillText");
        rp = GetUI<TMP_Text>("RpText");

    }
}
