using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerBinder : BaseBinder
{

    private void Awake()
    {
        Bind();
        InitField();
    }

    private void InitField()
    {

    }
}
