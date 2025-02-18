using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    protected string textStorage;
    [SerializeField] protected TMP_Text narr;
    [SerializeField] Image roy;

    [SerializeField] protected IntroText[] textArray;

    protected Coroutine textCo;

    private void Start()
    {
        TextStart();
        
    }
    private void Update()
    {
        NarrChange();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("TitleScene");

    }
    protected IEnumerator TextWriteCo(TMP_Text textBox, string textStorage)
    {
        int i = 0;
        while (i < textStorage.Length - 1)
        {
            textBox.text += textStorage[i].ToString();

            i++;

            yield return 0.05f.GetDelay();
        }

        StartCoroutine(TextOffCo());
    }

    protected IEnumerator TextOffCo()
    {
        yield return 0.3f.GetDelay();
        gameObject.SetActive(false);
    }

    protected virtual void TextStart()
    {
        textArray[0].gameObject.SetActive(true);
    }

    protected virtual void NarrChange()
    {
        narr.text =  "Narration";

        if (!textArray[0].gameObject.activeSelf)
        {
            narr.text = "Roy";
            roy.gameObject.SetActive(true);
        }
    }
}
