using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NSJ_TesterPanel
{
    public class TesterMonster : MonoBehaviour
    {
        public void Die()
        {
            IBattle battle = GetComponent<IBattle>();
            battle.TakeDamage(10000000);
        }
    }
}