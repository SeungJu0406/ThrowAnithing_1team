using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingTest : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        //  //Comment : �� �̸� �����������
        //  if (SceneManager.GetActiveScene().name == "GameSceneSta1")
        //  {
        //      if (other.transform.tag == Tag.Player)
        //          Loading.LoadScene("GameSceneSta1");
        //  }
        //  else if( SceneManager.GetActiveScene().name == "GameSceneSta2")
        //  {
        //      if (other.transform.tag == Tag.Player)
        //          Loading.LoadScene("GameSceneSta2");
        //  }
        //  else if (SceneManager.GetActiveScene().name == "GameSceneSta2")
        //  {
        //      if (other.transform.tag == Tag.Player)
        //          Loading.LoadScene("GameSceneBoss");
        //  }

        if (other.transform.tag == Tag.Player)
                Loading.LoadScene("GameSceneSta1");
    }

}
