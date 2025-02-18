using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleDamage", menuName = "AdditionalEffect/PrevThrow/DoubleDamage")]
public class DoubleDamageBuff : ThrowAdditional
{
    public float DamageMultyplier;



    public override void Trigger()
    {
        Debug.Log($" 데미지 들어가기 전{_throwObject.DamageMultyPlier}");
        _throwObject.DamageMultyPlier += DamageMultyplier;
        Debug.Log($" 데미지 들어간 후{_throwObject.DamageMultyPlier}");
    }
}
