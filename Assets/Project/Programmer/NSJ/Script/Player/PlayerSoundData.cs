using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSound" , menuName = "Sound/Type/Player")]
public class PlayerSoundData : ScriptableObject
{
    #region Hit
    [System.Serializable]
    public struct HiTStruct
    {
        [SerializeField] private AudioClip[] Hits;
        public AudioClip Hit { get { return Hits[Random.Range(0, Hits.Length)]; } }

        public AudioClip Critical;
    }
    [SerializeField] public HiTStruct Hit;
    #endregion

    #region 파워암
    [System.Serializable] 
    public struct PowerStruct
    {
        public AudioClip SpecialHit;
        public AudioClip Charge;
    }
    [SerializeField] public PowerStruct Power;
    #endregion

    #region 밸런스 암
    [System.Serializable]
    public struct BalanceStruct
    {
        public AudioClip Special2Hit;
        public AudioClip Special3Hit;
        public AudioClip Special3Loop;
    }
    [SerializeField] public BalanceStruct Balance;
    #endregion

    #region 움직임

    [System.Serializable] 
    public struct MoveStruct
    {
        public AudioClip Dash;
        public AudioClip DoubleJump;
        public AudioClip JumpAttack;
        public AudioClip Landing;
    }
    [SerializeField] public MoveStruct Move;

    #endregion
}
