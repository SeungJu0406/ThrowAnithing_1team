using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealObject : MonoBehaviour
{
    [SerializeField] GameObject _effect;
    [Header("Èú ·®")]
    [SerializeField] private int _healAmount;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Tag.Player)
        {
            IHeal healable = other.gameObject.GetComponent<IHeal>();
            healable.TakeHeal(_healAmount);
            ObjectPool.Get(_effect, transform.position, transform.rotation, 2f);
            Destroy(gameObject);
        }
    }
}
