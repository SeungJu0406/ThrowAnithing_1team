using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrainField : MonoBehaviour
{
    public  PlayerController _player;

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == Tag.Trash || other.gameObject.tag == Tag.Item)
        {
            Drain(other.transform);
        }
    }

    private void Drain(Transform trash)
    {
        //if (Player.Model.CurThrowables >= Player.Model.MaxThrowables)
        //    return;
        trash.position = Vector3.MoveTowards(trash.position, _player.transform.position, 5f * Time.deltaTime);
    }
}
