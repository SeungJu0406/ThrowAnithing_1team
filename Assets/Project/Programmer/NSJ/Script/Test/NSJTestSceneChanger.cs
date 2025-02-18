using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NSJTestSceneChanger : BaseUI
{
    [SerializeField] SceneField _nextScene;
    private void Awake()
    {
        Bind();

        GetUI<Button>("Button").onClick.AddListener(() => SceneManager.LoadSceneAsync(_nextScene));
    }
}
