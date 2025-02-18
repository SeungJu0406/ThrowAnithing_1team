using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroText : Intro
{
    TMP_Text myText;

    //Comment 반복문 돌리기 및 다음 스크립트 켜기용 변수
    int count;

    private void OnEnable()
    {

        for (count = 0; count < textArray.Length; count++)
        {
            //Comment 해당 스크립트가 textArary[i] 와 같으면 myText 변수에 textArray[i]를 할당
            if (this == textArray[count])
                myText = textArray[count].GetComponent<TMP_Text>();
        }

        // Comment : 텍스트스토리지에 텍스트 내용을 저장한 후 코루틴을 사용해 텍스트스토리지의 텍스트를 한글자씩 출력
        textStorage = myText.text;
        myText.text = " ";
        StartCoroutine(TextWriteCo(myText, textStorage));
        
    }

    private void OnDisable()
    {
        //Comment : textArray[connt]가 내 스크립트와 같으면 다음 인덱스 활성화 
        for (count = 0; count < textArray.Length; count++)
        {
            if (this == textArray[count])
            {
                if (count == textArray.Length - 1)
                {
                    //Todo: 비디오 재생
                    SceneManager.LoadScene("TitleScene");
                    return;
                }
                count++;
                textArray[count].gameObject.SetActive(true);
            }
        }
    }

    protected override void TextStart()
    {
        return;
    }
    protected override void NarrChange()
    {
        return;
    }

}
