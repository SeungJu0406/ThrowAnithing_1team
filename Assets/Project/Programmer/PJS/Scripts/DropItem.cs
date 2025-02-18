using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] Rigidbody rigid;
    [Range(0f, 1f)]
    [SerializeField] float num;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Vector3 dir = new Vector3(Random.Range(-num, num), 5, Random.Range(-num, num));
        Debug.Log(dir);
        rigid.AddForce(dir, ForceMode.Impulse);
    }
}
