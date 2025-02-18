using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Power PrevSpecial", menuName = "Arm/AttackType/Power/PrevSpecial")]
public class PowerSpecialAttack : ArmSpecialAttack
{
    [System.Serializable]
    struct ChargeEffectStruct
    {
        public GameObject ChargeEffect;
    }
    [System.Serializable]
    struct EffectStruct
    {
        public ChargeEffectStruct[] Effects;
        public GameObject Attack;
        public GameObject FullCharge;
        public PowerObjectEffect EffectObject;
   
    }
    [System.Serializable]
    struct ChargeStruct
    {
        public float ChargeTime;
        public float ChargeMana;
        public int ObjectCount;
        public Vector3 AttackOffset;
        public float Radius;
        public int Damage;
        public float KnockBackDistance;
    }
    [SerializeField] private ChargeStruct[] _charges;
    [SerializeField] private GameObject _specialRange;
    [SerializeField] private float _moveSpeedMultyPlier;
    [SerializeField] private EffectStruct _effect;
    private float _maxChargeTime => _charges[_charges.Length - 1].ChargeTime;
    private float _maxChargeMana => _charges[_charges.Length - 1].ChargeMana;
    private PowerObjectEffect _effectObject;
    private GameObject _instanceSpecialRange;

    private GameObject _chargeEffect;
    private Vector3 _dropPos;
    Coroutine _chargeRoutine;


    Coroutine _checkManaUIRoutien;
    public override void Init(PlayerController player, ArmUnit arm)
    {
        base.Init(player,arm);
        View.Panel.SetChargingMpVarMaxValue(Model.MaxMana);
        for (int i = 0; i < _charges.Length; i++)
        {
            _charges[i].Damage = (int)Model.PowerSpecialAttack[i];
            //View.Panel.StepTexts[i].SetText(_charges[i].ObjectCount.GetText());
            _charges[i].ChargeMana = Model.ManaConsumption[i];
            View.Panel.SetChargingMpHandle(i, _charges[i].ChargeMana);
        }
        // 현재 차지 가능 마나 체크
        _checkManaUIRoutien = CoroutineHandler.StartRoutine(_checkManaUIRoutien, CheckManaUIRoutine());
    }

    private void OnDestroy()
    {
        _checkManaUIRoutien = CoroutineHandler.StopRoutine(_checkManaUIRoutien);
    }
    public override void Enter()
    {
        if (Model.ThrowObjectStack.Count < _charges[_index].ObjectCount || Model.CurMana < Model.ManaConsumption[0])
        {
            ChangeState(Player.PrevState);
            return;
        }
        Player.Rb.velocity = Vector3.zero;

        // 차징 모션 시작
        View.SetTrigger(PlayerView.Parameter.PowerSpecial);
        if (_chargeRoutine == null)
        {
            _chargeRoutine = CoroutineHandler.StartRoutine(ChargeRoutine());
        }
    }
    public override void Exit()
    {
        if (_chargeRoutine != null)
        {
            CoroutineHandler.StopRoutine(_chargeRoutine);
            _chargeRoutine = null;
        }


        #region 공격에 소환했던 그래픽 오브젝트 삭제
        if (_effectObject != null)
        {
            _effectObject.End();
            ObjectPool.Return(_effectObject);
            _effectObject = null;
        }
        if (_instanceSpecialRange != null)
        {
            ObjectPool.Return(_instanceSpecialRange);
            _instanceSpecialRange = null;
        }
        if (_chargeEffect != null)
        {
            ObjectPool.Return(_chargeEffect);
        }
        #endregion
        // 사운드 끄기
        Player.StopSFX();

        Model.SpecialChargeGage = 0;
        _index = 0;
        Player.IsInvincible = false;
    }
    public override void Update()
    {

    }
    public override void OnTrigger()
    {
        if (_chargeEffect != null)
        {
            ObjectPool.Return(_chargeEffect);
        }
        AttackSpecial();
    }
    public override void EndAnimation()
    {
        ChangeState(PlayerController.State.Idle);
    }
    IEnumerator ChargeRoutine()
    {
        // 손에 오브젝트 모이는 연출 이펙트 생성
        CreateSpecialObject();

        // 차지 사운드
        Player.PlaySFX(Player.Sound.Power.Charge);
        while (true)
        {
            Move();
            ProcessCharge();

            if (InputKey.GetButtonUp(InputKey.Special))
            {
                Model.SpecialChargeGage = 0;


                // 차지 사운드 종료
                if (_index != 0)
                {
                    _index--;
                    Player.LookAtAttackDir();
                    View.SetTrigger(PlayerView.Parameter.ChargeEnd);

                    Player.StopSFX();
                }
                else
                {
                    View.SetTrigger(PlayerView.Parameter.ChargeCancel);
                    ChangeState(PlayerController.State.Idle);
                }
                _chargeRoutine = null;
                // 캐릭터 임시 무적
                Player.IsInvincible = true;
                break;
            }
            yield return null;
        }
    }
    private void ProcessCharge()
    {
        // 플레이어 공격방향 계속 바라보기
        Player.LookAtAttackDir();
        // 공격범위 위치 잡기
        if (_index > 0)
        {
            _dropPos = new Vector3(
                transform.position.x + (Player.transform.forward.x * _charges[_index - 1].AttackOffset.x),
                transform.position.y + 0.01f,
                transform.position.z + (Player.transform.forward.z * _charges[_index - 1].AttackOffset.z));
            _instanceSpecialRange.transform.position = _dropPos;
        }
        // 오른손 효과 손 따라다니기
        if (_effectObject != null)
        {
            _effectObject.transform.position = Player.RightArmPoint.position;
            _effectObject.transform.rotation = Quaternion.LookRotation(transform.forward);
        }

        // 차지시간 계산
        Model.SpecialChargeGage += Time.deltaTime * 100 / _maxChargeTime;
        // 인덱스가 배열 크기보다 작을떄만
        if (_index < _charges.Length)
        {
            // 소모 오브젝트가 부족한 경우
            if (Model.ThrowObjectStack.Count >= _charges[_index].ObjectCount)
            {
                // 차지 시간이 다음 단계 차징 조건시간을 넘긴 경우
                if (Model.SpecialChargeGage >= _charges[_index].ChargeMana)
                {
                    // 오른손 그래픽
                    CreateSpecialObject();

                    CreateSpecialRange();

                    CreateChargeEffect();
                    _index++;
                }
                // 현재 특수자원량보다 차지량이 더 많은 경우
                else if (Model.SpecialChargeGage > Model.CurMana)
                {
                    Model.SpecialChargeGage = Model.CurMana;
                }
            }
            else
            {
                Model.SpecialChargeGage = _charges[_index - 1].ChargeMana;
            }
        }
        else
        {
            Model.SpecialChargeGage = _charges[_index - 1].ChargeMana;
        }
    }

    private void CreateSpecialObject()
    {
        if(_effectObject == null)
        {
            _effectObject = ObjectPool.Get(_effect.EffectObject, Player.RightArmPoint.position, _effect.EffectObject.transform.rotation);
            _effectObject.transform.SetParent(Player.RightArmPoint);
        }
        else
        {
            _effectObject.Next();
        }
    }
    private void CreateSpecialRange()
    {
        //if (_effectObject != null)
        //    Destroy(_instanceSpecialRange);
        // 공격범위 그래픽
        _dropPos = new Vector3(
              transform.position.x + (Player.CamareArm.forward.x * _charges[_index].AttackOffset.x),
              transform.position.y + 0.01f,
              transform.position.z + (Player.CamareArm.forward.z * _charges[_index].AttackOffset.z));


        if (_instanceSpecialRange == null)
        {
            _instanceSpecialRange = ObjectPool.Get(_specialRange, _dropPos, Quaternion.identity);
        }
        // 크기 조정
        _instanceSpecialRange.transform.localScale = new Vector3(
            _charges[_index].Radius * 2,
            _instanceSpecialRange.transform.localScale.y,
            _charges[_index].Radius * 2);
    }

    private void CreateChargeEffect()
    {
        if (_chargeEffect != null)
        {
            ObjectPool.Return(_chargeEffect);
        }
        _chargeEffect = ObjectPool.Get(_effect.Effects[_index].ChargeEffect, Player.RightArmPoint);
    }
    private void AttackSpecial()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(_dropPos, _charges[_index].Radius, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            int finalDamage = Player.GetFinalDamage(_charges[_index].Damage, out bool isCritical);
            // 범위 내 적에게 데미지

            // 데미지 주기
            Battle.TargetAttackWithDebuff(Player.OverLapColliders[i], isCritical, finalDamage,   false);
            // 풀차지 시
            if(_index == _charges.Length - 1)
            {
                Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stun, 1);
            }
            else
            {
                Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stiff);
            }


            // 넉백 가능하면 넉백
            if (_charges[_index].KnockBackDistance > 0)
                Player.DoKnockBack(Player.OverLapColliders[i].transform, transform, _charges[_index].KnockBackDistance);
        }
        // 차지 사용량만큼 제거
        Model.CurMana -= _charges[_index].ChargeMana;
        // 사용한 오브젝트만큼 제거
        for (int i = 0; i < _charges[_index].ObjectCount; i++)
        {
            Model.PopThrowObject();
        }

        
        // 타격지점 이펙트 생성
        GameObject attackEffect = ObjectPool.Get(_effect.Attack, _dropPos, Quaternion.identity, 2f);
        attackEffect.transform.localScale = Util.GetPos(_charges[_index].Radius/2);
        // 풀차지는 추가 이펙트 생성
        if (_index == _charges.Length - 1) 
        {
            ObjectPool.Get(_effect.FullCharge, _dropPos, Quaternion.identity, 2f);
        }
        ObjectPool.Return(_instanceSpecialRange);
        ObjectPool.Return(_effectObject,0.5f);

        // 공격 사운드
        SoundManager.PlaySFX(Player.Sound.Power.SpecialHit);

        _instanceSpecialRange = null;
    }

    private void Move()
    {
        if (Player.MoveDir == Vector3.zero)
            return;
        Player.LookAtMoveDir();

        // 플레이어 이동
        // 지상에 있고 벽에 부딪히지 않은 상태에서만 이동
        if (Player.IsGround == false && Player.IsWall == true)
            return;
        Vector3 originRb = Rb.velocity;
        Vector3 velocityDir = transform.forward * (Model.MoveSpeed * _moveSpeedMultyPlier);
        Rb.velocity = new Vector3(velocityDir.x, originRb.y, velocityDir.z);
    }

    IEnumerator CheckManaUIRoutine()
    {
        while (true)
        {
            View.Panel.Step[0].SetActive(Model.CurMana >= _charges[0].ChargeMana && Model.CurThrowables >= _charges[0].ObjectCount);
            View.Panel.Step[1].SetActive(Model.CurMana >= _charges[1].ChargeMana && Model.CurThrowables >= _charges[1].ObjectCount);
            View.Panel.Step[2].SetActive(Model.CurMana >= _charges[2].ChargeMana && Model.CurThrowables >= _charges[2].ObjectCount);
            yield return 0.1f.GetDelay();
        }
    }
}
