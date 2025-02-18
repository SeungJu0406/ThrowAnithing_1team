using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Runner", menuName = "AdditionalEffect/Player/Runner")]
public class RunnerAdditional : PlayerAdditional
{
    [Header("스테미나 감소량(%)")]
    [Range(0, 100)][SerializeField] private float _staminaReduction;

    public override void Enter()
    {
        Model.StaminaReduction += _staminaReduction;
    }

    public override void Exit()
    {
        Model.StaminaReduction -= _staminaReduction;
    }
}
