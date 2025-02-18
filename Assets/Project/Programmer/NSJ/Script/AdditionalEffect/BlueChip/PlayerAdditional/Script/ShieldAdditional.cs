using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shield", menuName = "AdditionalEffect/Player/Shield")]
public class ShieldAdditional : PlayerAdditional
{
    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
    }
    [SerializeField] EffectStrcut _effect;

    [Header("쉴드 재생성 시간")]
    [SerializeField] private float _regenerateTime;

    private bool _isShield;
    Coroutine _regenerateShieldRoutine;
    public override void Enter()
    {
        Player.OnPlayerHitActionEvent += HitPlayerWithShield;

        _effect.Effect = Instantiate(_effect.EffectPrefab, transform);
        CreateShield();
    }

    public override void Exit()
    {
        Player.OnPlayerHitActionEvent -= HitPlayerWithShield;

        // 코루틴 종료
        if( _regenerateShieldRoutine != null)
        {
            CoroutineHandler.StopRoutine(_regenerateShieldRoutine);
            _regenerateShieldRoutine = null;
        }
        // 쉴드가 있었으면 쉴드 파괴
        if( _isShield == true)
        {
            DestroyShield();
        }
        Destroy(_effect.Effect);
    }

    private void CreateShield()
    {
        Player.IsShield = true;
        _effect.Effect.SetActive(true);
    }
    private void DestroyShield()
    {
        Player.IsShield = false;
        _effect.Effect.SetActive(false);
    }

    private void HitPlayerWithShield()
    {
        if (_regenerateShieldRoutine == null)
            _regenerateShieldRoutine = CoroutineHandler.StartRoutine(RegenerateShieldRoutine());
    }

    IEnumerator RegenerateShieldRoutine()
    {
        // 1프레임 대기(피격 로직 먼저 받아야함)
        yield return null;
        // 쉴드 파괴
        DestroyShield();
        // 쉴드 재생성 대기
        yield return _regenerateTime.GetDelay();
        CreateShield();
        _regenerateShieldRoutine = null;
    }
}
