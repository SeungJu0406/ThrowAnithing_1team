using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InitializePlayer : MonoBehaviour
{
    [Inject]
    PlayerData _playerData;
    void Start()
    {
        Inventory.Instance.Controller.ItemReset();
        _playerData.ClearAdditional();
        _playerData.ClearThrowObject();
        _playerData.InitialzePlayerData();
    }
}
