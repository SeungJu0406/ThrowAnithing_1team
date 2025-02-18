using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPopup : MonoBehaviour
{
    PlayerInput playerInput;

    [SerializeField] GameObject pause;
    [SerializeField] Button[] buttons;

    [SerializeField] SceneField mainScene;
    [SerializeField] SceneField lobbyScene;

    [SerializeField] GameObject exitImage;
    private void Start()
    {
        playerInput = InputKey.PlayerInput;

        buttons[0].onClick.AddListener(ExitScene);
        buttons[1].onClick.AddListener(Exit);
    }

    private void Update()
    {
        if (playerInput.actions["Choice"].WasPressedThisFrame())
            buttons[0].onClick.Invoke();

        if(playerInput.actions["Cancel"].WasPressedThisFrame())
            buttons[1].onClick.Invoke();
    }
    
    private void ExitScene()
    {
        // ·Îºñ¾À¿¡¼­´Â ¸ÞÀÎ¾ÀÀ¸·Î º¸³¿
        if (SceneManager.GetActiveScene().name == lobbyScene.SceneName)
        {
            LoadingToBase.LoadScene(mainScene);
        }
        else
        {
            LoadingToBase.LoadScene(lobbyScene);
        }
    }


    public void Exit()
    {
        gameObject.SetActive(false);
        EventSystem.current.SetSelectedGameObject(exitImage);
    }
}
