using Cinemachine;
using EPOOutline;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerBinder))]
public class PlayerView : MonoBehaviour
{
    public PlayerPanel Panel;
    public PlayerBinder Binder;
    public enum Parameter
    {
        AttackSpeed,
        Idle,
        Run,
        OnCombo,
        Jump,
        DoubleJump,
        Fall,
        DoubleJumpFall,
        Landing,
        BalanceJumpDown,
        PowerJumpDown,
        JumpAttack,
        Dash,
        DashEnd,
        Drain,
        Hit,
        Charge,
        ChargeEnd,
        ChargeCancel,
        BalanceMelee,
        BalanceThrow,
        BalanceSpecial1,
        BalanceSpecial2,
        BalanceSpecial3,
        PowerMelee,
        PowerThrow,
        PowerSpecial,
        OnBuff,
        Dead,
        Size
    }

    private bool _isAnimationFinish;
    public bool IsAnimationFinish
    {
        get
        {
            bool answer = _isAnimationFinish;
            if (_isAnimationFinish == true)
            {
                _isAnimationFinish = false;
            }
            return answer;
        }
        set
        {
            _isAnimationFinish = value;
        }
    }


    [SerializeField] Animator _animator;

    private int[] _animatorHashes = new int[(int)Parameter.Size];

    private void Awake()
    {
        Init();

        Camera.main.GetOrAddComponent<CinemachineBrain>();
        Camera.main.GetOrAddComponent<Outliner>();
        Camera.main.cullingMask = Layer.GetLayerMaskEveryThing();
        Camera.main.cullingMask &= ~(1<<Layer.IgnoreRaycast);
        Camera.main.cullingMask &= ~(1<< Layer.HideWall);
    }

    private void Update()
    {
        
    }
    // 애니메이션 =========================================================================================================//
    /// <summary>
    /// 플레이어 애니메이션 SetTrigger
    /// </summary>
    public void SetTrigger(Parameter animation)
    {
        _animator.SetTrigger(_animatorHashes[(int)animation]);
    }

    /// <summary>
    /// 플레이어 애니메이션 SetInteger
    /// </summary>
    public void SetInteger(Parameter animation, int value)
    {
        _animator.SetInteger(_animatorHashes[(int)animation], value);
    }

    public int GetInteger(Parameter animation)
    {
        return _animator.GetInteger(_animatorHashes[(int)animation]);
    }

    /// <summary>
    /// 플레이어 애니메이션 SetBool
    /// </summary>
    public void SetBool(Parameter animation, bool value)
    {
        _animator.SetBool(_animatorHashes[(int)animation], value);
    }

    public bool GetBool(Parameter animation)
    {
        return _animator.GetBool(_animatorHashes[(int)animation]);
    }

    /// <summary>
    /// 플레이어 애니메이션 SetFloat
    /// </summary>
    public void SetFloat(Parameter animation, float value)
    {
        _animator.SetFloat(_animatorHashes[(int)animation], value);
    }
    public float GetFloat(Parameter animation)
    {
        return _animator.GetFloat(_animatorHashes[(int)animation]);
    }

    // UI ================================================================================================================//

    public void UpdateText(TMP_Text target, string text)
    {
        if (target == null)
            return;

        target.SetText(text);
    }

    private void Init()
    {
        _animatorHashes[(int)Parameter.AttackSpeed] = Animator.StringToHash("AttackSpeed");
        _animatorHashes[(int)Parameter.Idle] = Animator.StringToHash("Idle");
        _animatorHashes[(int)Parameter.Run] = Animator.StringToHash("Run");
        _animatorHashes[(int)Parameter.OnCombo] = Animator.StringToHash("OnCombo");
        _animatorHashes[(int)Parameter.Jump] = Animator.StringToHash("Jump");
        _animatorHashes[(int)Parameter.DoubleJump] = Animator.StringToHash("DoubleJump");
        _animatorHashes[(int)Parameter.Fall] = Animator.StringToHash("Fall");
        _animatorHashes[(int)Parameter.Landing] = Animator.StringToHash("Landing");
        _animatorHashes[(int)Parameter.Dash] = Animator.StringToHash("Dash");
        _animatorHashes[(int)Parameter.DashEnd] = Animator.StringToHash("DashEnd");
        _animatorHashes[(int)Parameter.Drain] = Animator.StringToHash("Drain");
        _animatorHashes[(int)Parameter.Charge] = Animator.StringToHash("Charge");
        _animatorHashes[(int)Parameter.ChargeEnd] = Animator.StringToHash("ChargeEnd");
        _animatorHashes[(int)Parameter.ChargeCancel] = Animator.StringToHash("ChargeCancel");
        _animatorHashes[(int)Parameter.BalanceMelee] = Animator.StringToHash("BalanceMelee");
        _animatorHashes[(int)Parameter.BalanceThrow] = Animator.StringToHash("BalanceThrow");
        _animatorHashes[(int)Parameter.BalanceJumpDown] = Animator.StringToHash("BalanceJumpDown");
        _animatorHashes[(int)Parameter.PowerMelee] = Animator.StringToHash("PowerMelee");
        _animatorHashes[(int)Parameter.PowerThrow] = Animator.StringToHash("PowerThrow");
        _animatorHashes[(int)Parameter.PowerSpecial] = Animator.StringToHash("PowerSpecial");
        _animatorHashes[(int)Parameter.PowerJumpDown] = Animator.StringToHash("PowerJumpDown");
        _animatorHashes[(int)Parameter.BalanceSpecial1] = Animator.StringToHash("BalanceSpecial1");
        _animatorHashes[(int)Parameter.BalanceSpecial2] = Animator.StringToHash("BalanceSpecial2");
        _animatorHashes[(int)Parameter.BalanceSpecial3] = Animator.StringToHash("BalanceSpecial3");
        _animatorHashes[(int)Parameter.JumpAttack] = Animator.StringToHash("JumpAttack");
        _animatorHashes[(int)Parameter.Hit] = Animator.StringToHash("Hit");
        _animatorHashes[(int)Parameter.Dead] = Animator.StringToHash("Dead");
        _animatorHashes[(int)Parameter.OnBuff] = Animator.StringToHash("OnBuff");
        _animatorHashes[(int)Parameter.DoubleJumpFall] = Animator.StringToHash("DoubleJumpFall");
    }
}
