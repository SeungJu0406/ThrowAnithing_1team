using UnityEngine;

public class TrapSpike : MonoBehaviour
{

    [SerializeField] GameObject spike;
    [SerializeField] Spike spikeCS;
    [SerializeField] Transform spikeBeforeMove;
    [SerializeField] Transform spikeAfterMove;


    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == Tag.Player)
        {
            spike.transform.position =
                Vector3.MoveTowards(spikeBeforeMove.position, spikeAfterMove.position, 1f);

        }

    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.tag == Tag.Player)
        {            
            spike.transform.position =
                Vector3.MoveTowards(spikeAfterMove.position, spikeBeforeMove.position, 1f);
            
        }

    }



}
