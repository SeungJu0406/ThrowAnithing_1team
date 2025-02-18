using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Oblivion", menuName = "AdditionalEffect/Player/Oblivion")]
public class OblivionAddtional : PlayerAdditional
{
    [Header("확률(%)")]
    [Range(0, 100)]
    [SerializeField] private float _probability;
    public override void Trigger()
    {
        if (Player.CurState != PlayerController.State.SpecialAttack)
            return;

        RecoverMana();
    }

    public void RecoverMana()
    {
        // 확률적으로 쓴 마나만큼 다시 회복
        if(Random.Range(0,100) <= _probability)
        {
            Model.CurMana += Model.ManaConsumption[Model.ChargeStep];
        }
    }
}
