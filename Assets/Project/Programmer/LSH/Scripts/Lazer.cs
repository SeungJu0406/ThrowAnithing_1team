using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Lazer : MonoBehaviour
{

    public Coroutine damageRoutine;

     WaitForSeconds wait;
    [Range(1f, 10f)][SerializeField] float timer;

    [SerializeField] int lazerDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        {
            IBattle battle = other.GetComponent<IBattle>();
            //hit.TakeDamage(spikeDamage, true);
            damageRoutine = StartCoroutine(DamageRoutine(battle));
        }

    }


    private void OnTriggerExit(Collider other)
    {
        if(damageRoutine != null)
            StopCoroutine(damageRoutine);
    }

    private void OnDisable()
    {
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);
    }


    IEnumerator DamageRoutine(IBattle battle)
    {
        while (true)
        {
            battle.TakeDamage(lazerDamage);
            battle.TakeCrowdControl(CrowdControlType.Stiff,1f);
            yield return timer.GetDelay();
        }
    }







}
