using System.Collections;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [HideInInspector] public BattleSystem Battle;
    public Transform target;
    public int Atk;     // 공격력

    private Rigidbody rigid;
    private float speed;    // 투척 스피드

    public float Speed { set { speed = value; } }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        transform.LookAt(target.position + Vector3.up);
        StartCoroutine(DestroyRoutine());
    }

    private void OnEnable()
    {
        // 오브젝트 풀을 사용하기에 오브젝트를 껏다 키는것임으로 Enable로 초기화 작업
        if(target != null)
        {
            transform.LookAt(target.position + Vector3.up);
            StartCoroutine(DestroyRoutine());
        }
    }

    private void FixedUpdate()
    {
        rigid.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == Tag.Player)
        {
            Battle.TargetAttack(other.transform, Atk, false);
            ObjectPool.Return(this);
        }

        ObjectPool.Return(this);
    }

    IEnumerator DestroyRoutine()
    {
        yield return 5f.GetDelay();
        ObjectPool.Return(this);
    }
}
