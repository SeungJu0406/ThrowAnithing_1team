using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Power Throw", menuName = "Arm/AttackType/Power/Throw")]
public class PowerThrowAttack : ArmThrowAttack
{
    [System.Serializable]
    struct ChargeStruct
    {
        public float ChargeTime;
        public int ObjectCount;
        public int Damage;
        public float KnockBackDistance;
    }
    [SerializeField] private ChargeStruct[] _charges;
    [SerializeField] private float _autoAttackDelay;
    private float m_curChargeTime;
    float _autoAttackTime;
    bool _isAutoAttack;
    private float _curChargeTime
    {
        get { return m_curChargeTime; }
        set
        {
            m_curChargeTime = value;
            Model.CurStaminaCharge = m_curChargeTime;
            View.SetFloat(PlayerView.Parameter.Charge, m_curChargeTime);
        }
    }
    Coroutine _chargeRoutine;
    Coroutine _autoAttackRoutine;
    public override void Init(PlayerController player)
    {
        base.Init(player);
        for (int i = 0; i < _charges.Length; i++)
        {
            _charges[i].Damage = (int)Model.PowerThrowAttack[i];
        }
    }
    public override void Enter()
    {
        Player.Rb.velocity = Vector3.zero;
        Model.MaxStaminaCharge = _charges[_charges.Length - 1].ChargeTime;

        Player.LookAtCameraFoward();
        View.SetTrigger(PlayerView.Parameter.PowerThrow);
        if (_chargeRoutine == null)
        {
            _chargeRoutine = CoroutineHandler.StartRoutine(ChargeRoutine());
        }
    }
    public override void Exit()
    {
        StopCoroutine();
        _curChargeTime = 0;
        _index = 0;
        _isAutoAttack = false;
        // 캐릭터 임시 무적
        Player.IsInvincible = false;
    }
    public override void Update()
    {

    }

    public override void OnTrigger()
    {
        ThrowObject();
    }
    public override void EndCombo()
    {
        ChangeState(PlayerController.State.Idle);
    }

    IEnumerator ChargeRoutine()
    {
        _index = 0;
        while (true)
        {
            ProcessCharge();

            // 차지 해제 시 던지는 애니메이션 실행
            if (InputKey.GetButtonUp(InputKey.Throw))
            {
                ChargeEnd();
                break;
            }
            yield return null;
        }
    }
    private void ThrowObject()
    {
        int throwObjectID = Model.ThrowObjectStack.Count > 0 && _index > 0 ? Model.PopThrowObject().ID : 0;

        ThrowObject throwObject = Instantiate(DataContainer.GetThrowObject(throwObjectID), _muzzlePoint.position, _muzzlePoint.rotation);
        throwObject.Init(Player, (int)Model.PowerThrowAttack[_index],Model.ThrowAdditionals);

        // 넉백가능하면 넉백
        if (_charges[_index].KnockBackDistance > 0)
        {
            throwObject.KnockBackDistance = _charges[_index].KnockBackDistance;
        }
        UseThrowObject(_charges[_index].ObjectCount);
        throwObject.Shoot(Player.ThrowPower);
    }

    private void ProcessCharge()
    {
        if (_autoAttackRoutine != null)
            return;

        if (_isAutoAttack ==false)
        {
            // 차지시간 계산
            _curChargeTime += Time.deltaTime * View.GetFloat(PlayerView.Parameter.AttackSpeed);
        }
        if (_charges.Length > _index + 1)
        {
            // 소모 오브젝트가 부족하면 차지 멈춤
            if (Model.ThrowObjectStack.Count <= _charges[_index].ObjectCount)
            {
                //_curChargeTime = _charges[_index].ChargeTime;
                ProcessAutoAttackTmer();
                return;
            }
            // 차지 시간이 다음 단계로 넘어 갈 수 있을 때
            if (_curChargeTime > _charges[_index + 1].ChargeTime)
            {
                _autoAttackTime = 0;
                _index++;
            }
        }
        else
        {
            //_curChargeTime = _charges[_index].ChargeTime + 0.01f;
            ProcessAutoAttackTmer();
        }
    }

    private void UseThrowObject(int count)
    {
        for (int i = 0; i < count - 1; i++)
        {
            if (Model.ThrowObjectStack.Count > 0)
            {
                Model.PopThrowObject();
            }
        }
    }

    private void ChargeEnd()
    {
        StopCoroutine();
        _autoAttackTime = 0;

        Player.LookAtAttackDir();
        View.SetTrigger(PlayerView.Parameter.ChargeEnd);
        _chargeRoutine = null;
        // 캐릭터 임시 무적
        Player.IsInvincible = true;
    }

    private void StopCoroutine()
    {
        if (_chargeRoutine != null)
        {
            CoroutineHandler.StopRoutine(_chargeRoutine);
            _chargeRoutine = null;
        }
        if (_autoAttackRoutine != null)
        {
            CoroutineHandler.StopRoutine(_autoAttackRoutine);
            _autoAttackRoutine = null;
        }
    }

    private void ProcessAutoAttackTmer()
    {
        _isAutoAttack = true;
        _autoAttackTime += Time.deltaTime;
        if (_autoAttackTime > _autoAttackDelay)
        {
            ChargeEnd();
        }
    }
}
