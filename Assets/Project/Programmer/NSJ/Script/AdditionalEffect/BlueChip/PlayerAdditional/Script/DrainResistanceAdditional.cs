using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrainResistance", menuName = "AdditionalEffect/Player/DrainResistance")]
public class DrainResistanceAdditional : PlayerAdditional
{
    [Header("피해 감소량(%)")]
    [Range(0, 100)][SerializeField] private float _resistanceAmount;

    public override void EnterState()
    {
        if (CurState != PlayerController.State.Drain)
            return;

        Model.DamageReduction += _resistanceAmount;
    }

    public override void ExitState()
    {
        if (CurState != PlayerController.State.Drain)
            return;

        Model.DamageReduction -= _resistanceAmount;
    }
}
