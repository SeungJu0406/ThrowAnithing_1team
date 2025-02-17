using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectPol : MonoBehaviour
{
    [SerializeField] GameObject prefab;


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ObjectPool.Get(prefab, 3f);
        }
    }
}
