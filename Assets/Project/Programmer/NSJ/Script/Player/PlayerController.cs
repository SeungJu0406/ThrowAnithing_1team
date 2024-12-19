using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerModel))]
[RequireComponent(typeof(PlayerView))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public PlayerModel Model;
    [HideInInspector] public PlayerView View;
    [HideInInspector] public Rigidbody Rb;

    #region �̺�Ʈ
    #endregion
    #region ���� ���� �ʵ�
    [System.Serializable]
    struct AttackStruct
    {
        public float AttackHeight;
        public float AttackBufferTime;
        public Transform MuzzlePoint;
        public ThrowObject ThrowPrefab;
    }
    [Header("���� ���� �ʵ�")]
    [SerializeField] private AttackStruct _attackStruct;
    public float AttackBufferTime { get { return _attackStruct.AttackBufferTime; } set { _attackStruct.AttackBufferTime = value; } }
    public Transform MuzzletPoint { get { return _attackStruct.MuzzlePoint; } set { _attackStruct.MuzzlePoint = value; } }
    public float AttackHeight { get { return _attackStruct.AttackHeight; } set { _attackStruct.AttackHeight = value; } }
    private ThrowObject _throwPrefab { get { return _attackStruct.ThrowPrefab; } set { _attackStruct.ThrowPrefab = value; } }
    #endregion
    #region Camera ���� �ʵ�
    /// <summary>
    /// ī�޶� ����
    /// </summary>
    [System.Serializable]
    struct CameraStruct
    {
        public Transform CamaraArm;
        public Transform CameraPos;
        [Range(0f, 50f)] public float CameraRotateAngle;
        [Range(0f, 5f)] public float CameraRotateSpeed;
        public bool IsVerticalCameraMove;
    }
    [Header("ī�޶� ���� �ʵ�")]
    [SerializeField] private CameraStruct _cameraStruct;
    public Transform CamareArm { get { return _cameraStruct.CamaraArm; } set { _cameraStruct.CamaraArm = value; } }
    private Transform _cameraPos { get { return _cameraStruct.CameraPos; } set { _cameraStruct.CameraPos = value; } }
    private float _cameraRotateAngle { get { return _cameraStruct.CameraRotateAngle; } set { _cameraStruct.CameraRotateAngle = value; } }
    private float _cameraRotateSpeed { get { return _cameraStruct.CameraRotateSpeed; } set { _cameraStruct.CameraRotateSpeed = value; } }
    private bool _isVerticalCameraMove { get { return _cameraStruct.IsVerticalCameraMove; } set { _cameraStruct.IsVerticalCameraMove = value; } }
    #endregion
    #region ���� ���� �ʵ�
    [System.Serializable]
    struct CheckStruct
    {
        public Transform GroundCheckPos;
        public WallCheckStruct WallCheckPos;
    }
    [System.Serializable]
    struct WallCheckStruct
    {
        public Transform Head;
        public Transform Chest;
        public Transform Foot;
    }
    [SerializeField] private CheckStruct _checkStruct;
    private Transform _groundCheckPos => _checkStruct.GroundCheckPos;
    private WallCheckStruct _wallCheckPos => _checkStruct.WallCheckPos;
    #endregion
    #region �׽�Ʈ ���� �ʵ�
    [System.Serializable]
    public struct TestStruct
    {
        public bool IsAttackForward;
    }
    [Header("�׽�Ʈ ���� �ʵ�")]
    [SerializeField] private TestStruct _testStruct;
    public bool IsAttackFoward { get { return _testStruct.IsAttackForward; } }
    #endregion
    public enum State { Idle, Run, MeleeAttack, ThrowAttack, Jump, Fall, Dash, Size }

    private PlayerState[] _states = new PlayerState[(int)State.Size];
    public State CurState;
    public State PrevState;

    public bool IsGround; // ���� ���� ����

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        InitUIEvent();
        Camera.main.transform.SetParent(_cameraPos, true);
        _states[(int)CurState].Enter();
    }

    private void OnDisable()
    {
        _states[(int)CurState].Exit();
    }

    private void Update()
    {      
        _states[(int)CurState].Update();

        CheckAnyState();
        RotateCamera();

        TestInput();
    }

    private void FixedUpdate()
    {
        _states[(int)CurState].FixedUpdate();
        CheckGround();
    }

    private void OnDrawGizmos()
    {
        if (_states[(int)CurState] == null)
            return;
        _states[(int)CurState].OnDrawGizmos();

        DrawCheckGround();
        //DrawWallCheck(); 
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void ChangeState(State state)
    {
        _states[(int)CurState].Exit();
        PrevState = CurState;
        CurState = state;
        _states[(int)CurState].Enter();

        //Debug.Log(CurState);
    }

    #region Instantiate �븮 �޼���
    public T InstantiateObject<T>(T instance) where T : Component
    {
        T instanceObject = Instantiate(instance);
        return instanceObject;
    }
    public T InstantiateObject<T>(T instance, Transform parent) where T : Component
    {
        T instanceObject = Instantiate(instance, parent);
        return instanceObject;
    }
    public T InstantiateObject<T>(T instance, Vector3 pos, Quaternion rot) where T : Component
    {
        T instanceObject = Instantiate(instance, pos, rot);
        return instanceObject;
    }
    #endregion

    /// <summary>
    /// ������Ʈ �ݱ�
    /// </summary>
    public void AddThrowObject(ThrowObject throwObject)
    {
        if (Model.CurThrowCount < Model.MaxThrowCount)
        {
            Model.PushThrowObject(DataContainer.GetThrowObject(throwObject.Data.ID).Data);
            Destroy(throwObject.gameObject);
        }
    }

    /// <summary>
    /// �߰� ����ȿ�� �߰�
    /// </summary>
    /// <param name="hitAdditional"></param>
    public void AddHitAdditional(HitAdditional hitAdditional)
    {
        Model.HitAdditionals.Add(hitAdditional);
    }

    private void CheckAnyState()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && CurState != State.Dash)
        {
            _states[(int)CurState].OnDash();
            ChangeState(PlayerController.State.Dash);
        }
    }

    /// <summary>
    /// TPS ���� ī�޶� ȸ��
    /// </summary>
    private void RotateCamera()
    {
        float angleX = Input.GetAxis("Mouse X");
        float angleY = default;
        // üũ�� ���콺 ���ϵ� ����
        if (_isVerticalCameraMove == true)
            angleY = Input.GetAxis("Mouse Y");
        Vector2 mouseDelta = new Vector2(angleX, angleY) * _cameraRotateSpeed;
        Vector3 camAngle = CamareArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;
        x = x < 180 ? Mathf.Clamp(x, -10f, 50f) : Mathf.Clamp(x, 360f - _cameraRotateAngle, 361f);
        CamareArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

        // ��������Ʈ ��������
        MuzzletPoint.rotation = Quaternion.Euler(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    /// <summary>
    /// ���� üũ
    /// </summary>
    private void CheckGround()
    {
        // ��¦������ ��
        Vector3 CheckPos = _groundCheckPos.position;

        if (Physics.SphereCast(CheckPos, 0.25f, Vector3.down, out RaycastHit hit, 0.4f))
        {
            //Debug.Log("����");
            IsGround = true;
        }
        else
        {
            // Debug.Log("����");
            IsGround = false;
        }
    }

    private void DrawCheckGround()
    {
        Gizmos.color = Color.yellow;

        Vector3 CheckPos = _groundCheckPos.position;

        if (Physics.SphereCast(CheckPos, 0.25f, Vector3.down, out RaycastHit hit, 0.4f))
        {
            Gizmos.DrawLine(CheckPos, hit.point);
            Gizmos.DrawWireSphere(CheckPos + Vector3.down * hit.distance, 0.3f);
        }
    }

    //private void DrawWallCheck()
    //{
    //    Gizmos.color = Color.green;

    //    if(Physics.Raycast(_wallCheckPos.Head.position, _wallCheckPos.Head.forward, out RaycastHit hitHead,0.4f))
    //    {
    //        Gizmos.DrawLine(_wallCheckPos.Head.position, hitHead.point);
    //    }
    //    if (Physics.Raycast(_wallCheckPos.Chest.position, _wallCheckPos.Head.forward, out RaycastHit hitChest, 0.4f))
    //    {
    //        Gizmos.DrawLine(_wallCheckPos.Chest.position, hitChest.point);
    //    }
    //    if (Physics.Raycast(_wallCheckPos.Foot.position, _wallCheckPos.Head.forward, out RaycastHit hitFoot, 0.4f))
    //    {
    //        Gizmos.DrawLine(_wallCheckPos.Foot.position, hitFoot.point);
    //    }
    //}

    private void TestInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ThrowObject throwObject = Instantiate(_throwPrefab);
            Model.PushThrowObject(DataContainer.GetThrowObject(throwObject.Data.ID).Data);
            Destroy(throwObject.gameObject);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene(1);
        }
    }

    // �ʱ� ���� ============================================================================================================================================ //
    /// <summary>
    /// �ʱ� ����
    /// </summary>
    private void Init()
    {
        InitGetComponent();
        InitPlayerStates();
    }

    /// <summary>
    /// �÷��̾� ���� �迭 ����
    /// </summary>
    private void InitPlayerStates()
    {
        _states[(int)State.Idle] = new IdleState(this);                 // Idle
        _states[(int)State.Run] = new RunState(this);                   // �̵�(�޸���)
        _states[(int)State.MeleeAttack] = new MeleeAttackState(this);   // ��������
        _states[(int)State.ThrowAttack] = new ThrowState(this);         // ��ô����
        _states[(int)State.Jump] = new JumpState(this);                 // ����
        _states[(int)State.Fall] = new FallState(this);                 // �߶�
        _states[(int)State.Dash] = new DashState(this);                 // �뽬
    }

    /// <summary>
    /// UI�̺�Ʈ ����
    /// </summary>
    private void InitUIEvent()
    {
        Model.CurThrowCountSubject
            .DistinctUntilChanged()
            .Subscribe(x => View.UpdateText(View.Panel.ThrowCount, $"{x} / {Model.MaxThrowCount}"));
        View.UpdateText(View.Panel.ThrowCount, $"{Model.CurThrowCount} / {Model.MaxThrowCount}");
    }

    /// <summary>
    /// �ʱ� ��������Ʈ ����
    /// </summary>
    private void InitGetComponent()
    {
        Model = GetComponent<PlayerModel>();
        View = GetComponent<PlayerView>();
        Rb = GetComponent<Rigidbody>();
    }
}