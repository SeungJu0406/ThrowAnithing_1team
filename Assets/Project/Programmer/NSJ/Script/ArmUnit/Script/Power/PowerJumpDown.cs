using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Power PowerJumpDown", menuName = "Arm/AttackType/Power/PowerJumpDown")]
public class PowerJumpDown : ArmJumpDown
{
    [SerializeField] GameObject _attackEffect;
    [Range(0, 20)]
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _range;
    [SerializeField] private int _damage;
    [SerializeField] private float _maxScaleEffectTime;

    private Vector3 _landingPoint;
    Coroutine _fallRoutine;
    public override void Enter()
    {
        Player.Rb.velocity = Vector3.zero;
        Player.Rb.AddForce(Vector3.down * _attackSpeed, ForceMode.Impulse);

        // 플레이어 무적
        Player.IsInvincible = true;

        View.SetTrigger(PlayerView.Parameter.PowerJumpDown);

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

        // 플레이어 무적 해제
        Player.IsInvincible = false;

        // 점프 조건 해제
        Player.IsJumpAttack = false;
        Player.IsDoubleJump = false;
    }

    public override void Update()
    {
        if (Player.IsGround)
        {
            Rb.velocity = Vector3.zero;
        }
    }



    public override void EndAnimation()
    {
        ChangeState(PlayerController.State.Idle);
    }

    IEnumerator FallRoutine()
    {
        while (Player.IsGround == false)
        {
            // 떨어지기 시작했을때 지면과 충분히 가까워졌다면 지면 체크 로직 종료
            if (Rb.velocity.y < 0)
            {
                Vector3 CheckPos = new Vector3(transform.position.x, transform.position.y + 0.31f, transform.position.z);
                if (Physics.SphereCast(CheckPos, 0.3f, Vector3.down, out RaycastHit hit, _attackSpeed / (_attackSpeed / 2), Layer.EveryThing, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform.gameObject.layer != Layer.Monster)
                    {
                        _landingPoint = hit.point;
                        break;
                    }              
                }

            }
            yield return null;
        }

        View.SetTrigger(PlayerView.Parameter.Landing);

        // 사운드 
        SoundManager.PlaySFX(Player.Sound.Move.JumpAttack);

        // 플레이어가 실제로 땅에 닿았을때까지 대기
        while (Player.IsGround == false)
        {
            yield return null;
        }
        // 데미지 주기
        AttackJumpDown();
    }

    private void AttackJumpDown()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(_landingPoint, _range, Player.OverLapColliders, 1 << Layer.Monster);
        int finalDamage = Player.GetFinalDamage(_damage, out bool isCritical);
        for (int i = 0; i < hitCount; i++)
        {
            // 데미지 주기
            Battle.TargetAttackWithDebuff(Player.OverLapColliders[i], isCritical, finalDamage, false);
            Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stiff);
            // 넉백
            Player.DoKnockBack(Player.OverLapColliders[i].transform, transform, 1f);
        }


        // 이펙트 생성
        ObjectPool.Get(_attackEffect, _landingPoint, Quaternion.identity, 5f);

    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
    }
}