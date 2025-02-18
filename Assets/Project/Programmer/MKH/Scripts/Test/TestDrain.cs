using UnityEngine;

public class TestDrain : MonoBehaviour
{
    [SerializeField] Transform player;

    private void Update()
    {
        transform.position = player.position + new Vector3(0, -1, 0);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == Tag.Item)
        {
            Drain(other.transform);
        }
    }

    public void Drain(Transform item)
    {
        item.position = Vector3.MoveTowards(item.position, player.position, 5f * Time.deltaTime);
    }
}
