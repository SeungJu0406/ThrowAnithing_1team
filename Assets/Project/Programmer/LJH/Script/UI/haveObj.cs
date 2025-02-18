using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class haveObj : MonoBehaviour
{
    public int maxObj { get; private set; }
    public int curObj { get; private set; }

    private void Start()
    {
        // Todo : 캐릭터에서 받아와야 함
        maxObj = 0;
        curObj = 0;
    }
    void Update()
    {
        gameObject.GetComponent<TMP_Text>().text = $"{curObj} / {maxObj}";

    }

}
