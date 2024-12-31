using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CameraSpeedSlider : MonoBehaviour
{
    
    [SerializeField] private Slider cameraSpeedSlider;

    [Inject]
    private OptionSetting setting;

    private void Start()
    {
        // 초기값 설정 
        cameraSpeedSlider = GetComponent<Slider>();

        // json으로 당겨온 속도 값으로 변경
        cameraSpeedSlider.value = setting.cameraSpeed;
    }

    // 슬라이더를 수정하고 나올때 

    public void SettingCameraSpeed(float value)
    {
        value = cameraSpeedSlider.value;
        setting.cameraSpeed = value;
    }
}
