using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndForge : MonoBehaviour
{
    [SerializeField] GameObject upPopup;
    [SerializeField] GameObject _ui;

    public bool IsUIActive;
    public bool IsActive;
    PlayerController _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag(Tag.Player).GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            SoundManager.PlaySFX(SoundManager.Data.UI.Win);
            upPopup.SetActive(true);
            IsActive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Tag.Player))
        {
            upPopup.SetActive(false);
        }
    }
}
