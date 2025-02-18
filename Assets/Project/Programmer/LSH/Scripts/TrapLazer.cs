using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TrapLazer : MonoBehaviour
{
    
    [SerializeField] GameObject lazer;

    [Range(5f, 10f)] [SerializeField] float count;
    WaitForSeconds OnblinkTiming;
    WaitForSeconds OffblinkTiming;
    Coroutine blinkRoutine;

    private void Start()
    {
        blinkRoutine = StartCoroutine(LazerBlinkRoutine());
    }


    IEnumerator LazerBlinkRoutine()
    {
        OnblinkTiming = new WaitForSeconds(count);
        OffblinkTiming = new WaitForSeconds(count*0.5f);

        while (true)
        {
            lazer.SetActive(true);
            yield return OnblinkTiming;

            lazer.SetActive(false);
            yield return OffblinkTiming;
        }

    }
}
