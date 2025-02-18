/*using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Depth2
{
    gameplay,
    sound,
    notDepth2 = 3
}
//Todo: 설정창 열리면 시간 멈추게 하는거 추가

public class Main_Option : MainScene
{
    public PausePanel Panel;

    //옵션 1Depth
    Button[] depth1;

    Button gamePlay;
    Button language;
    Button sound;
    Button input;
    Button exit;

    public int optionCheck;


    //옵션 Depth2 바인딩
    GameObject gameplayPannel;
    GameObject languagePannel;
    GameObject soundPannel;
    GameObject inputPannel;

    public int depth1_cur;

    GameObject optionTitle;

    //옵션 Depth2 체크용 변수
    protected Depth2 depth2_cur = Depth2.notDepth2;

    protected GameObject gameplayOnOff;
    protected GameObject soundOnOff;

    [Header("메인화면인경우 체크")]
    [SerializeField] protected bool _isMain;

    void Start()
    {
        Init();

    }


    private void Update()
    {
        if (menuCo == null)
        {
            menuCo = StartCoroutine(Depth1_Select());
        }
        if (!gameplayOnOff.activeSelf && !soundOnOff.activeSelf)
        {
            OptionTitle();
            SelectedEnter();
        }
    }

    private void OnDisable()
    {
        menuCo = null;
    }

    void OptionTitle()
    {

        switch (depth1_cur)
        {
            case 0:
                optionTitle.GetComponent<TMP_Text>().text = "게임플레이";
                break;
            case 1:
                optionTitle.GetComponent<TMP_Text>().text = "소리";
                break;

            case 2:
                optionTitle.GetComponent<TMP_Text>().text = "조작키";
                break;

            case 3:
                optionTitle.GetComponent<TMP_Text>().text = "";
                break;
        }

        OptionD2Show();

    }

    // Comment 커서 위치에 따라 패널 온오프
    void OptionD2Show()
    {
        if (depth1_cur == 0)
            gameplayPannel.SetActive(true);
        else
            gameplayPannel.SetActive(false);

        if (depth1_cur == 1)
            soundPannel.SetActive(true);
        else
            soundPannel.SetActive(false);

        if (depth1_cur == 2)
            inputPannel.SetActive(true);
        else
            inputPannel.SetActive(false);


    }
    private IEnumerator Depth1_Select()
    {

        if (!gameplayOnOff.activeSelf && !soundOnOff.activeSelf)
        {
            float y = -InputKey.GetAxisRaw(InputKey.Vertical);
            if (y == 0)
                yield return null;
            else
                yield return inputDelay.GetRealTimeDelay();




            depth1_cur += (int)y;

            if (depth1_cur == depth1.Length)
            {
                depth1_cur = 0;
                depth1[depth1.Length - 1].GetComponent<TMP_Text>().color = Color.white;
                depth1[depth1_cur].GetComponent<TMP_Text>().color = new Color(1, 0.5f, 0);
                yield return null;
            }

            if (depth1_cur == -1)
            {
                depth1_cur = depth1.Length - 1;
                depth1[0].GetComponent<TMP_Text>().color = Color.white;
                depth1[depth1_cur].GetComponent<TMP_Text>().color = new Color(1, 0.5f, 0);
                yield return null;
            }

            ButtonReset();

            depth1[depth1_cur].GetComponent<TMP_Text>().color = new Color(1, 0.5f, 0);     
        }
        else
            yield return null;
        menuCo = null;

    }

    void ButtonReset()
    {
        for (int i = 0; i < depth1.Length; i++)
        {
            depth1[i].GetComponent<TMP_Text>().color = Color.white;
        }
    }

    void SelectedEnter()
    {
        if (InputKey.GetButtonDown(InputKey.PrevInteraction))
        {
            switch (depth1_cur)
            {
                case 0:
                    GameplayButton();
                    break;

                case 1:
                    SoundButton();
                    break;

                case 2:
                    //Comment : 조작키 설명
                    InputButton();
                    break;

                case 3:
                    ExitButtonOp();
                    break;
            }
        }

        if (InputKey.GetButtonDown(InputKey.Cancel))
        {
            gameObject.SetActive(false);
            Debug.Log("옵션 화면 나가기");
        }
    }
    void PannelOnOff()
    {
        gameplayPannel.SetActive(true);
        gameplayPannel.SetActive(false);

        soundPannel.SetActive(true);
        soundPannel.SetActive(false);

        inputPannel.SetActive(true);
        inputPannel.SetActive(false);
    }

    public void GameplayButton()
    {
        if (optionCheck == 0)
        {
            optionTitle.GetComponent<TMP_Text>().text = "게임플레이";
            optionCheck = 2;
            gameplayOnOff.SetActive(true);
            gameplayPannel.SetActive(true);
            soundPannel.SetActive(false);
            inputPannel.SetActive(false);
            ButtonReset();
        }
        optionCheck--;
    }

    public void SoundButton()
    {
        if (optionCheck == 0)
        {
            optionTitle.GetComponent<TMP_Text>().text = "소리";
            optionCheck = 2;
            soundOnOff.SetActive(true);
            gameplayPannel.SetActive(false);
            soundPannel.SetActive(true);
            inputPannel.SetActive(false);
            ButtonReset();
        }
        optionCheck--;
    }

    public void InputButton()
    {
        optionTitle.GetComponent<TMP_Text>().text = "조작키";
        gameplayPannel.SetActive(false);
        soundPannel.SetActive(false);
        inputPannel.SetActive(true);
    }

    public void ExitButtonOp()
    {
        optionTitle.GetComponent<TMP_Text>().text = "";
        if (_isMain)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Panel.ChangeBundle(PausePanel.Bundle.Pause);
        }
   
    }

    private void Init()
    {

        depth1 = new Button[4];

        depth1[0] = gamePlay = GetUI<Button>("GamePlay");
        depth1[1] = sound = GetUI<Button>("Sound");
        depth1[2] = input = GetUI<Button>("input");
        depth1[3] = exit = GetUI<Button>("Exit");

        gameplayPannel = GetUI("GamePlayPackage");
        soundPannel = GetUI("SoundPackage");
        inputPannel = GetUI("InputImage");

        gameplayOnOff = GetUI("GameplayOnOff");
        soundOnOff = GetUI("SoundOnOff");
        depth1_cur = 0;

        optionTitle = GetUI("optionTitle");

    }
}
*/