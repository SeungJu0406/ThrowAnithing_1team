using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject.SpaceFighter;

[CreateAssetMenu(fileName = "Balance PrevSpecial", menuName = "Arm/AttackType/Balance/PrevSpecial")]
public class BalanceSpecialAttack : ArmSpecialAttack
{
    [System.Serializable]
    struct ChargeEffect
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
        public GameObject StepUpEffectPrefab;
        public GameObject ChargeEndEffect;
    }
    [System.Serializable]
    struct ChargeStruct
    {
        public float ChargeTime;
        public float ChargeMana;
        public int ObjectCount;
    }
    [System.Serializable]
    struct FirstStruct
    {
        public GameObject SpecialEffect;
        public GameObject BuffEffectPrefab;
        public GameObject BuffEndEffect;
        [HideInInspector]public GameObject BuffEffect;
        public float AttackSpeed;
        public float Duration;
    }
    [System.Serializable]
    struct SecondStruct
    {
        public SpecialObject SpecialObject;
        public float Damage;
        public float MiddleDamage;
        public float Range;
        public float MiddleRange;
    }
    [System.Serializable]
    struct ThirdStruct
    {
        public GameObject Effect;
        public GameObject BeforeEffect;
        public ElectricShockAdditonal ElectricShockOrigin;
        [HideInInspector] public ElectricShockAdditonal ElectricShock;
        public float AttackDelay;
        public Vector3 AttackOffset;
        public float Damage;
        public float MiddleDamage;
        public float Range;
        public float MiddleRange;
        public float ElectricShockDuration;
    }
    [SerializeField] private ChargeEffect _chargeEffect;
    [SerializeField] private ChargeStruct[] _charges;
    [SerializeField] private FirstStruct _first;
    [SerializeField] private SecondStruct _second;
    [SerializeField] private ThirdStruct _third;

    private List<Transform> MiddleHittargets = new List<Transform>();
    private float _maxChargeTime => _charges[_charges.Length - 1].ChargeTime;
    private float _maxChargeMana => _charges[_charges.Length - 1].ChargeMana;

    // 암유닛 캐싱
    private BalanceArm _balance => arm as BalanceArm;

    Coroutine _firstSpecialRoutine;

    Coroutine _checkManaUIRoutien;
    public override void Init(PlayerController player, ArmUnit arm)
    {
        base.Init(player, arm);
        View.Panel.SetChargingMpVarMaxValue(Model.MaxMana);
        for (int i = 0; i < _charges.Length; i++)
        {
            View.Panel.StepTexts[i].SetText(_charges[i].ObjectCount.GetText());
            View.Panel.SetChargingMpHandle(i, _charges[i].ChargeMana);
        }

        // 감전 효과 클론으로 제작(수치 변경하기 위함)
        _third.ElectricShock = Instantiate(_third.ElectricShockOrigin);
        _third.ElectricShock.Duration = _third.ElectricShockDuration;

        // 현재 차지 가능 마나 체크
        _checkManaUIRoutien = CoroutineHandler.StartRoutine(_checkManaUIRoutien, CheckManaUIRoutine());
    }

    private void OnDestroy()
    {
        _checkManaUIRoutien = CoroutineHandler.StopRoutine(_checkManaUIRoutien);
    }

    public override void Enter()
    {
        if (_isSpecialCharge == false
            && (Model.ThrowObjectStack.Count < _charges[_index].ObjectCount || Model.CurMana < Model.ManaConsumption[0]))
        {
            _isSpecialCharge = false;
            ChangeState(Player.PrevState);
            return;
        }

        if (_isSpecialCharge == false)
        {
            ChangeState(Player.PrevState);
            _isSpecialCharge = true;
            CoroutineHandler.StartRoutine(ChargeRoutine());
        }
        else
        {
            _index--;
            if (_index < 0)
            {
                Model.SpecialChargeGage = 0;
                _isSpecialCharge = false;
                ChangeState(Player.PrevState);
                return;
            }
            SelectSpecialAttack(_index);
        }
    }
    public override void Exit()
    {
        if (_isSpecialCharge == false)
        {

        }
        else
        {

            _isSpecialCharge = false;
        }
        _index = 0;
    }
    public override void OnTrigger()
    {
        SelectTrigger(_index);
    }
    public override void EndAnimation()
    {
        ChangeState(PlayerController.State.Idle);
    }
    IEnumerator ChargeRoutine()
    {
        // 차지 이펙트 생성
        _chargeEffect.Effect = ObjectPool.Get(_chargeEffect.EffectPrefab, transform);
        while (true)
        {
            ProcessCharge();
            if (InputKey.GetButtonUp(InputKey.Special))
            {
                ObjectPool.Return(_chargeEffect.Effect);

                ObjectPool.Get(_chargeEffect.ChargeEndEffect, transform, 2f);

                ChangeState(PlayerController.State.SpecialAttack);
                yield break;
            }
            yield return null;
        }
    }

    private void ProcessCharge()
    {        
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
                    _index++;
                    ObjectPool.Get(_chargeEffect.StepUpEffectPrefab, transform, 1f);
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

    private void SelectSpecialAttack(int index)
    {
        Rb.velocity = Vector3.zero;
        // 차지 사용량만큼 제거
        Model.CurMana -= _charges[_index].ChargeMana;
        Model.SpecialChargeGage = 0;
        // 사용한 오브젝트만큼 제거ㄴ
        for (int i = 0; i < _charges[_index].ObjectCount; i++)
        {
            Model.PopThrowObject();
        }
        switch (index)
        {
            case 0:
                ProcessFirstSpecial();
                break;
            case 1:
                ProcessSecondSpecial();
                break;
            case 2:
                ProcessThirdSpecial();
                break;
        }
    }

    private void SelectTrigger(int index)
    {
        switch (index)
        {
            case 0:
                break;
            case 1:
                ThrowSpecialObject();
                break;
            case 2:
                Bombard();
                break;
        }
    }


    /// <summary>
    /// 첫번째 특수
    /// </summary>
    private void ProcessFirstSpecial()
    {
        if (_firstSpecialRoutine != null)
        {
            CoroutineHandler.StopRoutine(_firstSpecialRoutine);
            _firstSpecialRoutine = null;

            Model.AttackSpeedMultiplier -= _first.AttackSpeed;
        }
        _firstSpecialRoutine = CoroutineHandler.StartRoutine(_firstSpecialRoutine, FirstSpecialBuffRoutine());

        // 딱히 애니메이션이 없어서 추가효과 트리거를 강제로 발동해야 할듯
        Player.TriggerPlayerAdditional();


        ObjectPool.Get(_first.SpecialEffect, Player.RightArmPoint, 2f);
        ObjectPool.Get(_first.BuffEffectPrefab, transform, _first.Duration);
        ChangeState(Player.PrevState);


    }
    IEnumerator FirstSpecialBuffRoutine()
    {
        Model.AttackSpeedMultiplier += _first.AttackSpeed;
        _balance.OnFirstSpecial = true;
        yield return _first.Duration.GetDelay();
        Model.AttackSpeedMultiplier -= _first.AttackSpeed;
        _balance.OnFirstSpecial = false;

        ObjectPool.Get(_first.BuffEndEffect, transform,3f);
        _firstSpecialRoutine = null;
    }
    /// <summary>
    /// 두번째 특수
    /// </summary>
    private void ProcessSecondSpecial()
    {
        Player.LookAtAttackDir();
        View.SetTrigger(PlayerView.Parameter.BalanceSpecial2);
    }
    /// <summary>
    /// 특수 투척물 던지기
    /// </summary>
    private void ThrowSpecialObject()
    {
        SpecialObject specialObject = ObjectPool.Get(_second.SpecialObject, _muzzlePoint.position, _muzzlePoint.rotation);
        specialObject.Init(Player, CrowdControlType.None, true, 0,Model.ThrowAdditionals);
        specialObject.InitSpecial(_second.Damage, _second.MiddleDamage, _second.Range, _second.MiddleRange);
        specialObject.Shoot(Player.ThrowPower);
    }

    /// <summary>
    /// 세번째 특수
    /// </summary>
    private void ProcessThirdSpecial()
    {
        Player.LookAtAttackDir();
        View.SetTrigger(PlayerView.Parameter.BalanceSpecial3);
    }
    /// <summary>
    /// 포격하기
    /// </summary>
    private void Bombard()
    {
        Player.LookAtAttackDir();
        CoroutineHandler.StartRoutine(BombardRoutine());
    }

    IEnumerator BombardRoutine()
    {
        Vector3 attackPos = new Vector3(
                transform.position.x + (Player.transform.forward.x * _third.AttackOffset.x),
                transform.position.y + 0.01f,
                transform.position.z + (Player.transform.forward.z * _third.AttackOffset.z));

        int layer = Layer.EveryThing;
        layer &= ~(Layer.Monster);

        if(Physics.Raycast(attackPos, Vector3.down, out RaycastHit hit,100f, Layer.EveryThing, QueryTriggerInteraction.Ignore))
        {
            attackPos = new Vector3(hit.point.x, hit.point.y + 0.01f, hit.point.z);
        }

        GameObject beforeEffect = ObjectPool.Get(_third.BeforeEffect, attackPos, Quaternion.identity);
        beforeEffect.transform.localScale *= _third.MiddleRange;

        yield return (_third.AttackDelay-0.3f).GetDelay();

        // 내려치는 사운드
        SoundManager.PlaySFX(Player.Sound.Balance.Special3Hit);
        yield return 0.3f.GetDelay();
        ObjectPool.Return(beforeEffect);
        // 루프 사운드
        SoundManager.PlaySFX(Player.Sound.Balance.Special3Loop);
        // 공격
        for (int i = 0; i < 8; i++) 
        {
            AttackBombard(attackPos);
            GameObject effect = ObjectPool.Get(_third.Effect, attackPos, _third.Effect.transform.rotation, 1f);       
            effect.transform.localScale *= _third.MiddleRange;
            yield return 0.25f.GetDelay();
        }

    }

    private void AttackBombard(Vector3 attackPos)
    {
        // 중앙 맞은 몬스터 걸러내기
        int hitCount = Physics.OverlapSphereNonAlloc(attackPos, _third.MiddleRange, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            MiddleHittargets.Add(Player.OverLapColliders[i].transform);
        }


        hitCount = Physics.OverlapSphereNonAlloc(attackPos, _third.Range, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            int finalDamage = 0;
            bool isCritical = false;
            // 중앙에 맞았을 경우
            if (MiddleHittargets.Contains(Player.OverLapColliders[i].transform))
            {
                finalDamage = Player.GetFinalDamage((int)_third.MiddleDamage, out isCritical);
            }
            else
            {
                finalDamage = Player.GetFinalDamage((int)_third.Damage, out isCritical);
            }

            Battle.TargetAttack(Player.OverLapColliders[i], isCritical, finalDamage);
            Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stun, 1f);
            Battle.TargetDebuff(Player.OverLapColliders[i], _third.ElectricShock);
        }
        MiddleHittargets.Clear();
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
