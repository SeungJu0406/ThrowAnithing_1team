using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowDefaultObject : ThrowObject
{
    protected override void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
