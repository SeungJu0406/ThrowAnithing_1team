using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum CrowdControlType { None, Stiff, Stun, Size}
public class BattleSystem : MonoBehaviour, IBattle
{
    public IHit Hit { get; set; }

    public Transform HitPoint;
    public List<HitAdditional> HitAdditionalList;
    public List<HitAdditional> DebuffList;
    public event UnityAction<int, bool> OnTargetAttackEvent;
    public event UnityAction<int, bool> OnTakeDamageEvent;
    public event UnityAction OnDieEvent;


    [HideInInspector]public bool IsDie;
    private void Awake()
    {
        Hit = GetComponent<IHit>();

        if (HitPoint == null)
        {
            HitPoint = new GameObject("HitTextPoint").transform;
            HitPoint.SetParent(transform, true);
            HitPoint.localPosition = new Vector3(0, 1f, 0);
        }
    }

    private void OnDisable()
    {
        ClearDebuff();
    }
    #region 공격 메서드
    /// <summary>
    /// 가진 모든 디버프만 주는 공격
    /// </summary>
    public void TargetDebuff<T>(T target) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return ;
        battle.TakeDebuff(HitAdditionalList);
    }
    /// <summary>
    /// 특정 디버프만 주는 공격
    /// </summary>
    public void TargetDebuff<T>(T target, HitAdditional debuff) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return ;
        battle.TakeDebuff(debuff);
    }
    /// <summary>
    /// 특정 디버프들만 주는 공격
    /// </summary>
    public void TargetDebuff<T>(T target, List<HitAdditional> debuffs) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return;
        battle.TakeDebuff(debuffs);
    }
    /// <summary>
    /// 적에게 CC기 
    /// </summary>
    public void TargetCrowdControl<T>(T target, CrowdControlType type) where T : Component
    {
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return;
        battle.TakeCrowdControl(type ,1f);
    }
    /// <summary>
    /// 적에게 CC기 
    /// </summary>
    public void TargetCrowdControl<T>(T target, CrowdControlType type, float time) where T : Component
    {
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return;
        battle.TakeCrowdControl(type, time);
    }
    /// <summary>
    /// 디버프 안주는 공격
    /// </summary>
    public int TargetAttack<T>(T target, int damage) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamage(false, damage, false); // 상대를 공격
        OnTargetAttackEvent?.Invoke(hitDamage, false);
        return hitDamage;
    }
    /// <summary>
    /// 디버프 안주는 공격 (치명타)
    /// </summary>
    public int TargetAttack<T>(T target, bool isCritical, int damage) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamage(isCritical, damage, false); // 상대를 공격
        OnTargetAttackEvent?.Invoke(hitDamage, isCritical);
        return hitDamage;
    }
    /// <summary>
    /// 디버프 안주는 공격(고정피해)
    /// </summary>
    public int TargetAttack<T>(T target, int damage, bool isIgnoreDef) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamage(damage, isIgnoreDef); // 상대를 공격

        OnTargetAttackEvent?.Invoke(hitDamage, false);
        return hitDamage;
    }
    /// <summary>
    /// 디버프 안주는 공격 (고정피해, 치명타)
    /// </summary>
    public int TargetAttack<T>(T target, bool isCritical,int damage, bool isIgnoreDef) where T : Component
    {
        // 배틀 시스템은 배틀 시스템 끼리 통신 
        // 플레이어 <-> 배틀시스템 <-> 배틀시스템 <->좀비
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamage(isCritical, damage, isIgnoreDef); // 상대를 공격
        OnTargetAttackEvent?.Invoke(hitDamage, isCritical);
        return hitDamage;
    }
    /// <summary>
    /// 가진 모든 디버프 주면서 공격
    /// </summary>
    public int TargetAttackWithDebuff<T>(T target, int damage) where T : Component
    {
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamageWithDebuff(false, damage, HitAdditionalList, false); // 상대를 공격

        OnTargetAttackEvent?.Invoke(hitDamage, false);
        return hitDamage;
    }
    /// <summary>
    /// 가진 모든 디버프 주면서 공격 (치명타)
    /// </summary>
    public int TargetAttackWithDebuff<T>(T target, bool isCritical, int damage) where T : Component
    {
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamageWithDebuff(isCritical, damage, HitAdditionalList, false); // 상대를 공격

        OnTargetAttackEvent?.Invoke(hitDamage, isCritical);
        return hitDamage;
    }
    /// <summary>
    /// 가진 모든 디버프 주면서 공격 (고정피해)
    /// </summary>
    public int TargetAttackWithDebuff<T>(T target, int damage, bool isIgnoreDef) where T : Component
    {
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage = battle.TakeDamageWithDebuff(false,damage, HitAdditionalList, isIgnoreDef); // 상대를 공격

        OnTargetAttackEvent?.Invoke(hitDamage, false);
        return hitDamage;
    }
    /// <summary>
    /// 가진 모든 디버프 주면서 공격 (치명타, 고정피해)
    /// </summary>
    public int TargetAttackWithDebuff<T>(T target, bool isCritical, int damage,  bool isIgnoreDef) where T : Component
    {
        IBattle battle = target.gameObject.GetComponent<IBattle>(); // 상대 배틀시스템 추적
        if (battle == null)
            return 0;
        int hitDamage =  battle.TakeDamageWithDebuff(isCritical, damage, HitAdditionalList,isIgnoreDef); // 상대를 공격

        OnTargetAttackEvent?.Invoke(hitDamage, isCritical);
        return hitDamage;
    }
    #endregion
    #region 피격 메서드
    /// <summary>
    /// 안때리고 특정 디버프만 주기
    /// </summary>
    public void TakeDebuff(HitAdditional debuff)
    {
        AddDebuff(debuff);
    }
    /// <summary>
    /// 안때리고 특정 디버프들만 주기
    /// </summary>
    public void TakeDebuff(List<HitAdditional> debuffs)
    {
        // 디버프 추가
        foreach (HitAdditional debuff in debuffs)
        {
            AddDebuff(debuff);
        }
    }
    /// <summary>
    /// CC기 맞기
    /// </summary>
    public void TakeCrowdControl(CrowdControlType type)
    {
        Hit.TakeCrowdControl(type, 1f);
    }
    /// <summary>
    /// CC기 맞기
    /// </summary>
    /// <param name="type"></param>
    public void TakeCrowdControl(CrowdControlType type,float time)
    {
        Hit.TakeCrowdControl(type, time);
    }
    /// <summary>
    /// 디버프 안주는 공격 맞기 (타입, 치명타 체크)
    /// </summary>
    public int TakeDamage(int damage)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, false);
        CreateDamageText(hitDamage, false);

        OnTakeDamageEvent?.Invoke(damage, false);
        return hitDamage;
    }
    /// <summary>
    /// 디버프 안주는 공격 맞기 (타입, 치명타 체크)
    /// </summary>
    public int TakeDamage(bool isCritical, int damage)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, false);
        CreateDamageText(hitDamage, isCritical);

        OnTakeDamageEvent?.Invoke(damage, isCritical);
        return hitDamage;
    }
    /// <summary>
    /// 디버프 안주는 공격 맞기
    /// </summary>
    public int TakeDamage(int damage, bool isIgnoreDef)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, isIgnoreDef);
        CreateDamageText(hitDamage);

        OnTakeDamageEvent?.Invoke(damage, false);
        return hitDamage;
    }
    /// <summary>
    /// 디버프 안주는 공격 맞기 (타입, 치명타 체크)
    /// </summary>
    public int TakeDamage(bool isCritical, int damage,  bool isIgnoreDef)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, isIgnoreDef);
        CreateDamageText(hitDamage, isCritical);

        OnTakeDamageEvent?.Invoke(damage, isCritical);
        return hitDamage;
    }
    /// <summary>
    /// 공격받으면서 디버프 전부 받기 (타입)
    /// </summary>
    public int TakeDamageWithDebuff(bool isCritical, int damage, List<HitAdditional> debuffs)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, false);
        CreateDamageText(hitDamage, isCritical);
        // 디버프 추가
        foreach (HitAdditional hitAdditional in debuffs)
        {
            AddDebuff(hitAdditional, hitDamage, isCritical);
        }
        OnTakeDamageEvent?.Invoke(damage, isCritical);
        return hitDamage;
    }
    /// <summary>
    /// 공격받으면서 디버프 전부 받기
    /// </summary>
    public int TakeDamageWithDebuff(int damage, List<HitAdditional> debuffs, bool isIgnoreDef)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, isIgnoreDef);
        CreateDamageText(hitDamage);
        // 디버프 추가
        foreach (HitAdditional hitAdditional in debuffs) 
        {
            AddDebuff(hitAdditional, hitDamage, false);
        }
        OnTakeDamageEvent?.Invoke(damage, false);
        return hitDamage;
    }
    /// <summary>
    /// 공격받으면서 디버프 전부 받기 (타입)
    /// </summary>
    public int TakeDamageWithDebuff(bool isCritical, int damage, List<HitAdditional> debuffs, bool isIgnoreDef)
    {
        // 데미지 주기
        int hitDamage = Hit.TakeDamage(damage, isIgnoreDef);
        CreateDamageText(hitDamage, isCritical);
        // 디버프 추가
        foreach (HitAdditional hitAdditional in debuffs)
        {
            AddDebuff(hitAdditional, hitDamage, isCritical);
        }
        OnTakeDamageEvent?.Invoke(damage, isCritical);
        return hitDamage;
    }
    #endregion
    #region 데미지 UI 
    /// <summary>
    /// 데미지 UI 띄우기
    /// </summary>
    private void CreateDamageText(int damage)
    {
        if (damage <= 0)
            return;
        DamageText text = Instantiate(DataContainer.GetDamageText(CrowdControlType.None), transform.position, Quaternion.identity);
        bool isPlayer = gameObject.tag == Tag.Player;
        text.SetDamageText(damage, HitPoint, isPlayer);
    }
    /// <summary>
    /// 데미지 UI 띄우기
    /// </summary>
    private void CreateDamageText(int damage, bool isCritical)
    {
        if (damage <= 0)
            return;
        DamageText text = Instantiate(DataContainer.GetDamageText(CrowdControlType.None), transform.position, Quaternion.identity);
        bool isPlayer = gameObject.tag == Tag.Player;
        text.SetDamageText(damage, HitPoint, CrowdControlType.None, isCritical, isPlayer);
    }
    /// <summary>
    /// 데미지 UI 띄우기
    /// </summary>
    private void CreateDamageText(int damage, CrowdControlType type)
    {
        if (damage <= 0)
            return;

        DamageText text = Instantiate(DataContainer.GetDamageText(CrowdControlType.None), transform.position, Quaternion.identity);
        bool isPlayer = gameObject.tag == Tag.Player;
        text.SetDamageText(damage, HitPoint, type, false, isPlayer);
    }
    /// <summary>
    /// 데미지 UI 띄우기
    /// </summary>
    private void CreateDamageText(int damage, CrowdControlType type, bool isCritical)
    {
        if (damage <= 0)
            return;

        DamageText text = Instantiate(DataContainer.GetDamageText(CrowdControlType.None), transform.position, Quaternion.identity);
        bool isPlayer = gameObject.tag == Tag.Player;
        text.SetDamageText(damage, HitPoint, type, isCritical, isPlayer);
    }
    #endregion
    #region 효과 등록
    /// <summary>
    /// 적중 효과 등록
    /// </summary>
    /// <param name="hitAdditional"></param>
    public void AddHitAdditionalList(HitAdditional hitAdditional)
    {
        // 중복체크
        int index = HitAdditionalList.FindIndex(origin => origin.Origin.Equals(hitAdditional.Origin));
        // 중복시 등록안함
        if (index != -1)
            return;
        HitAdditionalList.Add(hitAdditional);
    }
    /// <summary>
    /// 적중 효과 삭제
    /// </summary>
    /// <param name="hitAdditional"></param>
    public void RemoveHitAdditionalList(HitAdditional hitAdditional)
    {
        // 중복체크
        int index = HitAdditionalList.FindIndex(origin => origin.Origin.Equals(hitAdditional.Origin));
        // 중복 없을 시 삭제 안함
        if (index == -1)
            return;
        HitAdditionalList.Remove(hitAdditional);
    }
    #endregion
    #region 디버프 추가/삭제
    /// <summary>
    /// 디버프 추가
    /// </summary>
    /// <param name="debuff"></param>
    private void AddDebuff(HitAdditional debuff)
    {
        if (IsDie == true)
            return;

        int index = DebuffList.FindIndex(origin => origin.Origin.Equals(debuff.Origin));
        if (index >= DebuffList.Count)
            return;
        // 디버프 중복 시
        if (index != -1)
        {
            // 기존 디버프 지속시간 갱신
            DebuffList[index].Init(0, false, DebuffList[index].Duration);
            // 디버프 재 발동
            DebuffList[index].Enter();
        }
        else
        {
            HitAdditional cloneDebuff = Instantiate(debuff);
            // 디버프 추가 후 발동
            DebuffList.Add(cloneDebuff);
            cloneDebuff.Origin = debuff.Origin;
            cloneDebuff.Battle = this;
            cloneDebuff.Init(0, false, cloneDebuff.Duration);
            cloneDebuff.Enter(); // 디버프 발동
        }
    }
    /// <summary>
    /// 디버프 추가
    /// </summary>
    /// <param name="debuff"></param>
    private void AddDebuff(HitAdditional debuff, int damage, bool isCritical)
    {
        if (IsDie == true)
            return;

        int index = DebuffList.FindIndex(origin => origin.Origin.Equals(debuff.Origin));
        if (index >= DebuffList.Count)
            return;
        // 디버프 중복 시
        if (index != -1)
        {
            // 기존 디버프 지속시간 갱신
            DebuffList[index].Init(damage, isCritical, DebuffList[index].Duration);
            // 디버프 재 발동
            DebuffList[index].Enter();
        }
        else
        {
            HitAdditional cloneDebuff = Instantiate(debuff);
            // 디버프 추가 후 발동
            DebuffList.Add(cloneDebuff);
            cloneDebuff.Origin = debuff.Origin;
            cloneDebuff.Battle = this;
            cloneDebuff.Init(damage, isCritical, cloneDebuff.Duration);
            cloneDebuff.Enter(); // 디버프 발동
        }
    }
    /// <summary>
    /// 디버프 삭제
    /// </summary>
    /// <param name="debuff"></param>
    private void RemoveDebuff(HitAdditional debuff)
    {
        debuff.Exit();
        DebuffList.Remove(debuff);
        Destroy(debuff);
    }
    /// <summary>
    /// 디버프 모두 삭제(정리)
    /// </summary>
    private void ClearDebuff()
    {
        foreach (HitAdditional debuff in DebuffList)
        {
            debuff.Exit();
            Destroy(debuff);
        }
        DebuffList.Clear();
    }
    #endregion
    #region 콜백
    public void Enter()
    {
        foreach (HitAdditional debuff in DebuffList) 
        {
            debuff.Enter();
        }
    }

    public void Exit()
    {
        foreach (HitAdditional debuff in DebuffList)
        {
            debuff.Exit();
        }
    }

    public void Update()
    {
        foreach (HitAdditional debuff in DebuffList)
        {
            debuff.Update();
        }
    }

    public void FixedUpdate()
    {
        foreach (HitAdditional debuff in DebuffList)
        {
            debuff.FixedUpdate();
        }
    }

    public void Trigger()
    {
        foreach (HitAdditional debuff in DebuffList)
        {
            debuff.Trigger();
        }
    }
    #endregion
    /// <summary>
    /// 디버프 종료 호출
    /// </summary>
    public void EndDebuff(HitAdditional debuff)
    {
        RemoveDebuff(debuff);
    }
    public void Die()
    {
        IsDie = true;
        OnDieEvent?.Invoke();
        ClearDebuff();
    }
}
