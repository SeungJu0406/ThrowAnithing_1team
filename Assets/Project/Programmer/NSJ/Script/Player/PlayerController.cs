
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(BattleSystem))]
public class PlayerController : MonoBehaviour, IHit, IHeal
{
    [SerializeField] public PlayerEffectData Effect;
    [SerializeField] public PlayerSoundData Sound;
    [Inject]
    [HideInInspector] public OptionSetting setting;
    [HideInInspector] public PlayerModel Model;
    [HideInInspector] public PlayerView View;
    [HideInInspector] public Collider Collider;
    [HideInInspector] public Rigidbody Rb;
    [HideInInspector] public BattleSystem Battle;
    [HideInInspector] public AudioSource Audio;
    public enum State
    {
        Idle,
        Run,
        MeleeAttack,
        ThrowAttack,
        Jump,
        DoubleJump,
        Fall,
        DoubleJumpFall,
        JumpAttack,
        JumpDown,
        Dash,
        Drain,
        SpecialAttack,
        Hit,
        Dead,
        Size
    }

    private PlayerState[] _states = new PlayerState[(int)State.Size];
    public State CurState;
    public State PrevState;

    #region 이벤트
    /// <summary>
    /// int : 데미지, bool : 고정데미지? CrowdControlType: CC기 타입 , 반환형 int 최종데미지
    /// </summary>
    public event Func<int, bool, int> OnPlayerHitFuncEvent;
    public event UnityAction<CrowdControlType> OnPlayerCCHitEvent;
    public event UnityAction OnPlayerHitActionEvent;
    public event UnityAction OnPlayerDieEvent;
    public event UnityAction<ThrowObject, bool> OnThrowObjectResult;
    #endregion
    #region 플레이어 포인트 관련 필드
    [System.Serializable]
    public struct PlayerPointStruct
    {
        public Transform RightArmPoint;
        public Transform LeftArmPoint;
        public Transform MeleeAttackPoint;
        public Transform DashArmPoint;
        public Transform DashFrontPoint;

    }
    [SerializeField] private PlayerPointStruct _points;
    public Transform RightArmPoint { get { return _points.RightArmPoint; } }
    public Transform LeftArmPoint { get { return _points.LeftArmPoint; } }
    public Transform MeleeAttackPoint { get { return _points.MeleeAttackPoint; } }
    public Transform DashArmPoint { get { return _points.DashArmPoint; } }
    public Transform DashFrountPoint { get { return _points.DashFrontPoint; } }
    #endregion
    #region 공격 관련 필드
    [System.Serializable]
    struct AttackStruct
    {
        public float AttackHeight;
        public float ThrowPower;
        public Transform MuzzlePoint;
        [HideInInspector] public bool IsTargetHolding;
        [HideInInspector] public bool IsTargetToggle;
        [HideInInspector] public Vector3 TargetPos;
    }
    [Header("공격 관련 필드")]
    [SerializeField] private AttackStruct _attackStruct;
    public Transform MuzzletPoint { get { return _attackStruct.MuzzlePoint; } set { _attackStruct.MuzzlePoint = value; } }
    public float AttackHeight { get { return _attackStruct.AttackHeight; } set { _attackStruct.AttackHeight = value; } }
    public float ThrowPower { get { return _attackStruct.ThrowPower; } set { _attackStruct.ThrowPower = value; } }
    public bool IsTargetHolding { get { return _attackStruct.IsTargetHolding; } set { _attackStruct.IsTargetHolding = value; } }
    public bool IsTargetToggle { get { return _attackStruct.IsTargetToggle; } set { _attackStruct.IsTargetToggle = value; } }
    public Vector3 TargetPos { get { return _attackStruct.TargetPos; } set { _attackStruct.TargetPos = value; } }
    #endregion
    #region Camera 관련 필드
    /// <summary>
    /// 카메라 관련
    /// </summary>
    [System.Serializable]
    struct CameraStruct
    {
        public Transform CamaraArm;
        public Transform CameraPos;
        [Range(0f, 50f)] public float CameraRotateAngle;
        public float MouseRotateSpeed;
        public float StickRotateSpeed;
        public PlayerCameraHold CameraHolder;
        public bool IsVerticalCameraMove;
    }
    [Header("카메라 관련 필드")]
    [SerializeField] private CameraStruct _cameraStruct;
    public Transform CamareArm { get { return _cameraStruct.CamaraArm; } set { _cameraStruct.CamaraArm = value; } }
    private Transform _cameraPos { get { return _cameraStruct.CameraPos; } set { _cameraStruct.CameraPos = value; } }
    private float _cameraRotateAngle { get { return _cameraStruct.CameraRotateAngle; } set { _cameraStruct.CameraRotateAngle = value; } }
    private float _mouseRotateSpeed { get { return _cameraStruct.MouseRotateSpeed; } set { _cameraStruct.MouseRotateSpeed = value; } }
    private float _stickRotateSpeed { get { return _cameraStruct.StickRotateSpeed; } set { _cameraStruct.StickRotateSpeed = value; } }
    private PlayerCameraHold _cameraHolder { get { return _cameraStruct.CameraHolder; } set { _cameraStruct.CameraHolder = value; } }
    public bool IsVerticalCameraMove { get { return _cameraStruct.IsVerticalCameraMove; } set { _cameraStruct.IsVerticalCameraMove = value; } }
    #endregion
    #region 감지 관련 필드
    [System.Serializable]
    struct CheckStruct
    {
        public Transform GroundCheckPos;
        [Range(0, 1)] public float SlopeAngle;
        public WallCheckStruct WallCheckPos;
        public float WallCheckDistance;
        [Space(10)]
        public bool IsGround; // 지면 접촉 여부
        public bool IsNearGround;
        public bool IsWall; // 벽 접촉 여부
        public bool CanClimbSlope; // 오를 수 있는 경사면 각도 인지 체크
    }
    [System.Serializable]
    public struct WallCheckStruct
    {
        public Transform Head;
        public Transform Foot;
    }
    [SerializeField] private CheckStruct _checkStruct;
    private Transform _groundCheckPos => _checkStruct.GroundCheckPos;
    public WallCheckStruct WallCheckPos => _checkStruct.WallCheckPos;

    private float _wallCheckDistance { get { return _checkStruct.WallCheckDistance; } set { _checkStruct.WallCheckDistance = value; } }
    private float _slopeAngle { get { return _checkStruct.SlopeAngle; } set { _checkStruct.SlopeAngle = value; } }

    #endregion
    #region 테스트 관련 필드
    [System.Serializable]
    public struct TestStruct
    {
        public bool IsAttackForward;
    }
    [Header("테스트 관련 필드")]
    [SerializeField] private TestStruct _testStruct;
    public bool IsAttackFoward { get { return _testStruct.IsAttackForward; } }
    #endregion
    #region 조건체크 Bool 필드
    [System.Serializable]
    public struct BoolField
    {
        public bool IsDoubleJump; // 더블점프 했음?
        public bool IsJumpAttack; // 점프공격 했음?
        public bool IsInvincible; // 무적상태임?
        public bool IsShield; // 쉴드가 존재하는지?
        public bool IsHit; // 맞음?
        public bool IsDead; // 죽음?
        public bool IsStaminaCool; // 스테미나 사용 후 쿨타임인지?
        public bool CanStaminaRecovery; // 스테미나 회복 할 수 있는지?
        public bool CanOperate; // 조작할 수 있는지?
    }
    [SerializeField] private BoolField _boolField;
    public bool IsDoubleJump { get { return _boolField.IsDoubleJump; } set { _boolField.IsDoubleJump = value; } }
    public bool IsJumpAttack { get { return _boolField.IsJumpAttack; } set { _boolField.IsJumpAttack = value; } }
    public bool IsInvincible { get { return _boolField.IsInvincible; } set { _boolField.IsInvincible = value; } }
    public bool IsShield { get { return _boolField.IsShield; } set { _boolField.IsShield = value; } }
    public bool IsHit { get { return _boolField.IsHit; } set { _boolField.IsHit = value; } }
    public bool IsDead { get { return _boolField.IsDead; } set { _boolField.IsDead = value; } }
    public bool IsStaminaCool { get { return _boolField.IsStaminaCool; } set { _boolField.IsStaminaCool = value; } }
    public bool CanStaminaRecovery { get { return _boolField.CanStaminaRecovery; } set { _boolField.CanStaminaRecovery = value; } }
    public bool CanOperate { get { return _boolField.CanOperate; } set { _boolField.CanOperate = value; TriggerCantOperate(); } }
    #endregion

    public bool IsGround { get { return _checkStruct.IsGround; } set { _checkStruct.IsGround = value; } }// 지면 접촉 여부
    public bool IsNearGround { get { return _checkStruct.IsNearGround; } set { _checkStruct.IsNearGround = value; } }
    public bool IsWall { get { return _checkStruct.IsWall; } set { _checkStruct.IsWall = value; } } // 벽 접촉 여부
    public bool CanClimbSlope { get { return _checkStruct.CanClimbSlope; } set { _checkStruct.CanClimbSlope = value; } } // 오를 수 있는 경사면 각도 인지 체크
    public int MaxHp { get { return Model.MaxHp; } set { Model.MaxHp = value; } }

    [HideInInspector] public Collider[] OverLapColliders = new Collider[100];
    [HideInInspector] public Vector3 MoveDir;

    [HideInInspector] public bool IsMouseVisible;
    Vector2 _mouseDir;
    Vector2 _stickDir;

    bool _isAudioPlay = true;
    Quaternion _defaultMuzzlePointRot;
    private void Awake()
    {

    }

    private void Start()
    {
        Init();
        InitUIEvent();
        SubscribeEvents();
        StartRoutine();
        InitAdditionnal();
        ChangeArmUnit(Model.NowWeapon);
        //Camera.main.transform.SetParent(_cameraPos, true);

        InputKey.SetActionMap(InputType.GAMEPLAY);



        EnterState(CurState);
    }

    private void OnDisable()
    {
        ExitPlayerAdditional();
        ExiteState(CurState);
        DeleteArmUnit();
    }

    private void Update()
    {
        ControlMousePointer();

        if (CanOperate == false)
            return;

        if (Time.timeScale == 0)
            return;

        _states[(int)CurState].Update();

        CheckAnyState();
        RotateCameraMouse();
        RotateCameraStick();
        ChackInput();
        UpdatePlayerAdditional();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;

        _states[(int)CurState].FixedUpdate();
        CheckGround();
        CheckWall();
        CheckIsNearGround();
        FixedPlayerAdditional();
    }
    void LateUpdate()
    {
        LatePlayerAdditional();
    }

    private void OnDrawGizmos()
    {
        if (_states[(int)CurState] == null)
            return;
        _states[(int)CurState].OnDrawGizmos();

        DrawCheckGround();
        DrawWallCheck();
        DrawIsNearGround();
    }

    /// <summary>
    /// 상태 변경
    /// </summary>
    public void ChangeState(State state)
    {
        if (CurState == State.Dead) return;



        // 스테미나 쓰는 상태
        if (_states[(int)state].UseStamina == true)
        {
            // 스테미나 소모량
            float staminaConsumption = _states[(int)state].StaminaAmount * (1 - Model.StaminaReduction / 100);

            // 사용할수 있음?(최소 스테미나)
            if (Model.CurStamina < staminaConsumption)
                return;
            // 사용가능하면 스테미나 깎음
            Model.CurStamina -= staminaConsumption;
        }

        ExiteState(CurState);
        PrevState = CurState;
        CurState = state;
        EnterState(CurState);
    }
    private void EnterState(State state)
    {
        _states[(int)state].Enter();
        EnterStatePlayerAdditional();
    }
    private void ExiteState(State state)
    {
        _states[(int)state].Exit();
        ExitStatePlayerAdditional();
    }
    /// <summary>
    /// 힐 받기
    /// </summary>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public int TakeHeal(int healAmount)
    {
        Model.CurHp += healAmount;

        // TODO : 힐 이펙트


        return Model.CurHp;
    }

    /// <summary>
    /// 데미지 받기
    /// </summary>
    public int TakeDamage(int damage, bool isIgnoreDef)
    {
        int hitDamage = (int)OnPlayerHitFuncEvent?.Invoke(damage, isIgnoreDef);
        OnPlayerHitActionEvent?.Invoke();
        return hitDamage;
    }

    public void TakeCrowdControl(CrowdControlType type, float time)
    {
        OnPlayerCCHitEvent?.Invoke(type);
    }
    /// <summary>
    /// 사망
    /// </summary>
    public void Die()
    {
        OnPlayerDieEvent?.Invoke();
    }
    #region 피해 흡혈

    /// <summary>
    /// 피해 흡혈(모델 피흡 만큼)
    /// </summary>
    public void DrainLife(int damage)
    {
        int drainAmount = (int)(damage * Model.DrainLife / 100);
        Model.CurHp += drainAmount;

        if (drainAmount > 0)
            ObjectPool.Get(Effect.LifeDrain, Battle.HitPoint.position.GetRandomPos(0.5f), transform.rotation, 1f);
    }
    /// <summary>
    /// 피해 흡혈(모델 피흡 + 추가 피흡)
    /// </summary>
    public void DrainLife(int damage, float additionalDrainLife)
    {
        int drainAmount = (int)(damage * (Model.DrainLife + additionalDrainLife) / 100);
        Model.CurHp += drainAmount;

        if (drainAmount > 0)
            ObjectPool.Get(Effect.LifeDrain, Battle.HitPoint.position.GetRandomPos(0.5f), transform.rotation, 1f);
    }

    #endregion
    #region 플레이어 방향 처리
    public void LookAtAttackDir()
    {
        if (IsTargetHolding == false && IsTargetToggle == false)
            LookAtCameraFoward();
        else
            LookAtTargetDir(TargetPos);
    }

    /// <summary>
    /// 카메라 방향으로 플레이어 방향 전환
    /// </summary>
    public void LookAtCameraFoward()
    {
        // 카메라 방향으로 플레이어가 바라보게
        Quaternion cameraRot = Quaternion.Euler(0, CamareArm.eulerAngles.y, 0);
        transform.rotation = cameraRot;
        // 카메라는 다시 로컬 기준 전방 방향
        if (CamareArm.parent != null)
        {
            CamareArm.localRotation = Quaternion.Euler(CamareArm.localRotation.eulerAngles.x, 0, 0);
        }
        MuzzletPoint.localRotation = _defaultMuzzlePointRot;
    }

    /// <summary>
    /// 입력한 방향을 플레이어가 바라봄
    /// </summary>
    /// <param name="moveDir"></param>
    public void LookAtMoveDir()
    {
        // 카메라 방향으로 플레이어가 바라보게
        Quaternion cameraRot = Quaternion.Euler(0, CamareArm.eulerAngles.y, 0);
        transform.rotation = cameraRot;
        // 카메라는 다시 로컬 기준 전방 방향
        if (CamareArm.parent != null)
        {
            // 카메라 흔들림 버그 잡아주는 코드
            CamareArm.localPosition = new Vector3(0, CamareArm.localPosition.y, 0);
            CamareArm.localRotation = Quaternion.Euler(CamareArm.localRotation.eulerAngles.x, 0, 0);
        }

        Quaternion cameraTempRot = CamareArm.rotation;

        // 입력한 방향쪽을 플레이어가 바라봄
        Vector3 dir = transform.forward * MoveDir.z + transform.right * MoveDir.x;
        if (dir == Vector3.zero)
        {
            if (CurState == State.Run)
                return;
            else
            {
                dir = transform.forward;
            }
        }
        transform.rotation = Quaternion.LookRotation(dir);
        CamareArm.rotation = cameraTempRot;
    }

    /// <summary>
    /// 타겟 방향을 플레이어가 바라봄
    /// </summary>
    public void LookAtTargetDir(Vector3 targetPos)
    {
        // 카메라 방향으로 플레이어가 바라보게
        Quaternion cameraRot = Quaternion.Euler(0, CamareArm.eulerAngles.y, 0);
        transform.rotation = cameraRot;
        // 카메라는 다시 로컬 기준 전방 방향
        if (CamareArm.parent != null)
        {
            // 카메라 흔들림 버그 잡아주는 코드
            CamareArm.localPosition = new Vector3(0, CamareArm.localPosition.y, 0);
            CamareArm.localRotation = Quaternion.Euler(CamareArm.localRotation.eulerAngles.x, 0, 0);
        }

        Quaternion cameraTempRot = CamareArm.rotation;
        targetPos = new Vector3(targetPos.x, targetPos.y + 2f, targetPos.z);
        // 입력한 방향쪽을 플레이어가 바라봄
        transform.LookAt(targetPos);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        CamareArm.rotation = cameraTempRot;
        MuzzletPoint.LookAt(targetPos);
    }

    /// <summary>
    ///  플레이어가 보고있는 방향으로 물리량 바꾸기
    /// </summary>
    public void ChangeVelocityPlayerFoward()
    {
        Vector3 tempVelocity = transform.forward * Rb.velocity.magnitude; // x,z 값의 물리량만 계산
        Rb.velocity = tempVelocity; // 대입
    }
    #endregion
    #region 투척오브젝트 추가
    /// <summary>
    /// 오브젝트 줍기
    /// </summary>
    public void AddThrowObject(ThrowObject throwObject)
    {

        if (Model.CurThrowables < Model.MaxThrowables)
        {
            Model.PushThrowObject(DataContainer.GetThrowObject(throwObject.Data.ID).Data);
        }
    }
    /// <summary>
    /// 오브젝트 줍기
    /// </summary>
    public void AddThrowObject(ThrowObjectData throwObjectData)
    {

        if (Model.CurThrowables < Model.MaxThrowables)
        {
            Model.PushThrowObject(throwObjectData);
        }
    }
    #endregion
    #region 암유닛 변경
    public void ChangeArmUnit(ArmUnit armUnit)
    {
        if (Model.Arm != null)
        {
            DeleteArmUnit();
        }

        Model.Arm = Instantiate(armUnit);
        Model.Arm.Init(this);
        foreach (PlayerState state in _states)
        {
            state.InitArm();
        }
    }
    public void ChangeArmUnit(GlobalGameData.AmWeapon armUnit)
    {
        if (Model.Arm != null)
        {
            DeleteArmUnit();
        }

        Model.Arm = Instantiate(DataContainer.GetArmUnit(armUnit));
        Model.Arm.Init(this);
        foreach (PlayerState state in _states)
        {
            state.InitArm();
        }
    }
    private void DeleteArmUnit()
    {
        Destroy(Model.Arm);
    }
    #endregion
    #region 플레이어 추가효과 관련
    /// <summary>
    /// 추가효과 추가
    /// </summary>
    public void AddAdditional(AdditionalEffect addtionalEffect)
    {
        switch (addtionalEffect.AdditionalType)
        {
            case AdditionalEffect.Type.Hit:
                if (CheckForAddAdditionalDuplication(Model.HitAdditionals, addtionalEffect as HitAdditional))
                {
                    Model.HitAdditionals.Add(addtionalEffect as HitAdditional);
                    Model.AdditionalEffects.Add(addtionalEffect);
                    // 적중효과는 배틀시스템에도 추가 등록
                    Battle.AddHitAdditionalList(addtionalEffect as HitAdditional);
                }
                break;
            case AdditionalEffect.Type.Throw:
                if (CheckForAddAdditionalDuplication(Model.ThrowAdditionals, addtionalEffect as ThrowAdditional))
                {
                    Model.ThrowAdditionals.Add(addtionalEffect as ThrowAdditional);
                    Model.AdditionalEffects.Add(addtionalEffect);
                }
                break;
            // 플레이어 추가효과는 플레이어에 종속되기 때문에 Clone을 더해줌
            case AdditionalEffect.Type.Player:
                if (CheckForAddAdditionalDuplication(Model.PlayerAdditionals, addtionalEffect as PlayerAdditional))
                {
                    PlayerAdditional instance = Instantiate(addtionalEffect as PlayerAdditional);
                    Model.PlayerAdditionals.Add(instance);
                    Model.AdditionalEffects.Add(instance);
                    instance.Init(this, addtionalEffect);
                    instance.Enter();
                }
                break;
        }
    }
    /// <summary>
    /// 추가효과 삭제
    /// </summary>
    public void RemoveAdditional(AdditionalEffect addtionalEffect)
    {
        int index = Model.AdditionalEffects.FindIndex(origin => origin.Origin.Equals(addtionalEffect.Origin));
        if (index < 0)
            return;

        addtionalEffect = Model.AdditionalEffects[index];

        switch (addtionalEffect.AdditionalType)
        {
            case AdditionalEffect.Type.Hit:
                if (CheckForRemoveAdditionalDuplication(Model.HitAdditionals, addtionalEffect as HitAdditional))
                {
                    Model.HitAdditionals.Remove(addtionalEffect as HitAdditional);
                    // 배틀시스템에도 적중 효과 삭제
                    Battle.RemoveHitAdditionalList(addtionalEffect as HitAdditional);
                }
                break;
            case AdditionalEffect.Type.Throw:
                if (CheckForRemoveAdditionalDuplication(Model.ThrowAdditionals, addtionalEffect as ThrowAdditional))
                {
                    Model.ThrowAdditionals.Remove(addtionalEffect as ThrowAdditional);
                }
                break;
            case AdditionalEffect.Type.Player:
                if (CheckForRemoveAdditionalDuplication(Model.PlayerAdditionals, addtionalEffect as PlayerAdditional))
                {

                    addtionalEffect.Exit();
                    Model.PlayerAdditionals.Remove(addtionalEffect as PlayerAdditional);
                    Destroy(addtionalEffect);
                }
                break;
        }
    }
    /// <summary>
    /// 추가효과 모두 삭제
    /// </summary>
    public void ClearAdditional()
    {
        // 잠깐 저장해둘 리스트 생성
        List<AdditionalEffect> tempList = new List<AdditionalEffect>(Model.AdditionalEffects.Count);
        // 값 복사
        foreach (AdditionalEffect additionalEffect in Model.AdditionalEffects)
        {
            tempList.Add(additionalEffect);
        }
        // 전부 삭제
        foreach (AdditionalEffect additionalEffect in tempList)
        {
            RemoveAdditional(additionalEffect);
        }
    }

    public void EnterPlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.Enter();
        }
    }
    public void ExitPlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.Exit();
        }
    }
    public void EnterStatePlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.EnterState();
        }
    }
    public void ExitStatePlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.ExitState();
        }
    }
    public void UpdatePlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.Update();
        }
    }

    public void FixedPlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.FixedUpdate();
        }
    }
    public void LatePlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.LateUpdate();
        }
    }
    public void TriggerPlayerAdditional()
    {
        foreach (PlayerAdditional playerAdditional in Model.PlayerAdditionals)
        {
            playerAdditional.Trigger();
        }
    }
    /// <summary>
    /// 추가효과 추가 시 중복 체크
    /// </summary>
    private bool CheckForAddAdditionalDuplication<T>(List<T> additinalList, T additinal) where T : AdditionalEffect
    {
        int index = additinalList.FindIndex(origin => origin.Origin.Equals(additinal.Origin));
        if (index >= additinalList.Count)
            return false;
        // 중복 시
        if (index != -1)
            return false;
        else
            return true;

    }
    /// <summary>
    /// 추가효과 삭제 시 중복 체크
    /// </summary>
    private bool CheckForRemoveAdditionalDuplication<T>(List<T> additinalList, T additinal) where T : AdditionalEffect
    {
        int index = additinalList.FindIndex(origin => origin.Origin.Equals(additinal.Origin));
        if (index >= additinalList.Count)
            return false;
        // 중복 시 (지울 수 있을 때)
        if (index != -1)
        {
            Model.AdditionalEffects.Remove(additinal);
            return true;
        }
        else
            return false;
    }
    #endregion
    #region 지형물 체크 로직
    /// <summary>
    /// 지면 체크
    /// </summary>
    private void CheckGround()
    {
        // 살짝위에서 쏨
        Vector3 CheckPos = _groundCheckPos.position;
        if (Physics.SphereCast(
            CheckPos,
            0.25f,
            Vector3.down,
            out RaycastHit hit,
            0.4f,
             Layer.EveryThing,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer == Layer.Monster)
            {
                IsGround = false;
                Physics.IgnoreLayerCollision(Layer.Player, Layer.Monster, true);
                return;
            }
            IsGround = true;

            if (_states[(int)CurState].IsIgnoreMonster == true)
            {
                Physics.IgnoreLayerCollision(Layer.Player, Layer.Monster, true);
            }
            else
            {
                Physics.IgnoreLayerCollision(Layer.Player, Layer.Monster, false);
            }


            // 오를 수 있는 경사면 체크
            Vector3 normal = hit.normal;
            if (normal.y > 1 - _slopeAngle)
            {
                CanClimbSlope = true;
            }
            else
            {
                CanClimbSlope = false;
            }
        }
        else
        {
            IsGround = false;
            Physics.IgnoreLayerCollision(Layer.Player, Layer.Monster, true);
        }
    }

    private void DrawCheckGround()
    {
        Gizmos.color = Color.yellow;

        Vector3 CheckPos = _groundCheckPos.position;

        if (Physics.SphereCast(CheckPos, 0.25f, Vector3.down, out RaycastHit hit, 0.4f, Layer.GetLayerMaskEveryThing(), QueryTriggerInteraction.Ignore))
        {
            Gizmos.DrawLine(CheckPos, hit.point);
            Gizmos.DrawWireSphere(CheckPos + Vector3.down * hit.distance, 0.3f);
        }
    }

    /// <summary>
    /// 지면에 가까운지 체크
    /// </summary>
    private void CheckIsNearGround()
    {
        Vector3 CheckPos = new Vector3(transform.position.x, transform.position.y + 0.31f, transform.position.z);
        if (Physics.SphereCast(
            CheckPos,
            0.3f,
            Vector3.down,
            out RaycastHit hit,
            1f,
            Layer.EveryThing,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer == Layer.Monster)
            {
                IsNearGround = false;
                return;
            }

            IsNearGround = true;
        }
        else
        {
            IsNearGround = false;
        }
    }

    private void DrawIsNearGround()
    {
        Vector3 CheckPos = new Vector3(transform.position.x, transform.position.y + 0.31f, transform.position.z);
        if (Physics.SphereCast(CheckPos, 0.3f, Vector3.down, out RaycastHit hit, 1, Layer.GetLayerMaskEveryThing(), QueryTriggerInteraction.Ignore))
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(CheckPos, hit.point);
            Gizmos.DrawWireSphere(CheckPos + Vector3.down * hit.distance, 0.3f);
        }
    }

    /// <summary>
    /// 벽체크
    /// </summary>
    private void CheckWall()
    {
        int layerMask = 0;
        layerMask |= 1 << Layer.Wall;
        layerMask |= 1 << Layer.HideWall;
        //layerMask |= 1 << Layer.Monster;
        int hitCount = Physics.OverlapCapsuleNonAlloc(
            WallCheckPos.Foot.position,
            WallCheckPos.Head.position,
            _wallCheckDistance,
            OverLapColliders,
            layerMask);

        if (hitCount > 0)
        {
            IsWall = true;
        }
        else
        {
            IsWall = false;
        }

    }

    private void DrawWallCheck()
    {
        Gizmos.color = Color.green;

        Vector3 footPos = WallCheckPos.Foot.position + transform.forward * _wallCheckDistance;
        Vector3 headPos = WallCheckPos.Head.position + transform.forward * _wallCheckDistance;

        Gizmos.DrawLine(footPos, headPos);
    }
    #endregion

    #region 키입력 관련
    /// <summary>
    /// 키입력 감지
    /// </summary>
    private void ChackInput()
    {
        Vector2 inputDir = InputKey.GetAxis(InputKey.Move);
        MoveDir = new Vector3(inputDir.x, 0, inputDir.y);

        if (IsTargetHolding == false && IsTargetToggle == false)
        {
            if (InputKey.GetButtonDown(InputKey.LoakOn) && IsTargetHolding == false)
            {
                //TODO: 카메라 몬스터 홀딩 기능
                IsTargetToggle = true;
                _cameraHolder.gameObject.SetActive(true);
            }
        }
        else
        {
            //if (input.GetMouseButtonUp(2) && IsTargetToggle == false)
            //{
            //    //TODO: 카메라 몬스터 홀딩 풀기
            //    IsTargetHolding = false;
            //    _cameraHolder.gameObject.SetActive(false);
            //}
            if (InputKey.GetButtonDown(InputKey.LoakOff) && IsTargetHolding == false)
            {
                //TODO: 카메라 몬스터 홀딩 풀기
                IsTargetToggle = false;
                _cameraHolder.gameObject.SetActive(false);
            }
        }
    }
    private void CheckAnyState()
    {
        if (IsDead == true || IsHit == true)
            return;

        if (InputKey.GetButtonDown(InputKey.Dash) && CurState != State.Dash && CurState != State.JumpDown)
        {
            ChangeState(PlayerController.State.Dash);
        }
    }
    #endregion

    #region  넉백
    /// <summary>
    /// 넉백 안함(위치 고정)
    /// </summary>
    public void DontKnockBack(Transform target)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        targetRb.velocity = new(0, targetRb.velocity.y, 0);
    }

    /// <summary>
    /// 해당 방향으로 입력 거리만큼 넉백
    /// </summary>
    public void DoKnockBack(Transform target, Vector3 dir, float distance)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();

        //targetRb.AddForce(dir * distance * 10f, ForceMode.Impulse);

        CoroutineHandler.StartRoutine(KnockBackRoutine(targetRb, dir, distance));
    }
    /// <summary>
    /// 공격자 중심으로 입력거리만큼 넉백
    /// </summary>
    public void DoKnockBack(Transform target, Transform attacker, float distance)
    {
        Vector3 attackerPos = new(attacker.position.x, 0, attacker.position.z);
        Vector3 targetPos = new(target.position.x, 0, target.position.z);
        Vector3 knockBackDir = targetPos - attackerPos;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();


        //targetRb.AddForce(knockBackDir.normalized * distance * 10f, ForceMode.Impulse);

        CoroutineHandler.StartRoutine(KnockBackRoutine(targetRb, knockBackDir, distance));
    }

    /// <summary>
    /// 특정 지점을 중심으로 넉백
    /// </summary>
    /// <param name="target"></param>
    /// <param name="pos"></param>
    /// <param name="distance"></param>
    public void DoKnockBackFromPos(Transform target, Vector3 pos, float distance)
    {
        Vector3 attackerPos = new(pos.x, 0, pos.z);
        Vector3 targetPos = new(target.position.x, 0, target.position.z);
        Vector3 knockBackDir = targetPos - attackerPos;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();

        //targetRb.AddForce(knockBackDir.normalized * distance * 10f, ForceMode.Impulse);

        CoroutineHandler.StartRoutine(KnockBackRoutine(targetRb, knockBackDir, distance));
    }

    IEnumerator KnockBackRoutine(Rigidbody targetRb, Vector3 knockBackDir, float distance)
    {
        BossEnemy bossEnemy = targetRb.GetComponent<BossEnemy>();
        if (bossEnemy != null)
            yield break;



        Vector3 originPos = targetRb.position;

        float knockbackDistance = distance * (1 + Model.KnockBackDistanceMultiplier / 100f);

        targetRb.transform.LookAt(transform.position);
        targetRb.transform.rotation = Quaternion.Euler(0, targetRb.transform.eulerAngles.y, 0);
        // 타겟이 날 바라보도록
        while (true)
        {
            targetRb.transform.Translate(knockBackDir * Time.deltaTime * 30f, Space.World);

            if ((originPos - targetRb.position).sqrMagnitude > Mathf.Pow(knockbackDistance,2))
            {
                break;
            }

            Vector3 targetPos = new(targetRb.transform.position.x, targetRb.transform.position.y + 0.75f, targetRb.transform.position.z);
            if (Physics.SphereCast(targetRb.transform.position, 0.2f, knockBackDir, out RaycastHit hit, 0.3f, 1 << Layer.Wall, QueryTriggerInteraction.Ignore))
            {
                break;
            }
            yield return null;
        }
    }
    #endregion

    #region 데미지 계산


    /// <summary>
    /// 모델의 데미지, 크리티컬 반영 x
    /// </summary>
    public int GetDamage(int damage)
    {
        int finalDamage = damage;
        // 데미지 배율이 0까지 떨어진 경우 0으로 고정
        float attackMultiplier = 1 + Model.DamageMultiplier / 100 >= 0 ? 1 + Model.DamageMultiplier / 100 : 0;
        finalDamage = (int)(finalDamage * attackMultiplier);

        return finalDamage;
    }
    /// <summary>
    /// 기본 스텟 데미지
    /// </summary>
    public int GetFinalDamage(out bool isCritical)
    {
        int finalDamage = 0;
        finalDamage = GetCommonDamage(finalDamage, out isCritical);
        return finalDamage;
    }
    /// <summary>
    /// 데미지 추가
    /// </summary>
    public int GetFinalDamage(int addtionalDamage, out bool isCritical)
    {
        int finalDamage = addtionalDamage;
        finalDamage = GetCommonDamage(finalDamage, out isCritical);
        return finalDamage;
    }
    /// <summary>
    /// 데미지 배율
    /// </summary>
    public int GetFinalDamage(float addtionalMultiplier, out bool isCritical)
    {
        int finalDamage = 0;
        finalDamage = GetCommonDamage(finalDamage, out isCritical);

        // 데미지 배율 추가
        finalDamage = (int)(finalDamage * (1 + addtionalMultiplier / 100));
        return finalDamage;
    }
    /// <summary>
    /// 추가 데미지 + 데미지 배율
    /// </summary>
    public int GetFinalDamage(int addtionalDamage, float additionalMultiplier, out bool isCritical)
    {
        int finalDamage = 0;
        // 추가 데미지
        finalDamage += addtionalDamage;
        finalDamage = GetCommonDamage(finalDamage, out isCritical);

        // 데미지 배율 추가
        finalDamage = (int)(finalDamage * (1 + additionalMultiplier / 100));
        return finalDamage;
    }
    /// <summary>
    /// 공통계산 용
    /// </summary>
    private int GetCommonDamage(int finalDamage, out bool isCritical)
    {
        // 기본 스텟 데미지 
        finalDamage += Model.AttackPower;
        // 치명타 데미지
        if (Random.value < Model.CriticalChance / 100f)
        {
            finalDamage = (int)(finalDamage * (Model.CriticalDamage / 100f));
            isCritical = true;
        }
        else
            isCritical = false;

        // 데미지 배율이 0까지 떨어진 경우 0으로 고정
        float attackMultiplier = 1 + Model.DamageMultiplier / 100 >= 0 ? 1 + Model.DamageMultiplier / 100 : 0;
        finalDamage = (int)(finalDamage * attackMultiplier);

        return finalDamage;
    }
    #endregion

    /// <summary>
    /// TPS 시점 카메라 회전
    /// </summary>
    private void RotateCameraStick()
    {
        Vector2 stickDir = InputKey.GetAxis(InputKey.CameraMove);

        float angleX = stickDir.x * _stickRotateSpeed;

        float rotateSpeed = 1;
        if (Model.IsTest == false)
        {
            rotateSpeed = setting.cameraSpeed;
        }
        Vector2 mouseDelta = new Vector2(angleX, 0) * rotateSpeed;
        Vector3 camAngle = CamareArm.rotation.eulerAngles;

        // 카메라 조정
        CamareArm.rotation = Quaternion.Euler(camAngle.x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    /// <summary>
    /// TPS 시점 카메라 회전
    /// </summary>
    private void RotateCameraMouse()
    {
        Vector2 mouseDir = InputKey.GetAxis(InputKey.MouseDelta);

        float angleX = mouseDir.x * _mouseRotateSpeed;

        float rotateSpeed = 1;
        if (Model.IsTest == false)
        {
            rotateSpeed = setting.cameraSpeed;
        }
        Vector2 mouseDelta = new Vector2(angleX, 0) * rotateSpeed;
        Vector3 camAngle = CamareArm.rotation.eulerAngles;

        // 카메라 조정
        CamareArm.rotation = Quaternion.Euler(camAngle.x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    /// <summary>
    /// 스테미나 회복 코루틴
    /// </summary>
    IEnumerator RecoveryStamina()
    {
        CanStaminaRecovery = true;
        while (true)
        {
            // 초당 MaxStamina / RegainStamina 만큼 회복
            // 현재 스테미나가 꽉찼으면 더이상 회복안함
            // 만약 스테미나 사용 후 쿨타임 상태면 쿨타임만큼 회복안함
            if (CanStaminaRecovery == true)
            {
                Model.CurStamina += Model.RegainStamina * Time.deltaTime;
            }
            if (Model.CurStamina >= Model.MaxStamina)
            {
                Model.CurStamina = Model.MaxStamina;
            }

            if (IsStaminaCool == true)
            {
                IsStaminaCool = false;
                //yield return 1f.GetDelay();
                yield return Model.StaminaCoolTime.GetDelay();
            }

            yield return null;
        }
    }
    void ControlMousePointer()
    {

        if (InputKey.GetActionMap() == InputType.GAMEPLAY)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CanOperate = true;

            if (_isAudioPlay == false)
            {
                _isAudioPlay = true;
                Audio.UnPause();
            }

        }
        else if (InputKey.GetActionMap() == InputType.UI)
        {
            if (InputKey.PlayerInput.currentControlScheme == InputType.CONSOLE)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            CanOperate = false;

            if (_isAudioPlay == true)
            {

                _isAudioPlay = false;
                Audio.Pause();
            }

        }
    }

    private void TriggerCantOperate()
    {
        _states[(int)CurState].TriggerCantOperate();
    }


    // 초기 설정 ============================================================================================================================================ //
    /// <summary>
    /// 초기 설정
    /// </summary>
    private void Init()
    {
        InitGetComponent();
        InitPlayerStates();

        _defaultMuzzlePointRot = MuzzletPoint.localRotation;



    }

    /// <summary>
    /// 플레이어 상태 배열 설정
    /// </summary>
    private void InitPlayerStates()
    {
        _states[(int)State.Idle] = new IdleState(this);                 // Idle
        _states[(int)State.Run] = new RunState(this);                   // 이동(달리기)
        _states[(int)State.MeleeAttack] = new MeleeAttackState(this);   // 근접공격
        _states[(int)State.ThrowAttack] = new ThrowState(this);         // 투척공격
        _states[(int)State.SpecialAttack] = new SpecialAttackState(this); // 특수공격
        _states[(int)State.Jump] = new JumpState(this);                 // 점프
        _states[(int)State.DoubleJump] = new DoubleJumpState(this);     // 더블점프
        _states[(int)State.JumpAttack] = new JumpAttackState(this);     // 점프공격
        _states[(int)State.JumpDown] = new JumpDownState(this);         // 하강 공격 
        _states[(int)State.Fall] = new FallState(this);                 // 추락
        _states[(int)State.DoubleJumpFall] = new DoubleJumpFallState(this); // 더블점프 추락
        _states[(int)State.Dash] = new DashState(this);                 // 대쉬
        _states[(int)State.Drain] = new DrainState(this);               // 드레인
        _states[(int)State.Hit] = new HitState(this);                   // 피격
        _states[(int)State.Dead] = new DeadState(this);                 // 사망
    }

    /// <summary>
    /// UI이벤트 설정
    /// </summary>
    private void InitUIEvent()
    {
        PlayerPanel panel = View.Panel;

        // 투척오브젝트
        Model.MaxThrowCountSubject = new Subject<int>();
        Model.MaxThrowCountSubject
            .DistinctUntilChanged()
            .Subscribe(x => View.UpdateText(panel.ObjectCount, $"{x} / {Model.MaxThrowables}"));
        View.UpdateText(panel.ObjectCount, $"{Model.CurThrowables} / {Model.MaxThrowables}");


        // 투척오브젝트
        Model.CurThrowCountSubject = new Subject<int>();
        Model.CurThrowCountSubject
            .DistinctUntilChanged()
            .Subscribe(x => View.UpdateText(panel.ObjectCount, $"{x} / {Model.MaxThrowables}"));
        View.UpdateText(panel.ObjectCount, $"{Model.CurThrowables} / {Model.MaxThrowables}");

        // 체력
        Model.CurHpSubject
            .DistinctUntilChanged()
            .Subscribe(x => panel.BarValueController(panel.HpBar, Model.CurHp, Model.MaxHp));
        panel.BarValueController(panel.HpBar, Model.CurHp, Model.MaxHp);

        // 스테미나
        Model.CurStaminaSubject
            .DistinctUntilChanged()
            .Subscribe(x => panel.BarValueController(panel.StaminaBar, Model.CurStamina, Model.MaxStamina));
        panel.BarValueController(panel.StaminaBar, Model.CurStamina, Model.MaxStamina);

        // 특수자원
        Model.CurManaSubject
            .DistinctUntilChanged()
            .Subscribe(x => panel.MpBar.value = x);
        panel.MpBar.maxValue = Model.MaxMana;
        panel.MpBar.value = Model.CurMana;

        // 특수공격 차지
        Model.SpecialChargeGageSubject
            .DistinctUntilChanged()
            .Subscribe(x => panel.ChargingMpBar.value = x);
        panel.ChargingMpBar.value = Model.SpecialChargeGage;

        // 스테미나 차지
        Model.MaxStaminaCharge = 1;
        Model.CurStaminaChargeSubject
            .DistinctUntilChanged()
            .Subscribe(x => panel.BarValueController(panel.ChanrgeStaminaBar, Model.CurStaminaCharge, Model.MaxStaminaCharge));
        panel.BarValueController(panel.ChanrgeStaminaBar, Model.CurStaminaCharge, Model.MaxStaminaCharge);
    }
    /// <summary>
    /// 이벤트 구독
    /// </summary>
    private void SubscribeEvents()
    {
        Battle.OnTargetAttackEvent += TargetAttackCallback;
        Battle.OnTakeDamageEvent += TakeDamageCallback;
    }

    /// <summary>
    /// 초기 겟컴포넌트 설정
    /// </summary>
    private void InitGetComponent()
    {
        Model = GetComponent<PlayerModel>();
        View = GetComponent<PlayerView>();
        Rb = GetComponent<Rigidbody>();
        Collider = GetComponent<Collider>();
        Battle = GetComponent<BattleSystem>();
        Audio = GetComponent<AudioSource>();
    }
    private void InitAdditionnal()
    {
        Model.AdditionalEffects.Clear();
        List<AdditionalEffect> tempList = new List<AdditionalEffect>();

        ProcessInitAddtional(tempList, Model.PlayerAdditionals);
        ProcessInitAddtional(tempList, Model.ThrowAdditionals);
        ProcessInitAddtional(tempList, Model.HitAdditionals);
    }
    private void ProcessInitAddtional<T>(List<AdditionalEffect> tempList, List<T> additionals) where T : AdditionalEffect
    {
        foreach (AdditionalEffect additional in additionals)
        {
            tempList.Add(additional);
        }
        additionals.Clear();
        foreach (AdditionalEffect additional in tempList)
        {
            AddAdditional(additional);
        }
        tempList.Clear();
    }

    private void StartRoutine()
    {
        StartCoroutine(RecoveryStamina());
    }


    #region 애니메이션 콜백

    public void OnTrigger()
    {
        _states[(int)CurState].OnTrigger();
        TriggerPlayerAdditional();
    }
    public void EndAnimation()
    {
        _states[(int)CurState].EndAnimation();
    }
    public void OnCombo()
    {
        _states[(int)CurState].OnCombo();
    }
    public void EndCombo()
    {
        _states[(int)CurState].EndCombo();
    }
    #endregion
    #region 콜백
    public void ThrowObjectResultCallback(ThrowObject throwObject, bool successHit)
    {
        OnThrowObjectResult?.Invoke(throwObject, successHit);
    }
    private void TargetAttackCallback(int damage, bool isCritical)
    {
        DrainLife(damage);
    }
    private void TakeDamageCallback(int damage, bool isCritical)
    {

    }
    #endregion
    #region 오디오 컨트롤
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;
        Audio.clip = clip;
        Audio.Play();
    }
    public void StopSFX()
    {
        Audio.Stop();
        Audio.clip = null;
    }
    #endregion
}
