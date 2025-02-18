using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour
{
    [SerializeField] Button firstButton;
    Canvas me;
    [SerializeField] Canvas canvas;

    private void Start()
    {
        me = GetComponent<Canvas>();

        
    }
    private void Update()
    {
    }
    public void Test()
    {
        Debug.Log("´­·È¾î¿ë");
    }

    public void ExitTab()
    {
        canvas.gameObject.SetActive(true);
        me.gameObject.SetActive(false);
    }


    public void OnChoIce()
    {
        //EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        Debug.Log("´­·È¾î¿ë");
    }
}
