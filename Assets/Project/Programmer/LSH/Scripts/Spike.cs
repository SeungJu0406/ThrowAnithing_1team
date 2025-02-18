using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public Coroutine spikeRoutine;

    [Range(0.1f, 1f)][SerializeField] float count;

    [SerializeField] int spikeDamage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        {

            IBattle battle = other.GetComponent<IBattle>();
            //hit.TakeDamage(spikeDamage, true);
            spikeRoutine = StartCoroutine(SpikeDamageRoutine(battle));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        {
            if (spikeRoutine != null)
            {
                StopCoroutine(spikeRoutine);
            }
        }

    }


    IEnumerator SpikeDamageRoutine(IBattle battle)
    {
        while (true)
        {
            battle.TakeDamage(spikeDamage);
            battle.TakeCrowdControl(CrowdControlType.Stiff, 1f);
            yield return count.GetDelay();
        }
    }
}
