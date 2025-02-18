using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ButtonActionInstaller : MonoBehaviour
{

    // OptionSave()
    [SerializeField] public Button saveButton;
    // OptionLode()
    [SerializeField] public Button cancelButton;
    // OptionReset()
    [SerializeField] public Button resetButton;

    // OptionSave()
    [SerializeField] public Button saveSoundButton;
    // OptionLode()
    [SerializeField] public Button cancelSoundButton;
    // OptionReset()
    [SerializeField] public Button resetSoundButton;

    // 미니맵 클릭이벤트 추가
    [SerializeField] public Button MinimapOn;
    [SerializeField] public Button MinimapOff;
    [SerializeField] public Button MiniMapFixOn;
    [SerializeField] public Button MiniMapFixOff;

    [SerializeField] public CameraSpeedSlider SensitivityBar;
    [SerializeField] public TotalVolumeBar totalVolumeBar;
    [SerializeField] public BackGroundVolume backGroundVolume;
    [SerializeField] public EffectVolume effectVolume;
    [SerializeField] public LanguageChoce languageChoce;


    [Inject]
    public OptionSetting setting;

    private void Start()
    {
        saveButton.onClick.AddListener(setting.OptionSave);
        cancelButton.onClick.AddListener(() =>
        {
            setting.OptionLoad();
            SensitivityBar.ResetCameraSpeed();
            //languageChoce.CancellLanguage();
            MinimapOn.gameObject.SetActive(setting.miniMapOnBool);
            MiniMapFixOn.gameObject.SetActive(setting.miniMapFixBool);
        });
        resetButton.onClick.AddListener(() =>
        {
            setting.OptionReset(1); // OptionReset1 실행
            SensitivityBar.ResetCameraSpeed();
            //languageChoce.ReturnLanguage();
            MinimapOn.gameObject.SetActive(setting.miniMapOnBool);
            MiniMapFixOn.gameObject.SetActive(setting.miniMapFixBool);
        });
        saveSoundButton.onClick.AddListener(setting.OptionSave);
        cancelSoundButton.onClick.AddListener(() =>
        {
            setting.OptionLoad();
            totalVolumeBar.ResetTotalVolume();
            backGroundVolume.ResetBackGroundVolume();
            effectVolume.ResetTotalVolume();          
        });
        resetSoundButton.onClick.AddListener(() =>
        {
            setting.OptionReset(2); // OptionReset2 실행
            totalVolumeBar.ResetTotalVolume();
            backGroundVolume.ResetBackGroundVolume();
            effectVolume.ResetTotalVolume();
        });

        MinimapOn.onClick.AddListener(setting.MinimapOff);
        MinimapOff.onClick.AddListener(setting.MinimapOn);
        MiniMapFixOn.onClick.AddListener(setting.MiniMapFixOff);
        MiniMapFixOff.onClick.AddListener(setting.MiniMapFixOn);
    }
}
