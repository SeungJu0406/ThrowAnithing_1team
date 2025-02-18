using MKH;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

public class PlayerModel : MonoBehaviour
{
    public bool IsTest;
    [Inject]
    public PlayerData Data;
    [Inject]
    [HideInInspector] public GlobalPlayerStateData GlobalStateData;
    [Inject]
    [HideInInspector] public GlobalGameData GameData;


    public ArmUnit Arm;
    #region 체력
    public int MaxHp { get { return Data.MaxHp; } set { Data.MaxHp = value; } }
    public float MaxHpMultiplier { get { return Data.MaxHpMultiplier; } set { Data.MaxHpMultiplier = value; } }
    public int CurHp { get { return Data.CurHp; } set { Data.CurHp = value; CurHpSubject?.OnNext(Data.CurHp); } }
    public Subject<int> CurHpSubject = new Subject<int>();
    #endregion
    #region 방어력
    public int Defense { get { return Data.Defense; } set { Data.Defense = value; } }
    public float DamageReduction { get { return Data.DamageReduction; } set { Data.DamageReduction = value; } }
    #endregion
    #region 공격
    public int AttackPower { get { return Data.AttackPower; } set { Data.AttackPower = value; } }
    public float AttackPowerMultiplier { get { return Data.AttackPowerMultiplier; } set { Data.AttackPowerMultiplier = value; } }
    public float DamageMultiplier { get { return Data.DamageMultiplier; } set { Data.DamageMultiplier = value; } }
    public float AttackSpeed
    {
        get
        {
            return Data.AttackSpeed;
        }
        set { Data.AttackSpeed = value; }
    }
    public float AttackSpeedMultiplier { get { return Data.AttackSpeedMultiplier; } set { Data.AttackSpeedMultiplier = value; } }
    public float[] PowerMeleeAttack { get { return Data.PowerMeleeAttack; } set { Data.PowerMeleeAttack = value; } }
    public float[] PowerThrowAttack { get { return Data.PowerThrowAttack; } set { Data.PowerThrowAttack = value; } }
    public float[] PowerSpecialAttack { get { return Data.PowerSpecialAttack; } set { Data.PowerSpecialAttack = value; } }
    #endregion
    #region 치명타
    public float CriticalChance { get { return Data.CriticalChance; } set { Data.CriticalChance = value; } }
    public float CriticalDamage { get { return Data.CriticalDamage; } set { Data.CriticalDamage = value; } }
    #endregion
    #region 투척물
    public int MaxThrowables { get { return Data.MaxThrowables; } set { Data.MaxThrowables = value; } }
    public Subject<int> MaxThrowCountSubject { get { return Data.MaxThrowCountSubject; } set { Data.MaxThrowCountSubject = value; } }
    public int CurThrowables
    {
        get { return Data.CurThrowables; }
        set
        {
            Data.CurThrowables = value;
        }
    }
    public Subject<int> CurThrowCountSubject { get { return Data.CurThrowCountSubject; } set { Data.CurThrowCountSubject = value; } }
    public List<ThrowObjectData> ThrowObjectStack { get { return Data.ThrowObjectStack; } set { Data.ThrowObjectStack = value; } }
    #endregion
    #region 특수효과
    public List<AdditionalEffect> AdditionalEffects { get { return Data.AdditionalEffects; } set { Data.AdditionalEffects = value; } } // 블루칩 모음 리스트
    public List<HitAdditional> HitAdditionals { get { return Data.HitAdditionals; } set { Data.HitAdditionals = value; } }
    public List<ThrowAdditional> ThrowAdditionals { get { return Data.ThrowAdditionals; } set { Data.ThrowAdditionals = value; } } // 공격 방법 추가효과 리스트
    public List<PlayerAdditional> PlayerAdditionals { get { return Data.PlayerAdditionals; } set { Data.PlayerAdditionals = value; } } // 플레이어 추가효과 리스트
    #endregion
    #region 이동
    public float MoveSpeed { get { return (Data.MoveSpeed) / 20; } set { Data.MoveSpeed = value; } } // 이동속도
    public float MoveSpeedMultyplier { get { return Data.MoveSpeedMultyplier; } set { Data.MoveSpeedMultyplier = value; } }
    // 대쉬
    public float DashDistance { get { return Data.DashDistance / 100f; } set { Data.DashDistance = value; } }
    public int DashStamina { get { return Data.DashStamina; } set { Data.DashStamina = value; } }
    // 점프
    public float JumpPower { get { return Data.JumpPower / 13f; } set { Data.JumpPower = value * 13f; } }
    public int JumpStamina { get { return Data.JumpStamina; } set { Data.JumpStamina = value; } }
    public int DoubleJumpStamina { get { return Data.DoubleJumpStamina; } set { Data.DoubleJumpStamina = value; } }
    public int JumpDownStamina { get { return Data.JumpDownStamina; } set { Data.JumpDownStamina = value; } }
    #endregion
    #region 스테미나
    public float MaxStamina { get { return Data.MaxStamina; } set { Data.MaxStamina = value; } } // 최대 스테미나
    public float CurStamina
    {
        get { return Data.CurStamina; }
        set
        {
            float originStamina = Data.CurStamina;
            Data.CurStamina = value;
            if (originStamina > Data.CurStamina)
            {
                _player.IsStaminaCool = true;
            }
            CurStaminaSubject.OnNext(Data.CurStamina);
        }
    } // 현재 스테미나
    public Subject<float> CurStaminaSubject = new Subject<float>();
    public float RegainStamina { get { return Data.RegainStamina; } set { Data.RegainStamina = value; } } // 스테미나 초당 회복량
    public float StaminaCoolTime { get { return Data.StaminaCoolTime; } set { Data.StaminaCoolTime = value; } } // 스테미나 소진 후 쿨타임
    public float StaminaReduction { get { return Data.StaminaReduction; } set { Data.StaminaReduction = value; } }
    public float MaxStaminaCharge { get { return Data.MaxStaminaCharge; } set { Data.MaxStaminaCharge = value; } }
    public float CurStaminaCharge { get { return Data.CurStaminaCharge; } set { Data.CurStaminaCharge = value; CurStaminaChargeSubject.OnNext(value); } }
    public Subject<float> CurStaminaChargeSubject = new Subject<float>();
    public float[] MeleeAttackStamina { get { return Data.MeleeAttackStamina; } set { Data.MeleeAttackStamina = value; } }
    #endregion
    #region 마나
    public float MaxMana { get { return Data.MaxMana; } set { Data.MaxMana = value; } } // 최대 특수자원
    public Subject<float> MaxManaSubject { get { return Data.MaxManaSubject; } }
    public float CurMana // 현재 특수 자원
    {
        get { return Data.CurMana; }
        set
        {
            Data.CurMana = value;
            // 현재 특수공격 자원이 최대치를 넘길 수 없음
            if (Data.CurMana > Data.MaxMana)
            {
                Data.CurMana = Data.MaxMana;
            }
            else if (Data.CurMana < 0)
            {
                Data.CurMana = 0;
            }
            CurManaSubject.OnNext(Data.CurMana);
        }
    }
    public Subject<float> CurManaSubject = new Subject<float>();
    public float[] ManaConsumption { get { return Data.ManaConsumption; } set { Data.ManaConsumption = value; } }
    public float[] RegainMana { get { return Data.RegainMana; } set { Data.RegainMana = value; } } // 특수자원 회복량
    public float RegainAdditiveMana { get { return Data.RegainAdditiveMana; } set { Data.RegainAdditiveMana = value; } }
    public float SpecialChargeGage // 특수공격 차지량
    {
        get { return Data.SpecialChargeGage; }
        set
        {
            Data.SpecialChargeGage = value;
            SpecialChargeGageSubject.OnNext(Data.SpecialChargeGage);
        }
    }
    public Subject<float> SpecialChargeGageSubject = new Subject<float>();
    #endregion
    public float DrainLife { get { return Data.DrainLife; } set { Data.DrainLife = value; } }
    public GlobalGameData.AmWeapon NowWeapon { get { return Data.NowWeapon; } set { Data.NowWeapon = value; } }
    public float EquipmentDropUpgrade { get { return Data.EquipmentDropUpgrade; } set { Data.EquipmentDropUpgrade = value; } }

    public float BoomRadius;

    [System.Serializable]
    public struct DefaultDrainStruct
    {
        public float DrainDistance;
        public float DrainStamina;
    }
    // TODO : 인스펙터 정리 필요
    [System.Serializable]
    public struct DrainStruct
    {
        public DefaultDrainStruct Default;
        public float DrainDistance;
        public float DrainDistanceMultyPlier;
        public float DrainStamina;
    }
    [SerializeField] public DrainStruct Drain;
    public float DrainDistance
    {
        get
        {
            return Drain.DrainDistance * (1 + DrainDistanceMultyPlier / 100); // 드레인 기본 범위 * 드레인 범위 증가량(%)
        }
        set { Drain.DrainDistance = value; }
    }
    public float DrainDistanceMultyPlier { get { return Drain.DrainDistanceMultyPlier; } set { Drain.DrainDistanceMultyPlier = value; } }
    public float DrainStamina { get { return Drain.DrainStamina * (1 - StaminaReduction / 100); } set { Drain.DrainStamina = value; } }
    [Tooltip("현재 차지 단계")]
    public int ChargeStep;
    [Tooltip("넉백 거리 증가")]
    public float KnockBackDistanceMultiplier;


    private PlayerView _view;
    private PlayerController _player;
    public void PushThrowObject(ThrowObjectData throwObjectData)
    {
        Data.PushThrowObject(throwObjectData);
    }

    public ThrowObjectData PopThrowObject()
    {
        return Data.PopThrowObject();
    }
    public ThrowObjectData PeekThrowObject()
    {
        return Data.PeekThrowObject();
    }
    public void ClearThrowObject()
    {
        Data.ClearThrowObject();
    }

    private void Awake()
    {
        _view = GetComponent<PlayerView>();
        _player = GetComponent<PlayerController>();
        Data.IsDead = false;

        if (SceneManager.GetActiveScene().name == SceneName.LobbyScene)
        {
            Data.CopyGlobalPlayerData(GlobalStateData, GameData);
        }
        if (IsTest == true)
        {
            GlobalStateData.NewPlayerSetting();
            Data.CopyGlobalPlayerData(GlobalStateData, GameData);
            RegainStamina = 100;
            RegainMana[0] = 100;
        }

        DrainDistance = Drain.Default.DrainDistance;
        DrainStamina = Drain.Default.DrainStamina;
        JumpDownStamina = 40;
        CriticalDamage = 200;
        StaminaCoolTime = 1;

    }


    private float prevAttackSpeed;
    private GlobalGameData.AmWeapon prevWeapon;
    private float prevMaxMana;
    void Start()
    {
        this.FixedUpdateAsObservable()
            .Where(x => prevAttackSpeed != AttackSpeed)
            .Subscribe(x =>
            {
                _view.SetFloat(PlayerView.Parameter.AttackSpeed, AttackSpeed);
                prevAttackSpeed = AttackSpeed;
            }
            );
        this.FixedUpdateAsObservable()
            .Where(x => prevWeapon != NowWeapon)
            .Subscribe(x => { _player.ChangeArmUnit(NowWeapon); prevWeapon = NowWeapon; });

        this.FixedUpdateAsObservable()
            .Where(x => prevMaxMana != MaxMana)
        .Subscribe(x =>
            {
                _view.Panel.MpBar.maxValue = MaxMana;
                _view.Panel.SetChargingMpVarMaxValue(MaxMana);
                prevMaxMana = MaxMana;
            });



        //if (IsTest == true)
        //{
        //    SoundManager.SetVolumeMaster(1);
        //    SoundManager.SetVolumeSFX(1);
        //}
    }
    #region 콤보 관련 폐기된 코드
    //[System.Serializable]
    //public struct MeleeStruct
    //{
    //    [HideInInspector] public int ComboCount;
    //    public MeleeAttackStruct[] PowerMeleeAttack;
    //}
    //[System.Serializable]
    //public struct MeleeAttackStruct
    //{
    //    public float Range;
    //    [Range(0, 360)] public float Angle;
    //    [Range(0, 5)] public float DamageMultiplier;
    //}
    //[Header("근접공격 관련 필드")]
    //[SerializeField] public MeleeStruct PrevMelee;
    //public int MeleeComboCount
    //{
    //    get { return PrevMelee.ComboCount; }
    //    set
    //    {
    //        PrevMelee.ComboCount = value;
    //        if (PrevMelee.ComboCount >= PrevMelee.PowerMeleeAttack.Length)
    //        {
    //            PrevMelee.ComboCount = 0;
    //        }
    //    }
    //}
    //public float Range => PrevMelee.PowerMeleeAttack[PrevMelee.ComboCount].Range;
    //public float Angle => PrevMelee.PowerMeleeAttack[PrevMelee.ComboCount].Angle;
    //public float DamageMultiplier => PrevMelee.PowerMeleeAttack[PrevMelee.ComboCount].DamageMultiplier;
    #endregion
}


public partial class GlobalPlayerData
{

}


public partial class PlayerData
{
    [System.Serializable]
    public struct HpStruct
    {
        [Header("최대 체력")]
        public int MaxHp;
        [Header("최대 체력 배율(%)")]
        public float MaxHpMultiplier;
        [Header("현재 체력")]
        public int CurHp;
    }

    [System.Serializable]
    public struct AttackStruct
    {
        [Header("공격력")]
        public int AttackPower;
        [Header("공격력 배율")]
        public float AttackPowerMultiplier;
        [Header("데미지 배율(%)")]
        public float DamageMultiplier;
        [Header("공격 속도")]
        public float AttackSpeed;
        [Header("공격 속도 배율(%)")]
        public float AttackSpeedMultiplier;
        [Header("암슈트-파워 근접공격력")]
        public float[] PowerMeleeAttack;
        [Header("암슈트-파워 투척공격력")]
        public float[] PowerThrowAttack;
        [Header("암슈트-파워 특수공격력")]
        public float[] PowerSpecialAttack;
    }
    [System.Serializable]
    public struct StaminaStruct
    {
        [Header("최대 스테미나")]
        public float MaxStamina; // 최대 스테미나
        [Header("현재 스테미나")]
        public float CurStamina; // 현재 스테미나
        [Header("스테미나 초당 회복량")]
        public float RegainStamina; // 스테미나 초당 회복량
        [Header("스테미나 사용 후 쿨타임")]
        public float StaminaCoolTime; // 스테미나 소진 후 쿨타임
        [Header("스테미나 소모량 (?)")]
        public float ConsumesStamina; // 스테미나 소모량
        [Header("스테미나 소모 감소량(%)")]
        public float StaminaReduction;
        [Header("스테미나 최대 차지 시간")]
        public float MaxStaminaCharge;
        [Header("현재 스테미나 차지 시간")]
        public float CurStaminaCharge;
        [Header("암유닛-파워 근접공격 스테미나 소모량")]
        public float[] MeleeAttackStamina;
    }
    [System.Serializable]
    public struct JumpStruct
    {
        [Header("점프력")]
        public float JumpPower;
        [Header("점프 스테미나")]
        public int JumpStamina;
        [Header("더블 점프 스테미나")]
        public int DoubleJumpStamina;
        [Header("하강 공격 스테미나")]
        public int JumpDownStamina;
        [Header("최대 점프 횟수")]
        public int MaxJumpCount;
        [Header("현재 점프 횟수")]
        public int CurJumpCount;
    }
    [System.Serializable]
    public struct DashStruct
    {
        [Header("대쉬 속도")]
        public float DashDistance;
        [Header("대쉬 스테미나")]
        public int DashStamina;
    }
    [System.Serializable]
    public struct SpecialStruct
    {
        [Header("최대 마나")]
        public float MaxMana;
        [Header("현재 마나")]
        public float CurMana;
        [Header("투척 공격 당 마나 회복")]
        public float[] RegainMana; // 던지기 공격당 마나 회복
        [Header("마나 회복량 증가량(%)")]
        public float RegainAdditiveMana;
        [Header("마나 소모량")]
        public float[] ManaConsumption; // 마나 소모량
        [HideInInspector] public float SpecialChargeGage;
    }
    [System.Serializable]
    public struct DefenseStruct
    {
        [Header("방어력")]
        public int Defense;
        [Header("피해 감소량")]
        public float DamageReduction;
    }
    [System.Serializable]
    public struct CriticalStruct
    {
        [Header("크리티컬 확률")]
        [Range(0, 100)] public float CriticalChance;
        [Header("크리티컬 데미지")]
        public float CriticalDamage;
    }
    [System.Serializable]
    public struct ThrowStruct
    {
        [Header("최대 투척물 수")]
        public int MaxThrowables;
        [Header("현재 투척물 수")]
        public int CurThrowables;
        [Header("투척물 파밍시 추가 획득 수")]
        public float GainMoreThrowables;
        [Header("투척물 리스트")]
        public List<ThrowObjectData> ThrowObjectStack;
    }
    [System.Serializable]
    public struct AdditionalStruct
    {
        [Header("특수효과 리스트")]
        public List<AdditionalEffect> AdditionalEffects; // 특수효과 리스트
        [Header("적중 시 효과 리스트")]
        public List<HitAdditional> HitAdditionals;
        [Header("투척물 효과 리스트")]
        public List<ThrowAdditional> ThrowAdditionals; // 공격 방법 추가효과 리스트
        [Header("플레이어 자체 효과 리스트")]
        public List<PlayerAdditional> PlayerAdditionals; // 플레이어 추가효과 리스트
    }
    [System.Serializable]
    public struct MoveStruct
    {
        [Header("이동속도")]
        public float MoveSpeed;
        [Header("이동속도 배율")]
        public float MoveSpeedMultyplier;
    }
    [System.Serializable]
    public struct DataStruct
    {
        public HpStruct Hp;
        public AttackStruct Attack;
        public DefenseStruct Defense;
        public CriticalStruct Critical;
        public StaminaStruct Stamina;
        public SpecialStruct Special;
        public MoveStruct Move;
        public DashStruct Dash;
        public JumpStruct Jump;
        public GlobalGameData.AmWeapon NowWeapon;
        public ThrowStruct Throw;
        public AdditionalStruct Additional;
        public float DrainLife;
        public float EquipmentDropUpgrade;
        // 상태이상 지속시간
    }
    [SerializeField] public DataStruct Data;
    #region 체력
    // 체력
    public int MaxHp
    {
        get
        {
            float maxHpMultiplier = 1 + MaxHpMultiplier / 100 >= 0 ? 1 + MaxHpMultiplier / 100 : 0;
            return (int)((Data.Hp.MaxHp + (int)EquipStatus.HP) * maxHpMultiplier); // (기본 체력+장비체력) * 체력 배율
        }
        set
        {
            Data.Hp.MaxHp = value;
            OnChangePlayerDataEvent?.Invoke();
        }
    }
    public float MaxHpMultiplier
    {
        get { return Data.Hp.MaxHpMultiplier; }
        set
        {
            int prevMaxHp = MaxHp;
            float prevMaxHpMultiplier = MaxHpMultiplier;

            Data.Hp.MaxHpMultiplier = value;
            // 체력 배율이 낮아진 경우 현재체력 맞춰줌
            if (Data.Hp.MaxHpMultiplier < prevMaxHpMultiplier)
            {
                if (CurHp > MaxHp)
                    CurHp = MaxHp;
            }
            // 체력 배율이 높아진 경우 현재체력을 증가한 체력만큼 올려줌
            else if (Data.Hp.MaxHpMultiplier > prevMaxHpMultiplier)
            {
                int HpComparison = MaxHp - prevMaxHp;
                CurHp += HpComparison;
            }

            OnChangePlayerDataEvent?.Invoke();
        }
    }
    public int CurHp
    {
        get
        {
            return Data.Hp.CurHp;
        }
        set
        {
            Data.Hp.CurHp = value;
            // 현재체력이 최대체력을 넘지 못하도록
            if (Data.Hp.CurHp > MaxHp)
                Data.Hp.CurHp = MaxHp;
        }
    }
    #endregion
    #region 방어력
    // 방어력
    public int Defense { get { return Data.Defense.Defense + (int)EquipStatus.Defense; } set { Data.Defense.Defense = value; OnChangePlayerDataEvent?.Invoke(); } }
    public float DamageReduction { get { return Data.Defense.DamageReduction; } set { Data.Defense.DamageReduction = value; } }
    #endregion
    #region 공격 
    public int AttackPower
    {
        get
        {
            float attackMultiplier = 1 + AttackPowerMultiplier / 100 >= 0 ? 1 + AttackPowerMultiplier / 100 : 0; // 데미지 배율이 0까지 떨어진 경우 0으로 고정
            return (int)((Data.Attack.AttackPower + (int)EquipStatus.Damage) * attackMultiplier); // (기본데미지+장비데미지) * 공격력 배율
        }
        set
        {
            Data.Attack.AttackPower = value;
            OnChangePlayerDataEvent?.Invoke();
        }
    }
    public float AttackPowerMultiplier { get { return Data.Attack.AttackPowerMultiplier; } set { Data.Attack.AttackPowerMultiplier = value; OnChangePlayerDataEvent?.Invoke(); } }
    public float DamageMultiplier { get { return Data.Attack.DamageMultiplier; } set { Data.Attack.DamageMultiplier = value; } }
    public float AttackSpeed
    {
        get
        {
            float attackSpeedMultiplier = 1 + AttackSpeedMultiplier / 100 >= 0 ? 1 + AttackSpeedMultiplier / 100 : 0; // 데미지 배율이 0까지 떨어진 경우 0으로 고정
            return (Data.Attack.AttackSpeed * (1 + EquipStatus.AttackSpeed)) * attackSpeedMultiplier; // 기본공격속도 * 장비공격속도 * 공격속도 배율
        }
        set
        {
            Data.Attack.AttackSpeed = value;
            OnChangePlayerDataEvent?.Invoke();
        }
    }
    public float AttackSpeedMultiplier { get { return Data.Attack.AttackSpeedMultiplier; } set { Data.Attack.AttackSpeedMultiplier = value; OnChangePlayerDataEvent?.Invoke(); } }
    public float[] PowerMeleeAttack { get { return Data.Attack.PowerMeleeAttack; } set { Data.Attack.PowerMeleeAttack = value; } }
    public float[] PowerThrowAttack { get { return Data.Attack.PowerThrowAttack; } set { Data.Attack.PowerThrowAttack = value; } }
    public float[] PowerSpecialAttack { get { return Data.Attack.PowerSpecialAttack; } set { Data.Attack.PowerSpecialAttack = value; } }
    #endregion
    #region 스테미나

    // 스테미나
    public float MaxStamina { get { return Data.Stamina.MaxStamina + EquipStatus.Stemina; } set { Data.Stamina.MaxStamina = value; OnChangePlayerDataEvent?.Invoke(); } }
    public float CurStamina { get { return Data.Stamina.CurStamina; } set { Data.Stamina.CurStamina = value; } }
    public float RegainStamina { get { return Data.Stamina.RegainStamina; } set { Data.Stamina.RegainStamina = value; } }
    public float StaminaCoolTime { get { return Data.Stamina.StaminaCoolTime; } set { Data.Stamina.StaminaCoolTime = value; } }
    public float ConsumesStamina { get { return Data.Stamina.ConsumesStamina; } set { Data.Stamina.ConsumesStamina = value; } }
    public float StaminaReduction { get { return Data.Stamina.StaminaReduction; } set { Data.Stamina.StaminaReduction = value; } }
    public float MaxStaminaCharge { get { return Data.Stamina.MaxStaminaCharge; } set { Data.Stamina.MaxStaminaCharge = value; } }
    public float CurStaminaCharge { get { return Data.Stamina.CurStaminaCharge; } set { Data.Stamina.CurStaminaCharge = value; } }
    public float[] MeleeAttackStamina { get { return Data.Stamina.MeleeAttackStamina; } set { Data.Stamina.MeleeAttackStamina = value; } }
    #endregion
    #region 마나
    // 특수공격
    public float MaxMana
    {
        get { return Data.Special.MaxMana + EquipStatus.Mana; }
        set
        {
            Data.Special.MaxMana = value;
            MaxManaSubject.OnNext(value);
            OnChangePlayerDataEvent?.Invoke();
        }
    }
    public Subject<float> MaxManaSubject = new Subject<float>();
    public float CurMana { get { return Data.Special.CurMana; } set { Data.Special.CurMana = value; } }
    public float[] RegainMana { get { return Data.Special.RegainMana; } set { Data.Special.RegainMana = value; } }
    public float RegainAdditiveMana { get { return Data.Special.RegainAdditiveMana; } set { Data.Special.RegainAdditiveMana = value; } }
    public float[] ManaConsumption { get { return Data.Special.ManaConsumption; } set { Data.Special.ManaConsumption = value; } }
    public float SpecialChargeGage { get { return Data.Special.SpecialChargeGage; } set { Data.Special.SpecialChargeGage = value; } }
    #endregion
    #region 이동
    public float MoveSpeed
    {
        get
        {
            float moveSpeedMultyplier = 1 + MoveSpeedMultyplier / 100 < 0 ? 0 : 1 + MoveSpeedMultyplier / 100;
            return Data.Move.MoveSpeed * (1 + EquipStatus.Speed) * moveSpeedMultyplier; // 기본 이동속도 * 장비 이동속도 * 이동속도 배율
        }
        set
        {
            Data.Move.MoveSpeed = value;
            OnChangePlayerDataEvent?.Invoke();
        }
    }
    public float MoveSpeedMultyplier { get { return Data.Move.MoveSpeedMultyplier; } set { Data.Move.MoveSpeedMultyplier = value; } }
    // 점프
    public float JumpPower { get { return Data.Jump.JumpPower; } set { Data.Jump.JumpPower = value; OnChangePlayerDataEvent?.Invoke(); } }
    public int JumpStamina
    {
        get { return (int)(Data.Jump.JumpStamina * (1 - StaminaReduction / 100)); }
        set { Data.Jump.JumpStamina = value; }
    }
    public int DoubleJumpStamina
    {
        get
        { return (int)(Data.Jump.DoubleJumpStamina * (1 - StaminaReduction / 100)); }
        set
        { Data.Jump.DoubleJumpStamina = value; }
    }
    public int JumpDownStamina
    {
        get { return (int)(Data.Jump.JumpDownStamina * (1 - StaminaReduction / 100)); }
        set { Data.Jump.JumpDownStamina = value; }
    }
    public int MaxJumpCount { get { return Data.Jump.MaxJumpCount; } set { Data.Jump.MaxJumpCount = value; } }
    public int CurJumpCount { get { return Data.Jump.CurJumpCount; } set { Data.Jump.CurJumpCount = value; } }
    // 대쉬
    public float DashDistance { get { return Data.Dash.DashDistance; } set { Data.Dash.DashDistance = value; } }
    public int DashStamina
    {
        get { return (int)(Data.Dash.DashStamina * (1 - StaminaReduction / 100)); }

        set { Data.Dash.DashStamina = value; }
    }
    #endregion
    #region 치명타
    // 크리티컬
    public float CriticalChance { get { return Data.Critical.CriticalChance + EquipStatus.Critical; } set { Data.Critical.CriticalChance = value; OnChangePlayerDataEvent?.Invoke(); } }
    public float CriticalDamage { get { return Data.Critical.CriticalDamage; } set { Data.Critical.CriticalDamage = value; } }
    #endregion
    #region 투척물
    // 투척오브젝트
    public int MaxThrowables { get { return Data.Throw.MaxThrowables; } set { Data.Throw.MaxThrowables = value; MaxThrowCountSubject.OnNext(Data.Throw.MaxThrowables); } }
    public Subject<int> MaxThrowCountSubject = new Subject<int>();
    public int CurThrowables { get { return Data.Throw.CurThrowables; } set { Data.Throw.CurThrowables = value; CurThrowCountSubject.OnNext(Data.Throw.CurThrowables); } }
    public Subject<int> CurThrowCountSubject = new Subject<int>();
    public float GainMoreThrowables { get { return Data.Throw.GainMoreThrowables; } set { Data.Throw.GainMoreThrowables = value; } }
    public List<ThrowObjectData> ThrowObjectStack { get { return Data.Throw.ThrowObjectStack; } set { Data.Throw.ThrowObjectStack = value; } }
    #endregion
    #region 추가효과
    //추가효과
    public List<AdditionalEffect> AdditionalEffects { get { return Data.Additional.AdditionalEffects; } set { Data.Additional.AdditionalEffects = value; } }
    public List<HitAdditional> HitAdditionals { get { return Data.Additional.HitAdditionals; } set { Data.Additional.HitAdditionals = value; } }
    public List<ThrowAdditional> ThrowAdditionals { get { return Data.Additional.ThrowAdditionals; } set { Data.Additional.ThrowAdditionals = value; } } // 공격 방법 추가효과 리스트
    public List<PlayerAdditional> PlayerAdditionals { get { return Data.Additional.PlayerAdditionals; } set { Data.Additional.PlayerAdditionals = value; } } // 플레이어 추가효과 리스트
    #endregion
    public float DrainLife { get { return Data.DrainLife; } set { Data.DrainLife = value; } }
    // 암유닛
    public GlobalGameData.AmWeapon NowWeapon { get { return Data.NowWeapon; } set { Data.NowWeapon = value; } }
    public float EquipmentDropUpgrade { get { return Data.EquipmentDropUpgrade + (100 * EquipStatus.EquipRate); } set { Data.EquipmentDropUpgrade = value; } }

    [HideInInspector] public bool IsDead;
    [System.Serializable]
    public struct InventoryStruct
    {
        public EquipmentInventory Inventory;
        public InventoryMain InventoryMain;
        public GameObject BlueChipChoice;
        public BlueChipPanel BlueChipPanel;
        public BlueChipChoicePanel BlueChipChoicePanel;
        public BlueChipChoiceController BlueChipChoiceController;
        public GameObject ChoicePanel;
        public GameObject PcText;
        public GameObject ConsloeText;
    }
    public InventoryStruct Inventory;

    public EquipmentEffect EquipStatus => Inventory.Inventory.CurrentEquipmentEffect;

    public event UnityAction OnChangePlayerDataEvent;
    public void CopyGlobalPlayerData(GlobalPlayerStateData globalData, GlobalGameData gameData)
    {
        MaxHp = (int)globalData.maxHp;
        CurHp = (int)globalData.maxHp;

        AttackPower = (int)globalData.commonAttack;
        PowerMeleeAttack = new float[globalData.shortRangeAttack.Length];
        PowerMeleeAttack[0] = (int)globalData.shortRangeAttack[0];
        PowerMeleeAttack[1] = (int)globalData.shortRangeAttack[1];
        PowerMeleeAttack[2] = (int)globalData.shortRangeAttack[2];

        PowerThrowAttack = new float[globalData.longRangeAttack.Length];
        PowerThrowAttack[0] = (int)globalData.longRangeAttack[0];
        PowerThrowAttack[1] = (int)globalData.longRangeAttack[1];
        PowerThrowAttack[2] = (int)globalData.longRangeAttack[2];
        PowerThrowAttack[3] = (int)globalData.longRangeAttack[3];

        PowerSpecialAttack = new float[globalData.specialAttack.Length];
        PowerSpecialAttack[0] = (int)globalData.specialAttack[0];
        PowerSpecialAttack[1] = (int)globalData.specialAttack[1];
        PowerSpecialAttack[2] = (int)globalData.specialAttack[2];
        AttackSpeed = globalData.attackSpeed;

        MoveSpeed = globalData.movementSpeed;
        CriticalChance = globalData.criticalChance;
        Defense = (int)globalData.defense;
        EquipmentDropUpgrade = globalData.equipmentDropUpgrade;
        DrainLife = globalData.drainLife;
        MaxStamina = globalData.maxStamina;
        RegainStamina = globalData.regainStamina;
        ConsumesStamina = globalData.consumesStamina;

        RegainMana = new float[globalData.regainMana.Length];
        RegainMana[0] = globalData.regainMana[0];
        RegainMana[1] = globalData.regainMana[1];
        RegainMana[2] = globalData.regainMana[2];
        RegainMana[3] = globalData.regainMana[3];

        ManaConsumption = new float[globalData.manaConsumption.Length];
        ManaConsumption[0] = globalData.manaConsumption[0];
        ManaConsumption[1] = globalData.manaConsumption[1];
        ManaConsumption[2] = globalData.manaConsumption[2];
        GainMoreThrowables = globalData.gainMoreThrowables;
        MaxThrowables = (int)globalData.maxThrowables;
        NowWeapon = gameData.nowWeapon;
        MaxMana = globalData.maxMana;
        MaxJumpCount = (int)globalData.maxJumpCount;
        JumpPower = globalData.jumpPower;
        JumpStamina = (int)globalData.jumpConsumesStamina;
        DoubleJumpStamina = (int)globalData.doubleJumpConsumesStamina;
        DashDistance = globalData.dashDistance;
        DashStamina = (int)globalData.dashConsumesStamina;

        MeleeAttackStamina = new float[globalData.shortRangeAttackStamina.Length];
        MeleeAttackStamina[0] = globalData.shortRangeAttackStamina[0];
        MeleeAttackStamina[1] = globalData.shortRangeAttackStamina[1];
        MeleeAttackStamina[2] = globalData.shortRangeAttackStamina[2];

        OnChangePlayerDataEvent?.Invoke();
    }

    public void InitialzePlayerData()
    {
        CurMana = 0;
    }

    public void ClearAdditional()
    {
        AdditionalEffects.Clear();
        HitAdditionals.Clear();
        ThrowAdditionals.Clear();
        PlayerAdditionals.Clear();
    }

    public void PushThrowObject(ThrowObjectData throwObjectData)
    {
        ThrowObjectStack.Add(throwObjectData);
        CurThrowables++;
    }

    public ThrowObjectData PopThrowObject()
    {
        CurThrowables--;
        ThrowObjectData data = ThrowObjectStack[CurThrowables];
        ThrowObjectStack.RemoveAt(CurThrowables);
        return data;
    }
    public ThrowObjectData PeekThrowObject()
    {
        ThrowObjectData data = ThrowObjectStack[CurThrowables - 1];
        return data;
    }
    public void ClearThrowObject()
    {
        ThrowObjectStack.Clear();
        CurThrowables = 0;
    }
}
