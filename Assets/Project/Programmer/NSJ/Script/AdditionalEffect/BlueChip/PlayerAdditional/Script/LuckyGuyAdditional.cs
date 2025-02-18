using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LuckyGuy", menuName = "AdditionalEffect/Player/LuckyGuy")]
public class LuckyGuyAdditional : PlayerAdditional
{
    [Header("¹ÝÈ¯ È®·ü(%)")]
    [Range(0, 100)] 
    [SerializeField]private float _probability;


    public override void Trigger()
    {
        if (CurState != PlayerController.State.Dash)
            return;

        ReturnStamina();
    }

    private void ReturnStamina()
    {
        if(Random.Range(0,100) <= _probability)
        {
            Model.CurStamina += Model.DashStamina;
        }
    }


}
