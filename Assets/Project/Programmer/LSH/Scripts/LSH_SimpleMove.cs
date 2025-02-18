using BehaviorDesigner.Runtime.Tasks.Unity.UnityRigidbody;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSH_SimpleMove : MonoBehaviour
{
    [Range(1f, 20f)] [SerializeField] float speed;
    [SerializeField] Transform player;
    Vector3 movePos;

    private void Start()
    {
        speed = 10f;
    }


    private void Update()
    {
        Walk();
    
    }

    private void Walk()
    {

        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        movePos = new Vector3(x, 0, z);
        player.Translate(movePos);

    }

}
