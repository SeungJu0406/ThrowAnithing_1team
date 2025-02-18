using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueChip : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + 180, 0);
    }
}
