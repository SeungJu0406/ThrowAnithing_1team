using System.Collections;
using UnityEditor;
using UnityEngine;

public class DrainState : PlayerState
{
    private GameObject _drainField;
    private GameObject _drainEffect;

    private float _drainDistance => Model.DrainDistance * 2;
    private float _curDrainDistance;
    private bool _isEnd;
    private bool _isFirst;
    private Vector3 _drainStartPos;
    private Collider[] _overlapColliders = new Collider[150];
    private Coroutine _drainRoutine;
    public DrainState(PlayerController controller) : base(controller)
    {
        UseStamina = true;    
    }
    public override void InitArm()
    {
        StaminaAmount = Model.DrainStamina;
    }
    public override void Enter()
    {
        Model.CurStamina += 10;
        _isEnd = false;
        _isFirst = false;

        Player.Rb.velocity = Vector3.zero;
        View.SetBool(PlayerView.Parameter.Drain, true);

        // 이펙트
        _drainField = ObjectPool.Get(Effect.Drain_Range, transform);
        _drainField.transform.localScale = new Vector3(0, _drainField.transform.localScale.y, 0);
        _drainEffect = ObjectPool.Get(Effect.Drain_Charge, Player.Battle.HitPoint);

        // 플레이어 위치 고정시킬 좌표 캐싱
        _drainStartPos = transform.position;

        Player.CanStaminaRecovery = false;

        if (_drainRoutine == null)
        {
            _drainRoutine = CoroutineHandler.StartRoutine(DrainRoutine());
        }
    }
    public override void Exit()
    {
        if (_drainRoutine != null)
        {
            CoroutineHandler.StopRoutine(_drainRoutine);
            _drainRoutine = null;
        }
        Player.CanStaminaRecovery = true;     
        View.SetBool(PlayerView.Parameter.Drain, false);

        ObjectPool.Return(_drainField);
        ObjectPool.Return(_drainEffect);
    }

    public override void Update()
    {
        // 플레이어 위치 고정
        transform.position = _drainStartPos;

        CheckInput();

        float curDistance = _drainField.transform.localScale.x;
        // 크기가 점점 커지도록
        if (curDistance < _drainDistance)
        {
            _drainField.transform.localScale = new Vector3(
                curDistance + _drainDistance * Time.deltaTime,
                _drainField.transform.localScale.y,
                curDistance + _drainDistance * Time.deltaTime);
        }

    }

    public override void OnTrigger()
    {
        ObjectPool.Return(_drainField);
        ObjectPool.Return(_drainEffect);
        if (_drainRoutine != null) 
        {
            CoroutineHandler.StopRoutine(_drainRoutine);
            _drainRoutine = null;
        }

        _curDrainDistance = 0;
    }

    public override void EndAnimation()
    {
        ChangeState(PlayerController.State.Idle);
    }

    public override void TriggerCantOperate()
    {
        if (Player.CanOperate == false)
        {
            ChangeState(PlayerController.State.Idle);
        }
    }




    IEnumerator DrainRoutine()
    {
        while (true)
        {
            _curDrainDistance += Time.deltaTime * 10;
            if (_curDrainDistance > Model.DrainDistance)
                _curDrainDistance = Model.DrainDistance;
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _curDrainDistance, _overlapColliders);
            for (int i = 0; i < hitCount; i++)
            {
                if (_overlapColliders[i].tag != Tag.Trash && _overlapColliders[i].tag != Tag.Item)
                    continue;

                Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
                _overlapColliders[i].transform.position = Vector3.MoveTowards(_overlapColliders[i].transform.position, playerPos, 5f * Time.deltaTime);
            }

            Model.CurStamina -= Time.deltaTime * Model.DrainStamina;

            if(Model.CurStamina <= 0)
            {
                _isEnd = true;
                EndDrain();
            }
            yield return null;
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, _curDrainDistance);
    }


    private void CheckInput()
    {
        //드레인 키를 뗐을 때
        if (InputKey.GetButtonUp(InputKey.Drain))
        {
            _isEnd = true;
            EndDrain();
        }
    }

    private void EndDrain()
    {
        if (_isEnd == true && _isFirst == false)
        {
            _isEnd = false;
            _isFirst = true;
            View.SetBool(PlayerView.Parameter.Drain, false);
        }
    }
}
