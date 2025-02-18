using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtensionDrain", menuName = "AdditionalEffect/Player/ExtensionDrain")]
public class ExtensionDrainAddtional : PlayerAdditional
{
    [Header("드레인 범위 증가량(%)")]
    [SerializeField] private float _increaseDrainDistance;

    public override void Enter()
    {
        Model.DrainDistanceMultyPlier += _increaseDrainDistance;
    }

    public override void Exit() 
    {
        Model.DrainDistanceMultyPlier -= _increaseDrainDistance;
    }

}
