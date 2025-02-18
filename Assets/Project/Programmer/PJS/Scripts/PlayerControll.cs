using Assets.Project.Programmer.NSJ.RND.Script;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattleSystem))]
public class PlayerControll : MonoBehaviour, IHit, IDebuff
{
    [HideInInspector] public BattleSystem Battle;
    [SerializeField] float speed;

    public int maxHp;
    public int curHp;
    public float jumpPower;
    public float attackSpeed;

    public int MaxHp { get => maxHp; set => maxHp = value; }
    public int CurHp { get => curHp; set => curHp = value; }
    public float MoveSpeed { get => speed; set => speed = value; }
    public float JumpPower { get => jumpPower; set => jumpPower = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }

    private void Awake()
    {
        Battle = GetComponent<BattleSystem>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(x, 0, z);

        transform.Translate(dir.normalized * speed * Time.deltaTime);
    }



    public int TakeDamage(int damage, bool isIgnoreDef)
    {
        Debug.Log($"{damage} 만큼의 피해를 입음");
        return damage;
    }

    public void TakeCrowdControl(CrowdControlType type, float time)
    {
        
    }
}
