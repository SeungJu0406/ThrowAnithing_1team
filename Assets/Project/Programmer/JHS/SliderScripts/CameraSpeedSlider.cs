using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CameraSpeedSlider : MonoBehaviour
{
    [SerializeField]
    private Slider cameraSpeedSlider;

    [Inject]
    public OptionSetting setting;

    private void Awake()
    {
        if (cameraSpeedSlider == null)
        {
            cameraSpeedSlider = GetComponent<Slider>();
        }
    }
    private void Start()
    {
        cameraSpeedSlider.minValue = 0.01f;
        cameraSpeedSlider.maxValue = 3f;
        cameraSpeedSlider.value = Mathf.Clamp(setting.cameraSpeed, cameraSpeedSlider.minValue, cameraSpeedSlider.maxValue);

        // 슬라이더 값 변경 시 SettingCameraSpeed 메서드 호출
        cameraSpeedSlider.onValueChanged.AddListener(SettingCameraSpeed);
    }
    public void SettingCameraSpeed(float value)
    {
        setting.cameraSpeed = value;
    }

    // 비활성화 시 이벤트 해제
    private void OnDestroy()
    {
        // 이벤트 리스너 해제
        if (cameraSpeedSlider != null)
        {
            cameraSpeedSlider.onValueChanged.RemoveListener(SettingCameraSpeed);
        }
    }

    // 리셋, 취소 버튼 누를떄 ui 갱신
    public void ResetCameraSpeed()
    {
        cameraSpeedSlider.value = Mathf.Clamp(setting.cameraSpeed, cameraSpeedSlider.minValue, cameraSpeedSlider.maxValue);
        cameraSpeedSlider.SetValueWithoutNotify(cameraSpeedSlider.value);
    }
}
