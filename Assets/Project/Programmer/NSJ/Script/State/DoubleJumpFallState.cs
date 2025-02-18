using System.Collections;
using UnityEngine;

public class DoubleJumpFallState : PlayerState
{
    private Vector3 _inertia; // 관성력

    Coroutine _fallRoutine;
    Coroutine _checkInputRoutine;
    public DoubleJumpFallState(PlayerController controller) : base(controller)
    {
        IsIgnoreMonster = true;
    }
    public override void Enter()
    {
        if (Player.PrevState != PlayerController.State.Jump &&
            Player.PrevState != PlayerController.State.DoubleJump)
        {
            View.SetTrigger(PlayerView.Parameter.DoubleJumpFall);
        }
        _inertia = new Vector3(Rb.velocity.x, Rb.velocity.y, Rb.velocity.z);
        if (_fallRoutine == null)
        {
            _fallRoutine = CoroutineHandler.StartRoutine(FallRoutine());
        }
    }

    public override void Exit()
    {
        if (_fallRoutine != null)
        {
            CoroutineHandler.StopRoutine(_fallRoutine);
            _fallRoutine = null;
        }

        if (_checkInputRoutine != null)
        {
            CoroutineHandler.StopRoutine(_checkInputRoutine);
            _checkInputRoutine = null;
        }
    }

    public override void FixedUpdate()
    {
        CheckIsNearGround();
    }
    public override void OnDrawGizmos()
    {
        Vector3 CheckPos = new Vector3(transform.position.x, transform.position.y + 0.31f, transform.position.z);
        if (Physics.SphereCast(CheckPos, 0.3f, Vector3.down, out RaycastHit hit, 1, Layer.EveryThing, QueryTriggerInteraction.Ignore))
        {
            Gizmos.DrawLine(CheckPos, hit.point);
            Gizmos.DrawWireSphere(CheckPos + Vector3.down * hit.distance, 0.3f);
        }
    }
    public override void EndAnimation()
    {
        ChangeState(PlayerController.State.Idle);
    }

    IEnumerator FallRoutine()
    {
        // Fall 상태에서 받을 수 있는 입력 대기
        if (_checkInputRoutine == null)
            _checkInputRoutine = CoroutineHandler.StartRoutine(CheckInputRoutine());

        yield return 0.1f.GetDelay();
        while (Player.IsGround == false)
        {
            if (MoveDir == Vector3.zero)
            {
                Rb.velocity = new Vector3(_inertia.x, Rb.velocity.y, _inertia.z);
            }
            else
            {
                Player.LookAtMoveDir();

                Vector3 moveDir = transform.forward * Model.MoveSpeed;
                //Vector3 moveDir = transform.forward * MoveDir.z * Model.MoveSpeed + transform.right * MoveDir.x * Model.MoveSpeed;
                Rb.velocity = new Vector3(moveDir.x, Rb.velocity.y, moveDir.z);
                _inertia = Rb.velocity;
            }
            if (Player.IsWall == true)
            {
                Rb.velocity = new Vector3(0, Rb.velocity.y, 0);
            }
            // 떨어지기 시작했을때 지면과 충분히 가까워졌다면 지면 체크 로직 종료
            if (Rb.velocity.y < 0)
            {
                Vector3 CheckPos = new Vector3(transform.position.x, transform.position.y + 0.31f, transform.position.z);
                if (Player.IsNearGround && Rb.velocity.y < 0)
                    break;
            }
            yield return 0.02f.GetDelay();
        }

        if (_checkInputRoutine != null)
        {
            CoroutineHandler.StopRoutine(_checkInputRoutine);
            _checkInputRoutine = null;
        }
        // 착지 애니메이션 실행
        Player.IsDoubleJump = false;
        Player.IsJumpAttack = false;
        View.SetTrigger(PlayerView.Parameter.Landing);

        SoundManager.PlaySFX(Player.Sound.Move.Landing);
    }

    IEnumerator CheckInputRoutine()
    {
        while (true)
        {
            if (InputKey.GetButtonDown(InputKey.Jump) && Player.IsDoubleJump == false)
            {
                Player.IsDoubleJump = true;
                ChangeState(PlayerController.State.DoubleJump);
                break;
            }
            if (InputKey.GetButtonDown(InputKey.Throw) && Player.IsJumpAttack == false)
            {
                Player.IsJumpAttack = true;
                ChangeState(PlayerController.State.JumpAttack);
                break;
            }
            if (InputKey.GetButtonDown(InputKey.Melee) && Player.IsDoubleJump == true)
            {
                Player.IsDoubleJump = false;
                ChangeState(PlayerController.State.JumpDown);
                break;
            }
            yield return null;
        }
        _checkInputRoutine = null;
    }

    /// <summary>
    /// 지면에 가까운지 체크
    /// </summary>
    private bool CheckIsNearGround()
    {
        Vector3 CheckPos = new Vector3(transform.position.x, transform.position.y + 0.31f, transform.position.z);
        if (Physics.SphereCast(CheckPos, 0.3f, Vector3.down, out RaycastHit hit, 1f, Layer.EveryThing, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.gameObject.layer != Layer.Monster)

                return true;
        }
        return false;
    }
}
