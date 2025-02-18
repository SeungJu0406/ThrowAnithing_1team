using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stalactite : MonoBehaviour
{

    [SerializeField] int stalactiteDamage;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        { 
            IBattle battle = other.GetComponent<IBattle>();
            battle.TakeDamage(stalactiteDamage);
            battle.TakeCrowdControl(CrowdControlType.Stiff, 1f);
        }
    }

}
