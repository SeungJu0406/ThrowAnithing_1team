using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Power PrevMelee", menuName = "Arm/AttackType/Power/PrevMelee")]
public class PowerMeleeAttack : ArmMeleeAttack
{
    [System.Serializable]
    struct ChargeStruct
    {
        [Header("차지 시간")]
        public float ChargeTime;
        [Header("추가 데미지")]
        public int Damage;
        [Header("공격 사거리")]
        public float AttackRange;
        [Header("전방 공격 각도")]
        [Range(0, 180)] public float AttackAngle;
        [Header("넉백 거리")]
        public float KnockBackRange;
        [Header("돌진 거리")]
        public float RushDistance;
        [HideInInspector] public float Stamina;

    }
    [System.Serializable]
    struct ChargeEffectStruct
    {
        public GameObject Charge;
        public GameObject Attack;
    }
    [System.Serializable]
    struct EffectStruct
    {
        public ChargeEffectStruct[] Charges;
        public GameObject Full;
    }
    [SerializeField] private ChargeStruct[] _charges;
    [SerializeField] private EffectStruct _effects;
 
    [SerializeField] private float RushSpeed;
    [SerializeField] private float _moveSpeedMultyPlier;
    [SerializeField] private float _autoAttackDelay;
    private float _staminaReduction => 1 - Model.StaminaReduction / 100;
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

    private GameObject _curChargeEffect;
    Coroutine _chargeRoutine;
    Coroutine _autoAttackRoutine;
    public override void Init(PlayerController player, ArmUnit arm)
    {
        base.Init(player, arm);
        for (int i = 0; i < _charges.Length; i++)
        {
            _charges[i].Damage = (int)Model.PowerMeleeAttack[i];
            _charges[i].Stamina = Model.MeleeAttackStamina[i];
        }
    }

    public override void Enter()
    {
        // 사용한 스테미나만큼 다시 회복
        Model.CurStamina += Model.MeleeAttackStamina[0] * _staminaReduction;
        // 최대 스테미나 차지량을 결정
        Model.MaxStaminaCharge = _charges[_charges.Length - 1].ChargeTime;
        // 위치 고정
        Player.Rb.velocity = Vector3.zero;
        // 스테미나 회복 멈춤
        Player.CanStaminaRecovery = false;
        // 공격방향 바라봄
        Player.LookAtAttackDir();
        // 애니메이션 실행
        View.SetTrigger(PlayerView.Parameter.PowerMelee);
        if (_chargeRoutine == null)
        {
            _chargeRoutine = CoroutineHandler.StartRoutine(ChargeRoutine());
        }
    }
    public override void Exit()
    {
        StopCoroutines();
        // 스테미나 다시 회복 시작
        Player.CanStaminaRecovery = true;

        // 암유닛 차지 이펙트 제거
        if (_curChargeEffect != null)
        {
            ObjectPool.ReturnPool(_curChargeEffect);
            _curChargeEffect = null;
        }

        // 초기화
        _curChargeTime = 0;
        _index = 0;
        _isAutoAttack = false;
        Player.IsInvincible = false;

        // 차지 사운드 종료
        Player.StopSFX();

    }
    public override void Update()
    {

    }

    public override void OnTrigger()
    {
        AttackMelee();
        // 암유닛 차지 이펙트 제거
        if (_curChargeEffect != null)
        {
            ObjectPool.ReturnPool(_curChargeEffect);
            _curChargeEffect = null;
        }
    }
    public override void EndCombo()
    {
        ChangeState(PlayerController.State.Idle);
    }

    IEnumerator ChargeRoutine()
    {
        _index = 0;

        // 차지 사운드
        Player.PlaySFX(Player.Sound.Power.Charge);
        while (true)
        {
            Move();
            ProcessCharge();

            if (InputKey.GetButtonUp(InputKey.Melee))
            {
                ChargeEnd();
                break;
            }
            yield return null;
        }
    }

    private void ProcessCharge()
    {
        if (_autoAttackRoutine != null)
            return;
        if (_isAutoAttack == false)
        {
            // 차지시간 계산
            _curChargeTime += Time.deltaTime * View.GetFloat(PlayerView.Parameter.AttackSpeed);
        }
        if (_charges.Length > _index + 1)
        {
            // 스테미나가 부족하면 차지 멈춤
            if (Model.CurStamina < _charges[_index + 1].Stamina)
            {
                ProcessAutoAttackTmer();
                return;
            }
            // 차지 시간이 다음 단계로 넘어갈 수 있을 때
            if (_curChargeTime > _charges[_index + 1].ChargeTime)
            {
                _index++;
                _autoAttackTime = 0;
                ShowArmEffect();
            }

        }
        else
        {
            ProcessAutoAttackTmer();
        }

    }
    public void AttackMelee()
    {
        // 자원소모 처리
        Model.CurStamina -= _charges[_index].Stamina * _staminaReduction;

        // 캐릭터 전방 조금 이동
        CoroutineHandler.StartRoutine(RushRoutine(transform.forward, _charges[_index].RushDistance));
        // 전방 앞에 있는 몬스터들을 확인하고 피격 진행
        // 1. 전방에 있는 몬스터 확인
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + Player.AttackHeight, transform.position.z);
        Vector3 attackPos = playerPos;
        int hitCount = Physics.OverlapSphereNonAlloc(attackPos, _charges[_index].AttackRange, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            Transform targetTransform = Player.OverLapColliders[i].transform;
            // 2. 각도 내에 있는지 확인
            Vector3 source = transform.position;
            source.y = 0;
            Vector3 destination = targetTransform.position;
            destination.y = 0;

            Vector3 targetDir = (destination - source).normalized;
            //float targetAngle = Vector3.Angle(transform.forward, targetDir); // 아크코사인 필요 (느리다)

            // 내적계산
            float targetAngle = Vector3.Dot(transform.forward, targetDir);

            targetAngle = Mathf.Acos(targetAngle) * Mathf.Rad2Deg;
         
            if (targetAngle > _charges[_index].AttackAngle * 0.5f)
                continue;


            int attackDamage = Player.GetFinalDamage(_charges[_index].Damage, out bool isCritical);
            Battle.TargetAttackWithDebuff(Player.OverLapColliders[i], isCritical, attackDamage, false);
            Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stiff);

            // 사운드
            SoundManager.PlaySFX(isCritical == true? Player.Sound.Hit.Critical: Player.Sound.Hit.Hit);

            if (_charges[_index].KnockBackRange > 0)
            {
                // 전방으로 밀기
                Player.DoKnockBack(targetTransform, transform.forward, _charges[_index].KnockBackRange);
                // 플레이어 중심 밀기
                //Player.DoKnockBack(targetTransform, transform, _charges[_index].KnockBackRange);
            }

            if (_index == 0)
                break;
        }

        ObjectPool.GetPool(_effects.Charges[_index].Attack, Player.MeleeAttackPoint.transform.position, transform.rotation, 2f);

    }
    public override void OnDrawGizmos()
    {
        if (Model == null)
            return;
        //거리
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + Player.AttackHeight, transform.position.z);
        Vector3 attackPos = playerPos;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, _charges[_index].AttackRange);

        //각도
        Vector3 rightDir = Quaternion.Euler(0, _charges[_index].AttackAngle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, _charges[_index].AttackAngle * -0.5f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + rightDir * _charges[_index].AttackRange);
        Gizmos.DrawLine(transform.position, transform.position + leftDir * _charges[_index].AttackRange);
    }

    // 차지시 암 이펙트 나타내기
    private void ShowArmEffect()
    {
        if (_curChargeEffect != null)
        {
            ObjectPool.ReturnPool(_curChargeEffect);
        }
        // 암유닛 이펙트
        _curChargeEffect = ObjectPool.GetPool(_effects.Charges[_index].Charge, Player.RightArmPoint);
    }

    private void ChargeEnd()
    {
        StopCoroutines();
        Player.IsInvincible = true;
        _autoAttackTime = 0;


        _chargeRoutine = null;
        // 공격방향 바라보기
        Player.LookAtAttackDir();
        // 애니메이션 실행
        View.SetTrigger(PlayerView.Parameter.ChargeEnd);

        // 차지 사운드 종료
        Player.StopSFX();
    }

    IEnumerator RushRoutine(Vector3 rushDir, float rushDistance)
    {
        // 풀차지시(돌진 가능할 때) 돌진 이펙트
        if (rushDistance > 0)
        {
            ObjectPool.GetPool(_effects.Full, Player.DashFrountPoint, 2f);
        }


        Vector3 originPos = transform.position;

        while (true)
        {
            if (Player.IsWall)
                break;

            if (Vector3.Distance(originPos, transform.position) >= rushDistance)
            {
                break;
            }

            transform.Translate(rushDir * Time.deltaTime * RushSpeed, Space.World);
            yield return null;
        }
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

    private void StopCoroutines()
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
