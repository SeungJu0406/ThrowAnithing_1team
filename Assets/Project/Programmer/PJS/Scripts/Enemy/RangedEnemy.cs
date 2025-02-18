using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : BaseEnemy
{
    [Header("공격 효과음")]
    [Tooltip("좀비 효과음")]
    [SerializeField] AudioClip attackClip;
    [Tooltip("투사체 효과음")]
    [SerializeField] List<AudioClip> bulletClips;
    [Header("피격 모션 쿨타임")]
    [SerializeField] float hitCoolTime;
    [Header("투사체 속도")]
    [SerializeField] float bulletSpeed;
    [Header("투사체 정보")]
    [SerializeField] EnemyBullet bulletPrefab;
    [SerializeField] Transform muzzle;

    public float BulletSpeed { get { return bulletSpeed; } }


    private void Start()
    {
        BaseInit();
        tree.SetVariableValue("HitCoolTime", hitCoolTime);
    }

    public void Attack()
    {
        SoundManager.PlaySFX(attackClip);
        EnemyBullet bulletPool = ObjectPool.Get(bulletPrefab, muzzle.position, muzzle.rotation);
        bulletPool.target = playerObj.Value.transform;
        bulletPool.Speed = bulletSpeed;
        bulletPool.Atk = state.Atk;
        bulletPool.Battle = Battle;
        SoundManager.PlaySFX(ChoiceAudioClip(bulletClips));
    }
}
