using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ApprenticeWizard", menuName = "AdditionalEffect/Player/ApprenticeWizard")]
public class ApprenticeWizardAddtional : PlayerAdditional
{
    [Header("회복 마나량(%)")]
    [SerializeField] private float _recoveryAmount;

    private float _prevMana;

    public override void LateUpdate()
    {
        if(_prevMana > Model.CurMana)
        {
            RecoverMana();
        }

        _prevMana = Model.CurMana;
    }

    private void RecoverMana()
    { 
        float usedMana = _prevMana - Model.CurMana;

        float recoveryMana = usedMana * (_recoveryAmount / 100);
        Model.CurMana += recoveryMana;  
    }
}
