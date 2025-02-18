using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToStage : MonoBehaviour
{
    public static void _ToStage()
    {
        if (SceneManager.GetActiveScene().name == SceneName.LobbyScene)
        {
            Loading.LoadScene(SceneName.Stage1_1);
        }
        else if (SceneManager.GetActiveScene().name == SceneName.Stage1_1)
        {
            Loading.LoadScene(SceneName.Stage1_2);
        }
        else if (SceneManager.GetActiveScene().name == SceneName.Stage1_2)
        {
            Loading.LoadScene(SceneName.StageBoss);
        }
    }
}
