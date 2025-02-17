using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TheortOfRelativity", menuName = "AdditionalEffect/Player/TheortOfRelativity")]
public class TheortOfRelativityAdditional : PlayerAdditional
{

    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
        public float EffectDuration;
    }
    [SerializeField] EffectStrcut _effect;
    [SerializeField] private SlowAddtional _slowOrigin;
    private SlowAddtional _slow;
    [Header("확률(%)")]
    [Range(0, 100)]
    [SerializeField] private float _probability;
    [Header("공격속도 증가량(%)")]
    [SerializeField] private float _increaseAttackSpeed;
    [Header("이동속도 증가량(%)")]
    [SerializeField] private float _increaseMoveSpeed;
    [Header("버프 시간")]
    [SerializeField] private float _buffDuration;
    [Header("슬로우 범위")]
    [SerializeField] private float _slowRange;
    [Header("적 이속감소량")]
    [SerializeField] private float _slowAmount;
    [Header("디버프 시간")]
    [SerializeField] private float _slowDuration;


    private bool _isBuff;
    Coroutine _buffRoutine;
    public override void Enter()
    {
        _slow = Instantiate(_slowOrigin); 
        _slow.Duration = _slowDuration;
        _slow.SlowAmount = _slowAmount;
    }


    public override void Exit()
    {
        // 제거 시에 버프 정리
        if (_buffRoutine != null)
        {
            CoroutineHandler.StopRoutine(_buffRoutine);
            _buffRoutine = null;
        }
        if (_isBuff ==true)
        {
            Model.AttackSpeedMultiplier -= _increaseAttackSpeed;
            Model.MoveSpeedMultyplier -= _increaseMoveSpeed;
        }
        Destroy(_slow);
    }

    public override void Trigger()
    {
        if (CurState != PlayerController.State.SpecialAttack)
            return;

        if (Random.Range(0, 100) < _probability)
        {
            Process();
        }
    }

    private void Process()
    {
        ProcessPlayerBuff();
        ProcessEnemyDebuff();
    }

    private void ProcessPlayerBuff()
    {
        if (_buffRoutine != null)
        {
            CoroutineHandler.StopRoutine( _buffRoutine );
            _buffRoutine = null;
        }

        if (_buffRoutine == null)
            _buffRoutine = CoroutineHandler.StartRoutine(BuffRoutine());
    }

    IEnumerator BuffRoutine()
    {
        // 버프 중복 금지
        if (_isBuff == false)
        {
            _isBuff = true;

            Model.AttackSpeedMultiplier += _increaseAttackSpeed;
            Model.MoveSpeedMultyplier += _increaseMoveSpeed;
        }

        yield return _buffDuration.GetDelay();
        // 플레이어 버프 끝
        Model.AttackSpeedMultiplier -= _increaseAttackSpeed;
        Model.MoveSpeedMultyplier -= _increaseMoveSpeed;


        _isBuff = false;
        _buffRoutine = null;
    }

    private void ProcessEnemyDebuff()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _slowRange, Player.OverLapColliders, 1 << Layer.Monster);
        for (int i = 0; i < hitCount; i++)
        {
            // TODO: 슬로우 기능 추가
            Battle.TargetDebuff(Player.OverLapColliders[i], _slow);
        }

        CreateSlowFieldEffect();
    }

    private void CreateSlowFieldEffect()
    {
        _effect.Effect = ObjectPool.Get(_effect.EffectPrefab, transform, _effect.EffectDuration);

        Vector3 effectScale = _effect.Effect.transform.localScale;
        _effect.Effect.transform.localScale = new Vector3(_slowRange * 2, effectScale.y, _slowRange * 2);
    }
}
