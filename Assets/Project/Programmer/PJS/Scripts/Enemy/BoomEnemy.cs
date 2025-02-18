using System.Collections.Generic;
using UnityEngine;

public class BoomEnemy : MeleeEnemy
{
    [Tooltip("기 모으는 파티클")]
    [SerializeField] ParticleSystem chargeParticle;

    public void BeginCharge()
    {
        chargeParticle.Play();
    }
}
