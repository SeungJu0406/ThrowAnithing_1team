using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class AudioPlay : Action
{
	public AudioClip fistChargeClip;

    public override void OnStart()
    {
		SoundManager.PlaySFX(fistChargeClip);
    }

    public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}