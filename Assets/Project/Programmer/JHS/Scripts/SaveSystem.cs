using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SaveSystem : MonoBehaviour
{
    [Inject]
    GlobalGameData gameData;
    [Inject]
    UserDataManager userDataManager;

    private void init()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void GetCoin(int getcoin)
    {
        gameData.GetCoin(getcoin);
        userDataManager.SaveData();
    }

    public void SavePlayerData()
    {
        userDataManager.SaveData();
    }
}
