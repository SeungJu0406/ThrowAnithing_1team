using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeText : MonoBehaviour
{
    Color backCol;
    Color textCol;
    [SerializeField] Canvas canvas;

    Coroutine textCo;

    private void OnEnable()
    {
        canvas = GameObject.FindWithTag(Tag.Canvas).GetComponent<Canvas>();
        gameObject.transform.SetParent(canvas.transform);
        backCol = gameObject.GetComponent<Image>().color;
        textCol = gameObject.GetComponentInChildren<TMP_Text>().color;

        GetComponent<Image>().raycastTarget = false;
    }

    private void OnDisable()
    {
        StopCoroutine(textCo);
        textCo = null;
    }

    private void Update()
    {
        TextHide();
        if(textCo == null)
        textCo = StartCoroutine(TextDelete());
    }

    void TextHide()
    {
        backCol.a -= 0.5f * Time.deltaTime;
        textCol.a -= 0.5f * Time.deltaTime;

        gameObject.GetComponent<Image>().color = backCol;
        gameObject.GetComponentInChildren<TMP_Text>().color = textCol;
    }

    IEnumerator TextDelete()
    {
        yield return 0.7f.GetRealTimeDelay();
        
        // 테스트 코드
        //gameObject.SetActive(false);
        
        Destroy(gameObject);
    }
}
