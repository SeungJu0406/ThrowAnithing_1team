using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToBase : MonoBehaviour
{
    public static void ToLobby()
    {
        LoadingToBase.LoadScene(SceneName.LobbyScene);
    }
}
