using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedHit : MonoBehaviour
{
    [SerializeField] List<AudioClip> trashHitClips;

    private BaseEnemy enemy;

    private void Awake()
    {
        enemy = transform.GetComponentInParent<BaseEnemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == Tag.Trash)
        {
            SoundManager.PlaySFX(enemy.ChoiceAudioClip(trashHitClips));
        }
    }
}
