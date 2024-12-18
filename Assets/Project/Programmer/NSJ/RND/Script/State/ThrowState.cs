using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : PlayerState
{
    private Transform _muzzlePoint;
    private float _atttackBufferTime;
    private bool _isCombe;
    private bool _isChangeAttack;
    public ThrowState(PlayerController controller) : base(controller)
    {
        _atttackBufferTime = Player.AttackBufferTime;
        _muzzlePoint = controller.MuzzletPoint;

        View.OnThrowAttackEvent += ThrowObject;
    }
    public override void Enter()
    {
        Player.Rb.velocity = Vector3.zero;
        _isChangeAttack = false;
        if (View.GetBool(PlayerView.Parameter.ThrowCombo) == false)
        {
            View.SetTrigger(PlayerView.Parameter.ThrowAttack);
        }
        else
        {
            Model.MeleeComboCount++;
        }
        CoroutineHandler.StartRoutine(MeleeAttackRoutine());
    }

    public override void Update()
    {
        //Debug.Log("Melee");
    }

    public override void Exit()
    {

    }


    /// <summary>
    /// 오브젝트 던지기 공격
    /// </summary>
    public void ThrowObject()
    {
        if (Model.ThrowObjectStack.Count > 0)
        {
            ThrowObjectData data = Model.PopThrowObject();
            ThrowObject throwObject = Player.InstantiateObject(DataContainer.GetThrowObject(data.ID), _muzzlePoint.position, _muzzlePoint.rotation);
            throwObject.Init(Model.Damage, Model.BoomRadius, Model.HitAdditionals);
            throwObject.Shoot();
        }
    }
    IEnumerator MeleeAttackRoutine()
    {
        if (Player.IsAttackFoward == true)
        {
            // 카메라 방향으로 플레이어가 바라보게
            Quaternion cameraRot = Quaternion.Euler(0, Player.CamareArm.eulerAngles.y, 0);
            transform.rotation = cameraRot;
            // 카메라는 다시 로컬 기준 전방 방향
            if (Player.CamareArm.parent != null)
            {
                Player.CamareArm.localRotation = Quaternion.Euler(Player.CamareArm.localRotation.eulerAngles.x, 0, 0);
            }
        }

        yield return null;
        float timeCount = _atttackBufferTime;
        while (View.IsAnimationFinish == false)
        {
            // 공격 버퍼
            if (Input.GetButtonDown("Fire2"))
            {
                // 다음 공격 대기
                _isCombe = true;
                View.SetBool(PlayerView.Parameter.ThrowCombo, true);
                timeCount = _atttackBufferTime;
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                // 근접 공격 전환
                _isCombe = false;
                _isChangeAttack = true;
                View.SetBool(PlayerView.Parameter.ThrowCombo, false);
                timeCount = _atttackBufferTime;
            }
            timeCount -= Time.deltaTime;
            if (timeCount <= 0)
            {
                // 다음 공격 취소
                _isCombe = false;
                View.SetBool(PlayerView.Parameter.ThrowCombo, false);
                timeCount = _atttackBufferTime;
            }

            yield return null;
        }

        // 콤보 선입력이 되었을 때 다시 투척 공격
        if (_isCombe == true)
        {
            ChangeState(PlayerController.State.ThrowAttack);
        }
        // 공격 키 입력이 바뀌었을 때 근접공격
        else if (_isChangeAttack == true)
        {
            Model.MeleeComboCount = 0;
            ChangeState(PlayerController.State.MeleeAttack);
        }
        // 아무 입력도 없었을 때 평상시 모드
        else
        {
            Model.MeleeComboCount = 0;
            ChangeState(PlayerController.State.Idle);
        }

    }
}
