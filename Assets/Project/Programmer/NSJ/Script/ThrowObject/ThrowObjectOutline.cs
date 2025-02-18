using EPOOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObjectOutline : MonoBehaviour
{
    [Header("아웃라인이 뜨는 최대 거리")]
    [SerializeField] private float _maxDistance = 20;

    private Outlinable[] _outlinables;
    private Transform _player;

    Coroutine _detectPlayerRoutine;
    private void Awake()
    {
        _outlinables = GetComponentsInChildren<Outlinable>();
        _player = GameObject.FindGameObjectWithTag(Tag.Player).transform;
    }

    private void OnEnable()
    {
        if(_detectPlayerRoutine == null)
        {
            _detectPlayerRoutine = StartCoroutine(DetectPlayerRoutine());
        }

    }

    private void OnDisable()
    {
        if (_detectPlayerRoutine != null)
        {
            StopCoroutine( _detectPlayerRoutine );
            _detectPlayerRoutine = null;
        }

    }


    IEnumerator DetectPlayerRoutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if(distance < _maxDistance) 
            {
                if (_outlinables[0].enabled == false)
                {
                    foreach (Outlinable outlinable in _outlinables)
                    {
                        outlinable.enabled = true;
                    }
                }                                  
            }
            else
            {
                if (_outlinables[0].enabled == true)
                {
                    foreach (Outlinable outlinable in _outlinables)
                    {
                        outlinable.enabled = false;
                    }
                }
            }
            yield return 1f.GetDelay();
        }
    }

}
