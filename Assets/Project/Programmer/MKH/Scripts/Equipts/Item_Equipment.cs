using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MKH
{
    [Serializable] public struct EquipmentEffect
    {
        [Header("공격력")]
        [SerializeField] private float mDamage;
        public float Damage { get { return mDamage; } set { mDamage = value; } }

        [Header("방어력")]
        [SerializeField] private float mDefense;
        public float Defense { get { return mDefense; } set { mDefense = value; } }

        [Header("체력")]
        [SerializeField] private float mHP;
        public float HP { get { return mHP; } set { mHP = value; } }

        [Header("치명타 확률")]
        [SerializeField] private float mCritical;
        public float Critical { get { return mCritical; } set { mCritical = value; } }

        [Header("공격 속도")]
        [SerializeField] private float mAttackSpeed;
        public float AttackSpeed { get { return mAttackSpeed; } set { mAttackSpeed = value; } }

        [Header("스테미나")]
        [SerializeField] private float mStemina;
        public float Stemina { get { return mStemina; } set { mStemina = value; } }

        [Header("장비획득률")]
        [SerializeField] private float mEquipRate;
        public float EquipRate { get { return mEquipRate; } set { mEquipRate = value; } }

        [Header("이동 속도")]
        [SerializeField] private float mSpeed;
        public float Speed { get { return mSpeed; } set { mSpeed = value; } }

        [Header("마나")]
        [SerializeField] private float mMana;
        public float Mana { get { return mMana; } set { mMana = value; } }

        public static EquipmentEffect operator +(EquipmentEffect param1, EquipmentEffect param2)
        {
            EquipmentEffect calcedEffect;

            calcedEffect.mDamage = param1.mDamage + param2.mDamage;
            calcedEffect.mDefense = param1.mDefense + param2.mDefense;
            calcedEffect.mHP = param1.mHP + param2.mHP;
            calcedEffect.mCritical = param1.mCritical + param2.mCritical;
            calcedEffect.mAttackSpeed = param1.mAttackSpeed + param2.mAttackSpeed;
            calcedEffect.mStemina = param1.mStemina + param2.mStemina;
            calcedEffect.mEquipRate = param1.mEquipRate + param2.mEquipRate;
            calcedEffect.mSpeed = param1.mSpeed + param2.mSpeed;
            calcedEffect.mMana = param1.mMana + param2.mMana;

            return calcedEffect;
        }
    }

    public class Item_Equipment : Item
    {
        [Space(50)]
        [Header("장비 아이템 효과")]
        [SerializeField] public EquipmentEffect mEffect;
        public EquipmentEffect Effect { get { return mEffect; } set { mEffect = value; } }
      
    }
}
