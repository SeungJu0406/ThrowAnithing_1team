using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NSJArmChanger : MonoBehaviour
{
    [SerializeField] private ArmUnit _armUnit;
    [SerializeField] TMP_Text _nameText;

    private void Start()
    {
        _nameText.SetText(_armUnit.Name);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == Tag.Player)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            player.ChangeArmUnit(_armUnit);
        }
    }
}
