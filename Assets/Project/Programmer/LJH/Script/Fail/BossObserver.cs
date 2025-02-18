using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BossObserver : MonoBehaviour
{
    [SerializeField] GameObject _portal;
    [SerializeField] GameObject _boss;


    private void Start()
    {
        _portal.SetActive(false);

        this.UpdateAsObservable()
            .Where(x => _boss.activeSelf == false)
            .First()
            .Subscribe( x => { _portal.SetActive(true); });
    }
}
