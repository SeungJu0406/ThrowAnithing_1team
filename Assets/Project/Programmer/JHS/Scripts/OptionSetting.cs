using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class OptionSetting : MonoBehaviour
{
    // 설정값 저장
    // 효과음 배경음 전체사운드
    [Range(0, 1f)]
    public float effectSound;
    [Range(0, 1f)]
    public float backgroundSound;
    [Range(0, 1f)]
    public float wholesound;

    // 화면 속도 감도 1~5
    [Range(0.01f, 3f)]
    public float cameraSpeed = 3;

    // 미니맵 온오프 기능 저장 1 = on , 0 = off
    public bool miniMapOnBool;
    public int miniMapOn;
    // 미니맵 고정 기능 저장 1 = on , 0 = off
    public bool miniMapFixBool;
    public int miniMapFix;

    // 게임 언어 추가 0 = korea, 1 = english
    public int language;

    private const string EffectSoundKey = "Option_EffectSound";
    private const string BackgroundSoundKey = "Option_BackgroundSound";
    private const string WholeSoundKey = "Option_WholeSound";
    private const string CameraSpeedKey = "Option_CameraSpeed";
    private const string MiniMapOnKey = "Option_MiniMapOn";
    private const string MiniMapFixKey = "Option_MiniMapFix";
    private const string LanguageKey = "Option_LanguageKey";

    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs 초기화 완료");
    }

    // 옵션 세팅 수치 세이브
    public void OptionSave()
    {
        if (miniMapOnBool == true)
            miniMapOn = 1;
        else miniMapOn = 0;
        if (miniMapFixBool == true)
            miniMapFix = 1;
        else miniMapFix = 0;
        PlayerPrefs.SetFloat(EffectSoundKey, effectSound);
        PlayerPrefs.SetFloat(BackgroundSoundKey, backgroundSound);
        PlayerPrefs.SetFloat(WholeSoundKey, wholesound);
        PlayerPrefs.SetFloat(CameraSpeedKey, cameraSpeed);
        PlayerPrefs.SetInt(MiniMapOnKey, miniMapOn);
        PlayerPrefs.SetInt(MiniMapFixKey, miniMapFix);
        PlayerPrefs.SetInt(LanguageKey, language);     
        PlayerPrefs.Save();
        Debug.Log("옵션 세팅 저장");
    }

    // 옵션 세팅 수치 로드
    public void OptionLoad()
    {
        if (!PlayerPrefs.HasKey(EffectSoundKey) || !PlayerPrefs.HasKey(BackgroundSoundKey) || !PlayerPrefs.HasKey(WholeSoundKey) || !PlayerPrefs.HasKey(CameraSpeedKey) 
            || !PlayerPrefs.HasKey(MiniMapOnKey) || !PlayerPrefs.HasKey(MiniMapFixKey) || !PlayerPrefs.HasKey(LanguageKey))
        {
            Debug.Log("기본 세팅 완료");
            OptionResetAll();
            return;
        }
        Debug.Log("옵션 세팅 불러오기");       
        effectSound = PlayerPrefs.GetFloat(EffectSoundKey);
        backgroundSound = PlayerPrefs.GetFloat(BackgroundSoundKey);
        wholesound = PlayerPrefs.GetFloat(WholeSoundKey);
        cameraSpeed = PlayerPrefs.GetFloat(CameraSpeedKey);
        miniMapOn = PlayerPrefs.GetInt(MiniMapOnKey);
        miniMapFix = PlayerPrefs.GetInt(MiniMapFixKey);
        language = PlayerPrefs.GetInt(LanguageKey);
        if (miniMapOn == 1)
            miniMapOnBool = true;
        else miniMapOnBool = false;
        if (miniMapFix == 1)
            miniMapFixBool = true;
        else miniMapFixBool = false;        
    }

    public void OptionReset(int value)
    {
        if (value == 1)
        {
            cameraSpeed = 3;
            miniMapOnBool = true;
            miniMapFixBool = true;
            language = 0;
        }
        else if (value == 2)
        {
            effectSound = 1f;
            backgroundSound = 1f;
            wholesound = 1f;
        }         
        OptionSave();
    }
    public void OptionResetAll()
    {
        cameraSpeed = 3;
        miniMapOnBool = true;
        miniMapFixBool = true;
        language = 0;
        effectSound = 1f;
        backgroundSound = 1f;
        wholesound = 1f;
        OptionSave();
    }
    public void MinimapOn()
    {
        miniMapOnBool = true;
        Debug.Log("미니맵 On");
    }

    public void MinimapOff()
    {
        miniMapOnBool = false;
        Debug.Log("미니맵 Off");
    }

    public void MiniMapFixOn()
    {
        miniMapFixBool = true;
        Debug.Log("미니맵픽스 On");
    }

    public void MiniMapFixOff()
    {
        miniMapFixBool = false;
        Debug.Log("미니맵픽스 Off");
    }
}
