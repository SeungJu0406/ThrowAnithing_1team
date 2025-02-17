using Coffee.UIExtensions;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
     UIParticle uiParticle;

     [SerializeField]ParticleSystem cursor;

    private void Awake()
    {
        uiParticle = GetComponentInChildren<UIParticle>();
    }

    private void OnEnable()
    {
        uiParticle.transform.position = Input.mousePosition;
        ParticleSystem particle = ObjectPool.Get(cursor, uiParticle.transform, 2f);
        uiParticle.RefreshParticles();
    }
}
