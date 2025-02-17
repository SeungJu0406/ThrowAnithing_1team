using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "PainTransfer", menuName = "AdditionalEffect/Hit/PainTransfer")]
public class PainTransferAdditional : HitAdditional
{
    [System.Serializable]
    struct EffectStrcut
    {
        public GameObject EffectPrefab;
        [HideInInspector] public GameObject Effect;
    }
    [Header("상태이상 전이 범위")]
    [SerializeField] private float _range;

    [SerializeField] EffectStrcut _effect;

    private bool _isFirst;

    public override void Enter()
    {
        Battle.OnDieEvent += TransmitDebuff;
    }
    public override void Exit()
    {
        Battle.OnDieEvent -= TransmitDebuff;
    }
    private void TransmitDebuff()
    {
        if (_isFirst == true)
            return;    

        Collider[] hits = Physics.OverlapSphere(Battle.transform.position, _range, 1 << Layer.Monster);

        foreach (Collider hit in hits) 
        {
            if (gameObject == hit.gameObject)
                continue;
            Debug.Log($"{hit.name} 에게 디버프 전이");
            Battle.TargetDebuff(hit, Battle.DebuffList);
        }
        _isFirst = true;
        CreateEffect();
        Battle.EndDebuff(this);
    }

    private void CreateEffect()
    {
       GameObject effect = _effect.Effect = ObjectPool.Get(_effect.EffectPrefab, transform.position, Quaternion.identity,2f);
        effect.transform.localScale = new Vector3(_range * 0.5f, _range * 0.5f, _range * 0.5f);
    }
}
