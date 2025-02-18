using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class BossAttack : Action
{
    [SerializeField] Animator anim;

    public override void OnAwake()
    {
        anim = GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        anim.SetTrigger("AttackT");
        anim.SetInteger("AttackID", 1);
        return TaskStatus.Success;
    }
}