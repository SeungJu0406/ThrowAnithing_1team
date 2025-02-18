using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using UnityEngine;

public class TrapStalactite : MonoBehaviour
{
    // 레이로 검사할때 변수
    //[SerializeField] GameObject ray;
    //RaycastHit hit;
    //[SerializeField] float maxDistance = 15f;
    
    //레이를 쏘고있다가 or 박스 영역 안에
    //플레이어가 들어오면 가시 내림
    //가시는 떨어지기 시작하면 2초뒤에 사라짐
    //가시는 플레이어랑 부딪히면 체력 -40깎게함

    [SerializeField] GameObject stalactite;
    [SerializeField] GameObject boxArea;
    [SerializeField] GameObject particle;

    [SerializeField] Transform stalactiteBeforeMove;
    [SerializeField] Transform stalactiteAfterMove;

    Coroutine deleteRoutine;
    WaitForSeconds wait;
    [SerializeField] float deleteCount;


    // 레이로 검사하는 로직
    //private void Update()
    //{
    //    Debug.DrawRay(ray.transform.position, Vector3.down, Color.red);
    //    Debug.DrawLine(ray.transform.position, Vector3.down, Color.red);
    //    if (Physics.Raycast(ray.transform.position, Vector3.down, out hit, maxDistance))
    //    {
    //        if (hit.collider.gameObject.tag == Tag.Player)
    //        {
    //            stalactite.transform.position = 
    //                Vector3.MoveTowards(stalactiteBeforeMove.position, stalactiteAfterMove.position, 5f);
    //        }

    //    }


    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == Tag.Player)
        {
            stalactite.GetComponent<Rigidbody>().isKinematic = false;
            Coroutine deleteRoutine = StartCoroutine(DeleteRoutine());
        }
    }

    
    IEnumerator DeleteRoutine()
    {
        particle.SetActive(false);
        wait = new WaitForSeconds(deleteCount);
        yield return wait;
        stalactite.SetActive(false);
    }

}
