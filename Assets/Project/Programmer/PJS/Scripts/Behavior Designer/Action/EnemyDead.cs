using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : BaseAction
{
    [SerializeField] Animator anim;
    public List<AudioClip> deathClips = new List<AudioClip>();
    private bool _isFirst;

    public override void OnAwake()
    {
        anim = GetComponent<Animator>();
        baseEnemy = GetComponent<BaseEnemy>();
        foreach (AudioClip clip in baseEnemy.GetDaethClips())
        {
            deathClips.Add(clip);
        }
    }

    public override void OnStart()
    {
        SoundManager.PlaySFX(baseEnemy.ChoiceAudioClip(deathClips));
    }

    public override TaskStatus OnUpdate()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Zombie Death") &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            return TaskStatus.Success;
        }

        if (_isFirst == false)
        {
            _isFirst = true;

            BattleSystem battle = GetComponent<BattleSystem>();
            battle.Die();

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            Collider[] colliders = transform.GetComponentsInChildren<Collider>();
            foreach (Collider item in colliders)
            {
                item.enabled = false;
            }
        }

        return TaskStatus.Running;
    }
}