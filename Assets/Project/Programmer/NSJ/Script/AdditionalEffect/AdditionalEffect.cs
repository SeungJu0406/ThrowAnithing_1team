using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalEffect : ScriptableObject
{
    public Sprite Image;
    public AdditionalEffect Origin;
    public enum Type { Throw, Hit, Player }

    public Type AdditionalType;

    [Space(20)]
    public string Name;
    [TextArea]
    public string Description;


    /// <summary>
    /// 적용 시 호출
    /// </summary>
    public virtual void Enter() { }
    /// <summary>
    /// 해제 시 호출
    /// </summary>
    public virtual void Exit() { }
    /// <summary>
    /// 상태 진입 시 호출
    /// </summary>
    public virtual void EnterState() { }
    /// <summary>
    /// 상태 퇴장 시 호출
    /// </summary>
    public virtual void ExitState() { }
    /// <summary>
    /// 프레임마다 호출
    /// </summary>
    public virtual void Update() { }

    public virtual void LateUpdate()
    {

    }

    /// <summary>
    /// 0.02초마다 호출
    /// </summary>
    public virtual void FixedUpdate() { }
    /// <summary>
    /// 트리거 됬을 때만 호출
    /// </summary>
    public virtual void Trigger() { }
}
