using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MKH
{
    public class BlueChipChoicePopUpTest : MonoBehaviour
    {
        [SerializeField] Button button;

        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }
    }
}
