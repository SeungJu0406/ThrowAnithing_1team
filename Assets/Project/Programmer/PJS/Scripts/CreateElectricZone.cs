using UnityEngine;

[RequireComponent(typeof(BattleSystem))]
public class CreateElectricZone : MonoBehaviour
{
    [HideInInspector] public BattleSystem battle;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == Tag.Player)
        {
            battle.TargetDebuff(other.transform);
        }
    }
}
