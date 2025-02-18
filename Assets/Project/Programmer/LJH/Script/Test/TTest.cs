using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TTest : MonoBehaviour
{
    [SerializeField] List<Button> buttons;

    GameObject preButton;
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject)
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.red;

        if(EventSystem.current.isActiveAndEnabled)
            EventSystem.current.currentSelectedGameObject.GetComponent<Image>().color = Color.green;

    }
}
