using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : BaseUI
{
    public TMP_Text ObjectCount => GetUI<TMP_Text>("ObjectCountText");

    public Slider HpBar => GetUI<Slider>("HpBar");
    public Slider StaminaBar => GetUI<Slider>("StaminaBar");
    public Slider MpBar => GetUI<Slider>("MpBar");
    public Slider ChargingMpBar => GetUI<Slider>("ChargingMpBar");
    public Slider ChanrgeStaminaBar => GetUI<Slider>("ChargeStaminaBar");
    [HideInInspector]public List<TMP_Text> StepTexts = new List<TMP_Text>(3);
    [HideInInspector]public List<Slider> StepHandle = new List<Slider>(3);
    [HideInInspector] public List<GameObject> Step = new List<GameObject>(3);
    private void Awake()
    {
        Bind();
        Init();
    }

    private void Init()
    {
        StepTexts.Add(GetUI<TMP_Text>("1StepText"));
        StepTexts.Add(GetUI<TMP_Text>("2StepText"));
        StepTexts.Add(GetUI<TMP_Text>("3StepText"));

        StepHandle.Add(GetUI<Slider>("1StepHandle"));
        StepHandle.Add(GetUI<Slider>("2StepHandle"));
        StepHandle.Add(GetUI<Slider>("3StepHandle"));

        Step.Add(GetUI("1Step"));
        Step.Add(GetUI("2Step"));
        Step.Add(GetUI("3Step"));
    }

    public void BarValueController(Slider bar,float curValue,float maxValue)
    {
        float per;
        per = curValue / maxValue;
        bar.value = per;
    }

    public void SetChargingMpVarMaxValue(float value)
    {
        ChargingMpBar.maxValue = value;
        foreach(Slider slider in StepHandle)
        {
            slider.maxValue = value;
        }
    }
    public void SetChargingMpHandle(int index, float value)
    {
        StepHandle[index].value = value;
    }
}
