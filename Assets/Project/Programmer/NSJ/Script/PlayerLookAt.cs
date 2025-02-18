using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookAt : MonoBehaviour
{
    GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag(Tag.Player);
    }

    private void Update()
    {
        if (_player == null)
            return;


    }
}
