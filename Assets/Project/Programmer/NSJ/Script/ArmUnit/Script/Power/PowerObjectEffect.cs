using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerObjectEffect : MonoBehaviour
{
    Animator _animator;

    int _nextStepHash;
    int _endHash;
    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _nextStepHash = Animator.StringToHash("Next");
        _endHash = Animator.StringToHash("End");
    }


    public void Next()
    {
        _animator.SetTrigger(_nextStepHash);
    }
    public void End()
    {
        _animator.SetTrigger(_endHash);
    }
    
}
