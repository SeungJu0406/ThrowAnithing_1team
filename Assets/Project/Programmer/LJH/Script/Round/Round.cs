using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Round : MonoBehaviour
{
    [SerializeField] public int curRound;
    [SerializeField] string curScene;



    public static Round instance = null;


    private void Awake()
    {
        Singleton();
        Init();

    }

    private void Update()
    {
        RoundCal();
    }

    void RoundCal()
    {
        curScene = SceneManager.GetActiveScene().name;

        switch (curScene)
        {
            case SceneName.LobbyScene:
                curRound = 0;
                break;

            case SceneName.Stage1_1:
                curRound = 1;
                break;

            case SceneName.Stage1_2:
                curRound = 2;
                break;

            case SceneName.StageBoss:
                curRound = 3;
                break;

            default:
                curRound = 99;
                break;

        }
    }



    void Init()
    {

        switch(curScene)
        {
            case SceneName.LobbyScene:
                curRound = 0;
                break;

            case SceneName.Stage1_1:
                curRound = 1;
                break;

            case SceneName.Stage1_2:
                curRound = 2;
                break;

            case SceneName.StageBoss:
                curRound = 3;
                break;

        }
        
    }

    void Singleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
