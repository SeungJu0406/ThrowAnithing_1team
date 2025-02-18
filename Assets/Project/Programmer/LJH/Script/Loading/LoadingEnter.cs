using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingEnter : MonoBehaviour
{
    void EnterStage()
    {
        //¾À Ä³½ÌÇØ¾ßÇÔ
        SceneManager.LoadScene(SceneName.ToStageLoading);
    }
}
