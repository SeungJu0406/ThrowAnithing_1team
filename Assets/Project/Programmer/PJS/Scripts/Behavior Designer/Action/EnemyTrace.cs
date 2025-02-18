using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyTrace : Action
{
    [SerializeField] SharedFloat speed;         // 몬스터 이동속도
    [SerializeField] SharedTransform player;    // 플레이어
    [SerializeField] SharedFloat traceDist;     // 추격 인식 거리
    [SerializeField] SharedFloat attackDis;     // 공격 인식 거리
    public SharedBool onTakeHit;                // 데미지 받았는지

    public override TaskStatus OnUpdate()
    {
        float dir = (player.Value.position - transform.position).magnitude;

        if (dir < attackDis.Value)  // 공격 범위에 들어왔을 시
            return TaskStatus.Success;
        else if (dir > traceDist.Value && onTakeHit.Value == false) // 인식 범위 밖이고 피해를 안입었을 시
            return TaskStatus.Failure;
        else if (dir < traceDist.Value) // 피해를 입어서 인식범위에 들어왔을 시
            onTakeHit.SetValue(false);

        // x,z축만 추적
        Vector3 movePos = new(
            player.Value.position.x,
            transform.position.y,
            player.Value.position.z);
        // 좀비가 캐릭터에 너무 딱 붙으려는 문제 해결용?
        if ((movePos - transform.position).magnitude > attackDis.Value - 0.1f)
        {
            if (player.Value.position.y > 2f)   // 플레이어가 지정한 높이 위에 있을 시 멈춤
            {
                GetComponent<Animator>().SetBool("Move", false);
                transform.position = this.transform.position;
            }
            else
            {
                GetComponent<Animator>().SetBool("Move", true);
                transform.position = Vector3.MoveTowards(transform.position, movePos, speed.Value * Time.deltaTime);
            }
        }

        transform.LookAt(player.Value);
        // 각도 다시 잡아주기
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        return TaskStatus.Running;
    }
}
