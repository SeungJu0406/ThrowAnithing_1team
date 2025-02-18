using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LJH_inputManager : MonoBehaviour
{
    public float maxHp = 100;
    public float curHp;

    public float maxMp = 200;
    public float curMp;

    public float maxSta = 50;
    public float curSta;

    public float maxCharging;
    public float curCharging;


    float moveSpeed = 3f;
    Vector3 preFor = Vector3.forward;

    [SerializeField] GameObject pause;

    private void Start()
    {
        curHp = maxHp;
        curMp = maxMp;
        curSta = maxSta;
        curCharging = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(curCharging);
            Debug.Log(maxCharging);
        }


        maxCharging = curMp;
        if (curCharging < maxCharging)
            if (Input.GetKey(KeyCode.Q))
                curCharging += 1f;

        if (Input.GetKeyUp(KeyCode.Q))
            curCharging = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            curHp -= 10;
            curMp -= 10;
            curSta -= 10;
        }
        if (!pause.activeSelf)
        {
            Move();
            Rotation();
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        transform.Translate(new Vector3(x, 0, z) * moveSpeed * Time.deltaTime, Space.World);
    }

    void Rotation()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (x != 0 || z != 0)
        {
            preFor = new Vector3(x, 0, z);
        }

        transform.forward = new Vector3(x, 0, z);

        if (x == 0 && z == 0)
            transform.forward = preFor;
    }
}
