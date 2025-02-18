using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AdditionalEffect/Hit/Freeze")]
public class FreezeAdditional : HitAdditional
{
    [Header("È®·ü")]
    [SerializeField] public float Probability;



    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
    }
    [SerializeField] EffectStrcut _effect;

    private float _decreasedMoveSpeed;
    public override void Enter()
    {
        if (Random.Range(0, 100) > Probability)
            return;


        if (_debuffRoutine == null)
        {
            CreateEffect();
            ChangeValue(true);

            _debuffRoutine = CoroutineHandler.StartRoutine(FreezeRoutine());
        }
    }
    public override void Exit()
    {
        if (_debuffRoutine != null)
        {
            CoroutineHandler.StopRoutine(_debuffRoutine);
            _debuffRoutine = null;
            ChangeValue(false);
            ObjectPool.Return(_effect.Effect);
        }   
    }

    IEnumerator FreezeRoutine()
    {
        _remainDuration = Duration;
        while (_remainDuration > 0)
        {
            _remainDuration -= Time.deltaTime;
            if(_targetType == TargetType.Enemy)
            {
                ChangeEnemyZeroValue();
            }

            yield return null;
        }

        Battle.EndDebuff(this);
    }
    private void ChangeValue(bool isDecrease)
    {
        if (_targetType == TargetType.Player)
            ChangePlayerValue(isDecrease);
        else if (_targetType == TargetType.Enemy)
            ChangeEnemyValue(isDecrease);
    }

    private void ChangePlayerValue(bool isDecrease)
    {
        if (transform.tag != Tag.Player)
            return;

        if (isDecrease == true)
        {
            Player.Model.MoveSpeedMultyplier -= 10000;
        }
        else
        {
            Player.Model.MoveSpeedMultyplier += 10000;
        }
    }
    private void ChangeEnemyValue(bool isDecrease)
    {
        if (isDecrease == true)
        {
            float enemyMoveSpeed = GetEnemyMoveSpeed();
            _decreasedMoveSpeed = enemyMoveSpeed;
            SetEnemyMoveSpeed(0);
        }
        else
        {
            float enemyMoveSpeed = GetEnemyMoveSpeed();
           
            enemyMoveSpeed += _decreasedMoveSpeed;
            SetEnemyMoveSpeed(enemyMoveSpeed);
        }
    }

    private void ChangeEnemyZeroValue()
    {
        float enemyMoveSpeed = GetEnemyMoveSpeed();
        _decreasedMoveSpeed += enemyMoveSpeed;
        SetEnemyMoveSpeed(0);
    }

    private void CreateEffect()
    {
        _effect.Effect = ObjectPool.Get(_effect.EffectPrefab, transform);
        _effect.Effect.transform.position = Battle.HitPoint.position;
    }

}
