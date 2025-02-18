using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradePopUp : MonoBehaviour
{
    PlayerInput playerInput;

    [SerializeField] GameObject _object;
    //[SerializeField] GameObject player;
    private void Start()
    {
        playerInput = InputKey.PlayerInput;
    }

    private void Update()
    {
        if(gameObject.activeSelf&& _object.activeSelf == false && Time.deltaTime != 0)
        {
            if (InputKey.GetButtonDown(InputKey.Interaction))
            {
                
                Debug.Log($"{name} ´©¸§");
                _object.SetActive(true);
            }
              
        }
    }

}
