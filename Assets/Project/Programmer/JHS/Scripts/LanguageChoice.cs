using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LanguageChoce : MonoBehaviour
{
    [SerializeField] public TMP_Dropdown langaugeDropdown;

    [Inject]
    public OptionSetting setting;

    private void Awake()
    {
        // langaugeDropdown 값이 변경될 때 이벤트 연결
        CancellLanguage();
        langaugeDropdown.onValueChanged.AddListener(ChangeLanguage);
    }

    private void ChangeLanguage(int value)
    {
        // langaugeDropdown.value 값을 setting.language에 전달
        setting.language = value;
    }

    public void ReturnLanguage()
    {
        langaugeDropdown.value = 0;
    }

    public void CancellLanguage()
    {
        langaugeDropdown.value = setting.language;
    }

    private void OnDestroy()
    {
        // 이벤트 연결 해제
        langaugeDropdown.onValueChanged.RemoveListener(ChangeLanguage);
    }
}
