using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Balance PrevMelee", menuName = "Arm/AttackType/Balance/PrevMelee")]
public class BalanceMeleeAttack : ArmMeleeAttack
{
    [SerializeField] GameObject _attackEffect;
    [Header("스테미나 소모량")]
    public float StaminaAmount;
    [SerializeField] float _range;
    [SerializeField] int _damage;
    [Range(0, 180)][SerializeField] float _angle;
    [SerializeField] float _damageMultiplier;
    [SerializeField] private float _coolTime;
    private float _staminaReduction => 1 - Model.StaminaReduction / 100;

    private bool _isCooltime;
    Coroutine _meleeRoutine;
    public override void Enter()
    {
        Player.Rb.velocity = Vector3.zero;

        if (_isCooltime == true)
        {
            Model.CurStamina += StaminaAmount;

            ChangeState(Player.PrevState);

            return;
        }


        View.SetTrigger(PlayerView.Parameter.BalanceMelee);



        if (Player.IsAttackFoward == true)
        {
            // 공격방향 바라보기
            Player.LookAtAttackDir();
        }
    }

    public override void Update()
    {
        //Debug.Log("PrevMelee");
    }

    public override void Exit()
    {
        if (_meleeRoutine != null)
        {
            CoroutineHandler.StopRoutine(_meleeRoutine);
            _meleeRoutine = null;
        }
    }
    public override void OnTrigger()
    {
        AttackMelee();
    }
    /// <summary>
    /// 근접 공격
    /// </summary>
    public void AttackMelee()
    {
        // 전방 앞에 있는 몬스터들을 확인하고 피격 진행
        // 1. 전방에 있는 몬스터 확인
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + Player.AttackHeight, transform.position.z);
        Vector3 attackPos = playerPos;
        int hitCount = Physics.OverlapSphereNonAlloc(attackPos, _range, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            // 2. 각도 내에 있는지 확인
            Vector3 source = transform.position;
            source.y = 0;
            Vector3 destination = Player.OverLapColliders[i].transform.position;
            destination.y = 0;

            Vector3 targetDir = (destination - source).normalized;
            //float targetAngle = Vector3.Angle(transform.forward, targetDir); // 아크코사인 필요 (느리다)
            //float targetAngle = Vector3.Dot(transform.forward, targetDir);
            float targetAngle = transform.forward.x * targetDir.x + transform.forward.y * targetDir.y + transform.forward.z * targetDir.z;
            targetAngle = Mathf.Acos(targetAngle) * Mathf.Rad2Deg;
            if (targetAngle > _angle * 0.5f)
                continue;

            int finalDamage = Player.GetFinalDamage(_damage, _damageMultiplier, out bool isCritical);

            // 사운드
            SoundManager.PlaySFX(isCritical == true ? Player.Sound.Hit.Critical : Player.Sound.Hit.Hit);

            // 데미지 주기
            Battle.TargetAttackWithDebuff(Player.OverLapColliders[i], isCritical, finalDamage, false);
            Battle.TargetCrowdControl(Player.OverLapColliders[i], CrowdControlType.Stiff);
        }


        CoroutineHandler.StartRoutine(CooltimeRoutine(_coolTime));

        ObjectPool.Get(_attackEffect, Player.MeleeAttackPoint.transform.position, transform.rotation, 2f);
    }

    public override void OnCombo()
    {
        if (_meleeRoutine == null)
        {
            _meleeRoutine = CoroutineHandler.StartRoutine(OnComboRoutine());
        }
    }

    public override void EndCombo()
    {
        if (_meleeRoutine != null)
        {
            ChangeState(PlayerController.State.Idle);
        }

    }



    IEnumerator OnComboRoutine()
    {
        while (true)
        {
            if (Player.CurState != PlayerController.State.MeleeAttack)
                yield break;


            if (InputKey.GetButtonDown(InputKey.Throw))
            {
                ChangeState(PlayerController.State.ThrowAttack);
                _meleeRoutine = null;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator CooltimeRoutine(float coolTime)
    {
        _isCooltime = true;
        yield return coolTime.GetDelay();
        _isCooltime = false;
    }

    public override void OnDrawGizmos()
    {
        if (Model == null)
            return;
        //거리
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y + Player.AttackHeight, transform.position.z);
        Vector3 attackPos = playerPos;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, _range);

        //각도
        Vector3 rightDir = Quaternion.Euler(0, _angle * 0.5f, 0) * transform.forward;
        Vector3 leftDir = Quaternion.Euler(0, _angle * -0.5f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + rightDir * _range);
        Gizmos.DrawLine(transform.position, transform.position + leftDir * _range);
    }
}
