using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TotalVolumeBar : MonoBehaviour
{
    [SerializeField]
    private Slider totalVolumeSlider;

    [Inject]
    public OptionSetting setting;

    private void Awake()
    {
        if (totalVolumeSlider == null)
        {
            totalVolumeSlider = GetComponent<Slider>();
        }
    }
    private void Start()
    {
        totalVolumeSlider.minValue = 0f;
        totalVolumeSlider.maxValue = 1f;
        totalVolumeSlider.value = Mathf.Clamp(setting.wholesound, totalVolumeSlider.minValue, totalVolumeSlider.maxValue);

        
        totalVolumeSlider.onValueChanged.AddListener(SettingTotalVolume);
    }
    public void SettingTotalVolume(float value)
    {
        setting.wholesound = value;
        SoundManager.SetVolumeMaster(value * 100);
    }

    private void OnDestroy()
    {
        // 이벤트 리스너 해제
        if (totalVolumeSlider != null)
        {
            totalVolumeSlider.onValueChanged.RemoveListener(SettingTotalVolume);
        }
    }

    // 리셋 버튼 누를떄 ui 갱신
    public void ResetTotalVolume()
    {
        totalVolumeSlider.value = Mathf.Clamp(setting.wholesound, totalVolumeSlider.minValue, totalVolumeSlider.maxValue);
        totalVolumeSlider.SetValueWithoutNotify(totalVolumeSlider.value);
    }
}
