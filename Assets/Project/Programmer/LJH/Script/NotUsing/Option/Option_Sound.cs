/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Option_Sound : Main_Option
{
    [Inject]
    OptionSetting setting;

    struct ButtonStruct
    {
        public GameObject Button;
        public Slider Bar;
        public TMP_Text Text;
    }
    List<ButtonStruct> buttonStructs = new List<ButtonStruct>();
    GameObject[,] buttons;

    float preTotal;
    float newTotal;
    float defaultTotal;

    float preBgm;
    float newBgm;
    float defaultBgm;

    float preEffect;
    float newEffect;
    float defaultEffect;

    int _curIndex;
    void Start()
    {
        Init();
    }

    void Update()
    {
        AudioVolumeCotroller();

        if (soundOnOff.activeSelf)
        {
            if (menuCo == null)
            {
                menuCo = StartCoroutine(Sound_Select());
            }

            //Todo : 수정 필요
            if (InputKey.GetButtonDown(InputKey.PrevInteraction))
            {
                if (buttonStructs[_curIndex].Button == GetUI("CancelButton_sound"))
                {
                    CancelButton();
                }
                else if (buttonStructs[_curIndex].Button == GetUI("AcceptButton_sound"))
                {
                    AcceptButton();
                }
                else if (buttonStructs[_curIndex].Button == GetUI("DefaultButton_sound"))
                {
                    DefaultButton();
                }
                //sound_Ver = 0;
                //sound_Ho = 1;
            }

        }
        else
        {
            _curIndex = 0;
        }
    }

    private void OnDisable()
    {
        menuCo = null;
    }

    private IEnumerator Sound_Select()
    {
        
        float x = InputKey.GetAxisRaw(InputKey.Horizontal);
        float y = InputKey.GetAxisRaw(InputKey.Vertical);

        ButtonStruct curButton = buttonStructs[_curIndex];
        for(int i = 0;  i < buttonStructs.Count; i++)
        {
            if (buttonStructs[i].Button == curButton.Button)
            {
                buttonStructs[i].Text.color = new Color(1, 0.5f, 0);
            }
            else
            {
                buttonStructs[i].Text.color = Color.white;
            }
        }
        if (buttonStructs[_curIndex].Bar != null)
        {
            // 아래 버튼 눌렀을 때
            if (y < 0)
            {
                _curIndex++;
                if (_curIndex >= buttonStructs.Count)
                {
                    _curIndex = buttonStructs.Count - 1;
                }
            }
            else if (y > 0)
            {
                _curIndex--;
                if (_curIndex < 0)
                {
                    _curIndex = 0;
                }
            }
            // 오른쪽 키 눌렀을 때
            if (x > 0)
            {
                buttonStructs[_curIndex].Bar.value += x * 10f;
            }
            else if (x < 0)
            {
                buttonStructs[_curIndex].Bar.value -= -x * 10f;
            }
        }
        // Bar 없을때
        else
        {
            if( x > 0)
            {
                _curIndex++;
                if (_curIndex >= buttonStructs.Count)
                {
                    _curIndex = buttonStructs.Count - 1;
                }
            }
            else if(x < 0)
            {
                _curIndex--;
                if (_curIndex < 3)
                {
                    _curIndex = 3;
                }
            }   
            // 위 누르면 2번 인덱스(슬라이드 버튼) 으로
            if( y > 0)
            {
                _curIndex = 2;
            }
        }

        // 입력없으면 프레임마다
        if (x == 0 && y == 0)
        {
            yield return null;
        }
        else
        {
            yield return inputDelay.GetRealTimeDelay();
        }
        menuCo = null;
    }

    public void AudioVolumeCotroller()
    {
        VolumeListCotroller(AudioManager.instance.bgmList, setting.backgroundSound, setting.wholesound);
        VolumeListCotroller(AudioManager.instance.effectList, setting.effectSound, setting.wholesound);

    }

    // AudioVolumeController를 통해 오디오 매니저의 오디오 볼륨 조절
    public void VolumeListCotroller(List<AudioSource> audioList, float volume, float totalVolume)
    {
        for (int i = 0; i < audioList.Count; i++)
        {
            audioList[i].volume = volume * 0.01f;
        }
    }
    public void AcceptButton()
    {
        VolumeCheck();
        setting.wholesound = newTotal;
        setting.backgroundSound = newBgm;
        setting.effectSound = newEffect;

        preTotal = setting.wholesound;
        preBgm = setting.backgroundSound;
        preEffect = setting.effectSound;

        //Todo : depth1으로 복귀
        soundOnOff.SetActive(false);
        UnActiveAllText();
    }

    void VolumeCheck()
    {
        newTotal = setting.wholesound;
        newBgm = setting.backgroundSound;
        newEffect = setting.effectSound;
    }

    public void CancelButton()
    {
        setting.wholesound = preTotal;
        setting.backgroundSound = preBgm;
        setting.effectSound = preEffect;

        //Todo : depth1으로 복귀
        soundOnOff.SetActive(false);
        UnActiveAllText();
    }

    public void DefaultButton()
    {
        // defaultPopUp.SetActive(true);
        // Todo: 팝업 과정 거쳐야함

        setting.wholesound = defaultTotal;
        setting.backgroundSound = defaultBgm;
        setting.effectSound = defaultEffect;

        //Todo : depth1으로 복귀
        soundOnOff.SetActive(false);
        UnActiveAllText();
    }

    private void UnActiveAllText()
    {
        foreach(ButtonStruct buttonStruct in buttonStructs)
        {
            buttonStruct.Text.color = Color.white;
        }
    }

    private void Init()
    {
        buttons = new GameObject[4, 4];

        buttons[1, 0] = GetUI("Total Volume");
        buttons[2, 0] = GetUI("BGM Volume");
        buttons[3, 0] = GetUI("Effect Volume");
        buttons[0, 1] = GetUI("AcceptButton_sound");
        buttons[0, 2] = GetUI("CancelButton_sound");
        buttons[0, 3] = GetUI("DefaultButton_sound");

        buttonStructs.Add(GetButtonStruct(GetUI("Total Volume"), GetUI<Slider>("TotalVolumeBar"), GetUI<TMP_Text>("Total Volume")));
        buttonStructs.Add(GetButtonStruct(GetUI("BGM Volume"), GetUI<Slider>("BGMVolumeBar"), GetUI<TMP_Text>("BGM Volume")));
        buttonStructs.Add(GetButtonStruct(GetUI("Effect Volume"), GetUI<Slider>("EffectVolumeBar"), GetUI<TMP_Text>("Effect Volume")));
        buttonStructs.Add (GetButtonStruct(GetUI("AcceptButton_sound"), null,GetUI<TMP_Text>("AcceptButton_sound")));
        buttonStructs.Add(GetButtonStruct(GetUI("CancelButton_sound"), null, GetUI<TMP_Text>("CancelButton_sound")));
        buttonStructs.Add(GetButtonStruct(GetUI("DefaultButton_sound"), null, GetUI<TMP_Text>("DefaultButton_sound")));

        soundOnOff = GetUI("SoundOnOff");

        defaultTotal = 100f;
        defaultBgm = 100f;
        defaultEffect = 100f;

        preTotal = setting.wholesound;
        preBgm = setting.backgroundSound;
        preEffect = setting.effectSound;

    }

    private ButtonStruct GetButtonStruct(GameObject button, Slider bar,TMP_Text text)
    {
        ButtonStruct newButtonStruct = new ButtonStruct();
        newButtonStruct.Button= button;
        newButtonStruct.Bar= bar;
        newButtonStruct.Text= text;
        return newButtonStruct;
    }
}
*/