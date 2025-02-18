using Assets.Project.Programmer.NSJ.RND.Script;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashObj : MonoBehaviour
{
    [SerializeField] bool onStun;

    [SerializeField] int atk;

    public int Atk { get { return atk; } }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == Tag.Monster)
        {
            Debug.Log(other.transform);
            IBattle hit = other.transform.GetComponentInParent<IBattle>();
            hit.TakeDamage(atk);
            if(onStun == true)
            {
                hit.TakeCrowdControl(CrowdControlType.Stun, 1f);
            }
            else
            {
                hit.TakeCrowdControl(CrowdControlType.Stiff, 1f);
            }
        }
    }
}
