using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DrainStamina", menuName = "AdditionalEffect/Player/DrainStamina")]
public class DrainStaminaAdditional : PlayerAdditional
{
    [Header("감소 스테미나(%)")]
    [SerializeField] private float _decreaseStamina;

    // 감소시킨 양 캐싱
    private float _decreaseAmount;
    public override void Enter()
    {
        // 감소시킬 소모 스테미나양 계산
        _decreaseAmount = Model.DrainStamina * (_decreaseStamina / 100);
        // 적용
        Model.DrainStamina -= _decreaseAmount;
    }
    public override void Exit()
    {
        // 감소시킨 만큼 다시 증가
        Model.DrainStamina += _decreaseAmount;
    }
}
