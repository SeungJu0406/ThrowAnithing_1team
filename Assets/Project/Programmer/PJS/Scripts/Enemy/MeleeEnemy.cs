using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [Header("공격 효과음")]
    [SerializeField] List<AudioClip> attackClips;
    [Header("피격 모션 쿨타임")]
    [SerializeField] float hitCoolTime;

    private void Start()
    {
        BaseInit();
        tree.SetVariableValue("HitCoolTime", hitCoolTime);
    }

    // 공격 애니메이션 이벤트
    public void BeginAttack()
    {
        AttackMelee();
        SoundManager.PlaySFX(ChoiceAudioClip(attackClips));
    }
    public void EndAttack()
    {
    }

    /// <summary>
    /// 근접 공격
    /// </summary>
    public void AttackMelee()
    {
        // 전방 앞에 있는 몬스터들을 확인하고 피격 진행
        // 1. 전방에 있는 몬스터 확인
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 attackPos = playerPos;
        int hitCount = Physics.OverlapSphereNonAlloc(attackPos, state.AttackDis, overLapCollider, 1 << Layer.Player);
        for (int i = 0; i < hitCount; i++)
        {
            // 2. 각도 내에 있는지 확인
            Vector3 source = transform.position;
            source.y = 0;
            Vector3 destination = overLapCollider[i].transform.position;
            destination.y = 0;

            Vector3 targetDir = (destination - source).normalized;
            float targetAngle = Vector3.Angle(transform.forward, targetDir); // 아크코사인 필요 (느리다)
            if (targetAngle > 120 * 0.5f)
                continue;

            int finalDamage = state.Atk;
            // 데미지 주기
            Battle.TargetAttackWithDebuff(overLapCollider[i], finalDamage);
            Battle.TargetCrowdControl(overLapCollider[i], CrowdControlType.Stiff);
        }
    }
}
