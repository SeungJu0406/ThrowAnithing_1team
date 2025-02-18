using UnityEngine;
[CreateAssetMenu(fileName = "SplitThrowable", menuName = "AdditionalEffect/PrevThrow/SplitThrowable")]
public class SplitThrowableAdditional : ThrowAdditional
{

    [Header("감소 피해량(%)")]
    [SerializeField] private float _reductionDamage;
    [Header("기존 오브젝트 대비 분열 오브젝트 크기(%)")]
    [SerializeField] private float _decreaseSize;

    public override void Trigger()
    {
        // 공격할수없으면 예외처리
        if (_throwObject.CanAttack == false)
            return;

        // 클론도 예외처리
        if (_throwObject.IsClone == true)
            return;


        SplitObject();
    }

    private void SplitObject()
    {

        TargetInfo nearTarget = FindNearTarget();


        // 두개 생성
        // 하나는 왼쪽 발사 하나는 오른쪽 발사
        for (int i = 0; i < 2; i++)
        {
            // 생성
            ThrowObject newObject = Instantiate(DataContainer.GetThrowObject(_throwObject.Data.ID), _throwObject.transform.position, _throwObject.transform.rotation);
            // 값 설정
            newObject.Init(Player, _throwObject.CCType, _throwObject.IsBoom,_throwObject.PlayerDamage,_throwObject.ThrowAdditionals);

            newObject.ObjectDamage = (int)(newObject.ObjectDamage * (_reductionDamage / 100f));
            newObject.PlayerDamage = (int)(newObject.PlayerDamage * (_reductionDamage / 100f));

            // 사이즈 조절
            Vector3 newObjScale = newObject.transform.localScale;
            newObject.transform.localScale *= _decreaseSize / 100;
            // 무시 대상 선정(가장 가까운 적, 이미 맞은 적)
            newObject.IgnoreTargets.Add(nearTarget.transform.gameObject);

            // 클론 지정
            newObject.IsClone = true;
            _throwObject.AddChainList(newObject);

            // 좌우 분열 
            float splitAngle = i == 0 ? 90 : -90;
            Vector3 newObjEuler = newObject.transform.eulerAngles;
            newObject.transform.rotation = Quaternion.Euler(newObjEuler.x, newObjEuler.y + splitAngle, newObjEuler.z);

            newObject.Shoot(Player.ThrowPower);

        }

    }
}
