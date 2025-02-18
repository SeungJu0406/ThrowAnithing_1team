using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sniper", menuName = "AdditionalEffect/PrevThrow/Sniper")]
public class SniperAdditional : ThrowAdditional
{
    [Header("최대 데미지 증가량(%)")]
    [SerializeField] private float _maxIncreaseDamage;
    [Header("최대 데미지까지 거리")]
    [SerializeField] private float _maxDistance;

    Vector3 _startPos;
    Vector3 _endPos;

    public override void Enter()
    {
        // 시작지점 캐싱
        _startPos = _throwObject.transform.position;
    }

    public override void Trigger()
    {
        // 공격 가능할때만
        if (_throwObject.CanAttack == false)
            return;
        // 끝지점 캐싱
        _endPos = _throwObject.transform.position;
        AmplifyDamage();
    }

    private void AmplifyDamage()
    {
        // 시작지점, 끝지점 거리 측정
        float distance = Vector3.Distance(_startPos, _endPos);
        // 최대거리와 비교 계산 1 초과일 경우 1로 고정
        float distancePerMax = distance / _maxDistance <= 1 ? distance / _maxDistance : 1 ;
        // 증가해야할 데미지 계산
        float increaseDamage = _maxIncreaseDamage * distancePerMax;
 
        _throwObject.DamageMultyPlier += increaseDamage;
    }
}
