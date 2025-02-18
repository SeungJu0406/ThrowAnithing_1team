using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NSJBlueChipRemover : MonoBehaviour
{
    [SerializeField] bool _isClearMode;
    [SerializeField] AdditionalEffect effect;
    [SerializeField] TMP_Text nameText;

    private void Start()
    {
        nameText.SetText($"Remove {effect.Name}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        {
           PlayerController player = other.gameObject.GetComponent<PlayerController>();

            if (_isClearMode)
            {
                player.ClearAdditional();
            }


            if (player.Model.AdditionalEffects.Count > 0)
            {
                player.RemoveAdditional(effect);
            }
            
            // Destroy(gameObject);
        }
    }
}
