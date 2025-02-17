using Assets.Project.Programmer.NSJ.RND.Script;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "AdditionalEffect/Hit/Poison")]
public class PoizonAdditional : HitAdditional
{

    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
        public float EffectDuration;
    }
    [Range(0,1)]public float DamageMultiplier;
    [SerializeField] EffectStrcut _effect;

    public override void Enter()
    {

        if (_debuffRoutine == null)
        {
            _effect.Effect = ObjectPool.Get(_effect.EffectPrefab, Battle.HitPoint);
            _debuffRoutine = CoroutineHandler.StartRoutine(PoisonRoutine());
        }
    }

    public override void Exit()
    {
        if (_debuffRoutine != null)
        {
            ObjectPool.Return(_effect.Effect);
            CoroutineHandler.StopRoutine(_debuffRoutine);
            _debuffRoutine = null;
        }
    }

    IEnumerator PoisonRoutine()
    {
        _remainDuration = Duration;
        int damage = (int)(Battle.Hit.MaxHp * 0.05f);
        while (_remainDuration > 0)
        {
            yield return 1f.GetDelay();
            Battle.TakeDamage(damage, true);
            _remainDuration--;         
        }
        Battle.EndDebuff(this);
    }
}
