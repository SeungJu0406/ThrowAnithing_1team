using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LSH_Player : MonoBehaviour
{
    [SerializeField] private int _hp;
    public int HP { get { return _hp; } private set { } }


    private void Start()
    {
        _hp = 100;
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
    }


}


