using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraHold : MonoBehaviour
{
    [SerializeField] GameObject _targetEffect;
    [Range(0, 360)][SerializeField] float _angle;
    [SerializeField] float _detectRange;
    private PlayerController _player;

    private int _targetIndex;
    private bool _isStart;
    Coroutine _checkDistanceToTarget;
    Coroutine _changeTargetRoutine;
    [System.Serializable]
    struct TargetInfo
    {
        public Transform transform;
        public Collider Collider;
        public float Distance;
    }

    [SerializeField] private List<TargetInfo> _targetList = new List<TargetInfo>();
    [SerializeField] private TargetInfo _target;
    private void Awake()
    {
        _player = GetComponentInParent<PlayerController>();
        _targetEffect.transform.SetParent(null);
    }

    private void Start()
    {
        gameObject.SetActive(false);
        _isStart = true;
    }
    private void OnEnable()
    {
        if (_isStart == false)
            return;
        _targetEffect.SetActive(true);
        SetTargetList();
    }

    private void OnDisable()
    {
        if (_checkDistanceToTarget != null)
        {
            StopCoroutine(_checkDistanceToTarget);
            _checkDistanceToTarget = null;
        }
        if (_changeTargetRoutine != null)
        {
            StopCoroutine(_changeTargetRoutine);
            _changeTargetRoutine = null;
        }

        _targetEffect.SetActive(false);
        _targetList.Clear();
        _target = default;
        _player.IsTargetHolding = false;
        _player.IsTargetToggle = false;
    }

    private void Update()
    {
        // 타겟을 찾지 못했을때
        if (_targetList.Count <= 0)
            return;
        // 타겟이 죽었을때 (Destroy되거나 Disable됬을때)
        if (_target.transform == null || _target.transform.gameObject.activeSelf == false || _target.Collider.enabled == false)
        {
            // 리스트를 비운 후, 다시 전방의 적을 재탐색한다
            _targetList.Clear();
            SetTargetList();
        }
        else
        {
            // 플레이어의 타겟 지점을 지정한 타겟위치로 지정
            _player.TargetPos = _target.transform.position;
            // 타겟 이펙트효과 타겟위치
            _targetEffect.transform.position = new(_target.transform.position.x, _target.transform.position.y + 0.3f, _target.transform.position.z);
        }

    }

    /// <summary>
    /// 적 타겟팅
    /// </summary>
    private bool SetTargetList()
    {
        // 플레이어가 앞을 바라봄
        //Player.LookAtCameraFoward();

        // 주변에 몬스터가 있는지 스캔
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _detectRange, _player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            Transform targetTransform = _player.OverLapColliders[i].transform;
            // 2. 각도 내에 있는지 확인
            Vector3 source = transform.position;
            source.y = 0;
            Vector3 destination = targetTransform.position;
            destination.y = 0;

            Vector3 targetDir = (destination - source).normalized;

            //float targetAngle = Vector3.Angle(_player.CamareArm.forward, targetDir); // 아크코사인 필요 (느리다)
            float targetAngle = Vector3.Dot(_player.CamareArm.forward, targetDir);
            targetAngle = Mathf.Acos(targetAngle) * Mathf.Rad2Deg;
            if (targetAngle > _angle * 0.5f)
                continue;

            // 조건에 부합하면 해당 타겟을 거리와 함께 저장
            TargetInfo targetInfo = SetTargetInfo(targetTransform, _player.OverLapColliders[i], (targetTransform.position - transform.position).sqrMagnitude);
            _targetList.Add(targetInfo);
        }
        // 스캔에 실패했다면 바로 꺼짐
        if (_targetList.Count <= 0)
        {
            gameObject.SetActive(false);
            return false;
        }
        // 타겟을 거리순으로 정렬
        _targetList.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        // 가장 가까운 적을 타겟으로 지정
        _target = _targetList[0];
        _targetIndex = 0;

        if (_checkDistanceToTarget == null)
        {
            _checkDistanceToTarget = StartCoroutine(CheckDistanceToTarget());
        }
        if (_changeTargetRoutine == null)
        {
            _changeTargetRoutine = StartCoroutine(ChangeTargetRoutine());
        }
        return true;
    }

    //TargetInfo 설정
    private TargetInfo SetTargetInfo(Transform target, Collider collider, float distance)
    {
        TargetInfo info = new TargetInfo();
        info.transform = target;
        info.Collider = collider;
        info.Distance = distance;
        return info;
    }

    /// <summary>
    /// 플레이어와 타겟이 멀어졌을때 타겟팅 자동 종료
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckDistanceToTarget()
    {
        while (true)
        {
            if (_target.transform != null)
            {
                //if (Vector3.Distance(_player.transform.position, _target.transform.position) > _detectRange)
                if ((_player.transform.position - _target.transform.position).sqrMagnitude > Mathf.Pow(_detectRange, 2))
                {
                    gameObject.SetActive(false);
                }
            }
            yield return 0.5f.GetDelay();
        }
    }

    IEnumerator ChangeTargetRoutine()
    {
        while (true)
        {
            yield return null;
            if (_player.IsTargetToggle == true && InputKey.GetButtonDown(InputKey.LoakOn))
            {
                // 현재 인덱스 임시 저장
                int tempIndex = _targetIndex;

                // 리스트를 비운 후, 다시 전방의 적을 재탐색한다
                _targetList.Clear();
                bool success = SetTargetList();
                if (success == false)
                    continue;

                // 타겟 인덱스 올림
                _targetIndex = tempIndex + 1;
                // 타겟 인덱스가 리스트 카운트값을 넘었다면 다시 처음부터
                if (_targetIndex >= _targetList.Count)
                {
                    _targetIndex = 0;
                }
                // 해당 타겟으로 변경
                _target = _targetList[_targetIndex];
            }
            //else if (Player.IsTargetToggle == true && mouseScroll < 0)
            //{
            //    // 타겟 인덱스 내림
            //    _targetIndex--;
            //    // 타겟 인덱스가 0 이하라면 맨 위부터
            //    if (_targetIndex < 0)
            //    {
            //        _targetIndex = _targetList.Count - 1;
            //    }
            //    // 해당 타겟으로 변경
            //    _target = _targetList[_targetIndex].transform;
            //    yield return 0.2f.GetDelay();
            //}
        }
    }
}
