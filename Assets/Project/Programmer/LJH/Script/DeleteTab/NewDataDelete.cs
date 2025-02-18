using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class NewDataDelete : MonoBehaviour
{
    [SerializeField] SlotManager slotManager;

    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    PlayerInput playerInput; 

    void Start()
    {
        playerInput = InputKey.PlayerInput;
        Debug.Log(yesButton.onClick.ToString());
        Debug.Log(noButton.onClick.ToString());
    }

    void Update()
    {
        if (playerInput.actions["Choice"].WasPressedThisFrame())
        {
            Debug.Log("삭제됐음");
            yesButton.onClick.Invoke();
        }
        if (playerInput.actions["Cancel"].WasPressedThisFrame())
        {
            Debug.Log("삭제취소");
            noButton.onClick.Invoke();
        }
    }

    

    
}
