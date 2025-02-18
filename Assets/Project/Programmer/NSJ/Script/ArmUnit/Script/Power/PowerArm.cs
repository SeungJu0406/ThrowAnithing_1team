using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerArm", menuName = "Arm/Power")]
public class PowerArm : ArmUnit
{
    public override void Init(PlayerController player)
    {
        base.Init(player);
        Model.NowWeapon = GlobalGameData.AmWeapon.Power;
    }
}
