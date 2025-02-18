using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class FailPanel : MonoBehaviour
{
    [Inject]
    private PlayerData playerData;
    [SerializeField] Failure failure;
    [SerializeField] Result result;

    bool _isFirst;
    private void Start()
    {
        this.UpdateAsObservable()
            .Where(x => playerData.IsDead == true)
            .Where(x => _isFirst == false)
            .Subscribe(x => 
            { 
                failure.gameObject.SetActive(true); 
                SoundManager.PlaySFX(SoundManager.Data.UI.Lose);
                _isFirst = true;
            });
    }
}
