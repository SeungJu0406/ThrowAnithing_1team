using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimator;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class PortalSceneNumber : MonoBehaviour
{
    [SerializeField] public SceneField nextScene;
    [SerializeField] public SceneField[] hiddenSceneArr;


     private Forge _forge;

    private void Awake()
    {
        _forge = GetComponentInParent<Forge>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tag.Player)
        {
            if (gameObject.tag == Tag.PortalHidden)
            {
                Destroy(_forge.gameObject);
            }
        }
    }
}
