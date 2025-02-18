using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CushionAttack", menuName = "AdditionalEffect/PrevThrow/CushionAttack")]
public class CushionAttack : ThrowAdditional
{

    [Header("튕김 횟수")]
    [SerializeField] int _maxCushionCount;
    [Header("튕김 거리")]
    [SerializeField] float _CushionDistance;
    [Range(0, 100)]
    [Header("데미지 감소량 ( % )")]
    [SerializeField] float _ReductionDamage;
    [Space(10)]
    [SerializeField] private int _cushionCount;

    private List<TargetInfo> _targetList = new List<TargetInfo>();
    private TargetInfo _target;
    public override void Trigger()
    {
        if (_throwObject.CanAttack == false)
            return;
        // 최대 튕긴 횟수가 더 클때만 작동
        if (_cushionCount < _maxCushionCount - 1)
        {
            ProcessCushionAttack();
        }
    }

    private void ProcessCushionAttack()
    {
        _targetList.Clear();
        int hitCount = Physics.OverlapSphereNonAlloc(_throwObject.transform.position, _CushionDistance, Player.OverLapColliders, 1 << Layer.Monster, QueryTriggerInteraction.Ignore);

        // 주변에 몬스터가 있는지 스캔
        for (int i = 0; i < hitCount; i++)
        {
            Transform targetTransform = Player.OverLapColliders[i].transform;

            // 조건에 부합하면 해당 타겟을 거리와 함께 저장
            TargetInfo targetInfo = SetTargetInfo(targetTransform, Vector3.Distance(_throwObject.transform.position, targetTransform.position));

            _targetList.Add(targetInfo);
        }

        // 스캔에 실패했다면 바로 꺼짐
        if (_targetList.Count <= 1)
            return;

        // 타겟을 거리순으로 정렬
        _targetList.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        // 가장 가까운 적(맞은 적 제외)을 타겟으로 지정
        _target = _targetList[1];
        _cushionCount++;
        // 해당 적방향으로 공격
        _throwObject.IgnoreTargets.Clear();
        _throwObject.IgnoreTargets.Add(_targetList[0].transform.gameObject);
        //ThrowObject newObject = Instantiate(_throwObject, _throwObject.transform.position, _throwObject.transform.rotation);
        //_throwObject = newObject;

        ThrowObject newObject = Instantiate(DataContainer.GetThrowObject(_throwObject.Data.ID), _throwObject.transform.position, _throwObject.transform.rotation);
        newObject.Init(Player, _throwObject.CCType, _throwObject.IsBoom,_throwObject.PlayerDamage, _throwObject.ThrowAdditionals);
        newObject.IgnoreTargets = _throwObject.IgnoreTargets;

        // 클론 적용
        newObject.IsClone = true;
        _throwObject.AddChainList(newObject);
        

        // 데미지 감소
        newObject.ObjectDamage = (int)(newObject.ObjectDamage * (_ReductionDamage / 100f));
        newObject.PlayerDamage = (int)(newObject.PlayerDamage * (_ReductionDamage / 100f));

        // 적 위치 탐색
        // 공격
        Vector3 targetPos = new(_target.transform.position.x, _target.transform.position.y + 1.5f, _target.transform.position.z);
        newObject.transform.LookAt(targetPos);
        newObject.Shoot(Player.ThrowPower);
    }
}
