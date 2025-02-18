using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseLoadingEnter : MonoBehaviour
{
    void EnterBase()
    {
        SceneManager.LoadScene(SceneName.ToLobbyLoading);
    }
}
