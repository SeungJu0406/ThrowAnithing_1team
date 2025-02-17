using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : BaseEnemy, IHit
{
    public enum PhaseType { Phase1, Phase2, Phase3 }

    [Header("공격 효과음")]
    [SerializeField] List<AudioClip> attackClips;
    [Header("1페이즈 패턴 효과음")]
    [Tooltip("일레트릭 아머 시전 시 효과음")]
    [SerializeField] AudioClip armorShieldStartClip;
    [Tooltip("일레트릭 아머 해제 시 효과음")]
    [SerializeField] AudioClip armorShieldReleaseClip;
    [Tooltip("라이트닝 노바 차징 시 효과음")]
    [SerializeField] AudioClip novaChargeClip;
    [Tooltip("라이트닝 노바 바닥 타격 시 효과음")]
    [SerializeField] AudioClip novaHitClip;
    [Tooltip("라이트닝 피스트 타격 시 효과음")]
    [SerializeField] AudioClip fistHitClip;

    [Header("2페이즈 패턴 효과음")]
    [Tooltip("진입 효과음")]
    [SerializeField] AudioClip joinClip;
    [Tooltip("회복 효과음")]
    [SerializeField] AudioClip healClip;
    [Tooltip("실드 타격 시 효과음")]
    [SerializeField] List<AudioClip> shieldHitClips;
    [Tooltip("실드 파괴 시 효과음")]
    [SerializeField] AudioClip shieldBrokenClip;
    [Tooltip("그로기 효과음")]
    [SerializeField] List<AudioClip> groggyClips;
    [Header("3페이즈 패턴 효과음")]
    [Tooltip("점프 공격 점프 시 효과음")]
    [SerializeField] AudioClip jumpClip;
    [Tooltip("점프 공격 착지 시 효과음")]
    [SerializeField] AudioClip jumpDownClip;

    [Header("현재 페이즈")]
    public PhaseType curPhase = PhaseType.Phase1;
    [Header("회복할 최대 시간 ( 초 단위)")]
    [SerializeField] int maxTime;
    [Header("회복할 최대 HP ( % 단위)"), Range(0, 100)]
    [SerializeField] int maxRecoveryHp;
    [Header("2페이즈에 생성되는 실드 파괴 카운트")]
    [SerializeField] int breakshieldCount = 20;
    [Header("실드 파괴 후 그로기 시간 ( 초 단위)")]
    [SerializeField] float stunTime;
    [Header("프렌지 패시브 버프량"), Range(0, 100)]
    [SerializeField] int frenzyPersent;
    [Header("점프 공격 관련")]
    [Tooltip("점프 애니메이션 모션")]
    public AnimationCurve curve;    // 움직이는 모션
    [Tooltip("점프 애니메이션의 재생 시간")]
    public float jumpAttackTime;    // 애니메이션의 재생 시간
    [Tooltip("점프 시 최대 높이")]
    public float jumpHeight;    // 점프 시 최대 높이
    [Tooltip("점프 공격 범위")]
    public float jumpRange;    // 점프 공격 범위
    [Header("패턴's 이펙트")]
    [SerializeField] ParticleSystem armorShieldParticle;  // 일레트릭 아머
    [SerializeField] ParticleSystem novaParticle;       // 라이트닝 노바
    [SerializeField] ParticleSystem fistParticle;       // 라이트닝 피스트
    [SerializeField] ParticleSystem healParticle;       // 회복
    [SerializeField] ParticleSystem jumpParticle;       // 점프 공격 시작 
    [SerializeField] ParticleSystem jumpDownParticle;   // 점프 공격 끝

    [Space, SerializeField] GameObject fistGroundParticle; // 라이트닝 피스트 바닥효과
    [SerializeField] Transform pos;
    [Header("체력 UI")]
    public Slider hpSlider;         // 체력 바 슬라이드
    public TMP_Text hpPersentText;  // 체력 바 퍼센트
    public Transform viewModel;     // 피격 범위 오브젝트

    private Coroutine attackAble;
    private Coroutine globalCoolTime;
    public Coroutine recovery;  // 회복 관련 코루틴

    private bool onEntryStop;
    private float _curHp;
    [HideInInspector] public bool createShield;
    [HideInInspector] public bool breakShield;
    [HideInInspector] public Vector3 playerPos;

    private void Start()
    {
        BaseInit();
        StateInit();
        hpSlider.maxValue = state.MaxHp;
        StartCoroutine(PassiveOn());
        this.UpdateAsObservable()
            .Select(x => CurHp)
            .Subscribe(x => ChangePhase());
        this.UpdateAsObservable()
            .Select(x => CurHp)
            .Subscribe(x => ChangeHp());
    }

    private void StateInit()
    {
        tree.SetVariableValue("MaxTime", maxTime);
        tree.SetVariableValue("MaxRecoveryHp", maxRecoveryHp);
        tree.SetVariableValue("StunTime", stunTime);
    }

    private void ChangePhase()
    {
        // 현재 체력으로 페이즈 변경
        if (CurHp > MaxHp * 0.8f && onEntryStop == false)
        {
            curPhase = PhaseType.Phase1;
            onEntryStop = true;
        }
        else if (CurHp <= MaxHp * 0.8f && CurHp > MaxHp * 0.5f)
        {
            curPhase = PhaseType.Phase2;
        }
        else if (CurHp <= MaxHp * 0.5f && CurHp > MaxHp * 0.3f)
        {
            curPhase = PhaseType.Phase3;
        }
    }

    private void ChangeHp()
    {
        _curHp = (float)CurHp / state.MaxHp * 100f;
        float curHpPersent = MathF.Floor(_curHp * 100f) / 100f;

        hpSlider.value = CurHp;
        if (curHpPersent > 0)
            hpPersentText.text = curHpPersent.ToString();
        else
            hpPersentText.text = "0";
    }

    IEnumerator PassiveOn()
    {
        float buffPersent = frenzyPersent / 100f;
        SharedFloat globalCoolTime = tree.GetVariable("GlobalCoolTime") as SharedFloat;
        while (true)
        {
            if (curPhase == PhaseType.Phase3)
            {
                tree.SetVariableValue("Speed", MoveSpeed + (MoveSpeed * buffPersent));
                tree.SetVariableValue("AtkDelay", AttackSpeed - (AttackSpeed * buffPersent));
                tree.SetVariableValue("GlobalCoolTime", globalCoolTime.Value - (globalCoolTime.Value * buffPersent));
                yield break;
            }

            yield return null;
        }
    }

    // 회복관련 루틴
    public void RecoveryStartCoroutine(int time, float value)
    {
        SoundManager.PlaySFX(joinClip);
        healParticle.Play();
        recovery = StartCoroutine(RecoveryRoutin(time, value));
    }
    public void RecoveryStopCotoutine()
    {
        healParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        StopCoroutine(recovery);
        transform.GetComponent<Animator>().SetBool("Recovery", false);
        SoundManager.PlaySFX(ChoiceAudioClip(groggyClips));
    }
    /// <summary>
    /// 회복 루틴
    /// </summary>
    /// <param name="maxTime">최대 회복 시간</param>
    /// <param name="recoveryValue">초당 회복하는 양(%)</param>
    IEnumerator RecoveryRoutin(int maxTime, float recoveryValue)
    {
        int time = maxTime;
        int recoveryHp = Mathf.RoundToInt(state.MaxHp * recoveryValue);

        while (time > 0)    // 회복 하는 시간
        {
            yield return 1f.GetDelay();
            SoundManager.PlaySFX(healClip);
            CurHp += recoveryHp;
            time--;
        }

        // 회복 끝
        createShield = false;
        transform.GetComponent<Animator>().SetBool("Recovery", false);
        healParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    /// <summary>
    /// 몬스터가 피해받는 데미지
    /// </summary>
    public new int TakeDamage(int damage, bool isIgnoreDef)
    {
        if (createShield == true)
        {
            SoundManager.PlaySFX(ChoiceAudioClip(shieldHitClips));
            breakshieldCount--;
            if (breakshieldCount <= 0) // 실드 깨짐
            {
                SoundManager.PlaySFX(shieldBrokenClip);
                breakShield = true;
                createShield = false;
            }

            return 0;
        }

        resultDamage = damage - (int)state.Def;
        tree.SetVariableValue("TakeDamage", true);

        if (resultDamage <= 0)
            resultDamage = 0;
        else if (resultDamage < CurHp)
            SoundManager.PlaySFX(ChoiceAudioClip(hitCilps));

        CurHp -= resultDamage;

        return resultDamage;
    }

    /// <summary>
    /// 글로벌 쿨타임 가동 함수
    /// </summary>
    public void StartGlobalCoolTime(SharedBool atkAble, float coolTime)
    {
        if (globalCoolTime != null)
        {
            Debug.Log(123);
            return;
        }

        globalCoolTime = StartCoroutine(CoolTimeRoutine(atkAble, coolTime));
    }

    #region 애니메이션 이벤트
    /// <summary>
    /// Move 애니메이션 이벤트
    /// </summary>
    public void FootStep()
    {
        if (stepCount >= 5)
        {
            SoundManager.PlaySFX(ChoiceAudioClip(voiceClips));
            stepCount = 0;
        }

        SoundManager.PlaySFX(moveClips[randomMoveClip]);
        stepCount++;
    }

    /// <summary>
    /// Attack 애니메이션 이벤트
    /// </summary>
    public void OnHitBegin()
    {
    }
    public void OnHitEnd()
    {
    }

    /// <summary>
    /// Attack_3 애니메이션 이벤트
    /// </summary>
    public void ThunderStomp()
    {
        StartCoroutine(ArmorShieldRoutine());
        armorShieldParticle.Play();
        SoundManager.PlaySFX(armorShieldStartClip);
    }
    // 실드 사라지는 소리 루틴
    IEnumerator ArmorShieldRoutine()
    {
        float time = armorShieldParticle.main.duration;
        yield return time.GetDelay();
        SoundManager.PlaySFX(armorShieldReleaseClip);
    }

    /// <summary>
    ///  라이트닝 노바 애니메이션 이벤트
    /// </summary>
    public void NovaRangeView()
    {
        StartCoroutine(ViewRangeRoutine(viewModel, 3, 10));
    }
    public void NovaCharge()
    {
        SoundManager.PlaySFX(novaChargeClip);
    }
    public void LightningNova()
    {
        novaParticle.Play();
        SoundManager.PlaySFX(novaHitClip);
    }
    /// <summary>
    /// 피격 범위 커지는 루틴
    /// </summary>
    /// <param name="viewModel">피격 범위 오브젝트</param>
    /// <param name="time">진행되는 최대 시간</param>
    /// <param name="maxScale">최대 피격 범위 크기</param>
    IEnumerator ViewRangeRoutine(Transform viewModel, int time, float maxScale)
    {
        // 정해진 것 - 최대 시간, 최대 크기
        float addScale = maxScale / time;   // 커지는 크기
        Vector3 addScaleVector = new Vector2(addScale, addScale);

        viewModel.gameObject.SetActive(true);
        viewModel.localScale = Vector3.zero;

        while (time > 0)
        {
            yield return 0.3f.GetDelay();
            viewModel.localScale += addScaleVector;
            time--;
        }

        yield return 0.3f.GetDelay();
        viewModel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 사망 애니메이션 이벤트
    /// </summary>
    public void DieEff()
    {
        SoundManager.PlaySFX(ChoiceAudioClip(deathCilps));
    }
    public void ShakingForAni()
    {
        dieParticle.Play();
    }

    /// <summary>
    /// 일반 공격 애니메이션 이벤트
    /// </summary>
    public void Shooting()
    {
        // 일반 근접 공격 - 모든 페이즈에 존재
        if (transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack"))
        {
            AttackMelee();
            SoundManager.PlaySFX(ChoiceAudioClip(attackClips));
        }
        // 라이트닝 피스트 - 1페이즈에만 존재
        else if (curPhase == PhaseType.Phase1)
        {
            fistParticle.Play();
            CreateElectricZone electricZone = ObjectPool.Get(fistGroundParticle.gameObject, pos.position, pos.rotation, 4.5f).GetComponent<CreateElectricZone>();
            electricZone.battle = Battle;
            SoundManager.PlaySFX(fistHitClip);
        }
    }

    // 점프 공격 애니메이션 이벤트
    public void JumpAttackBegin()
    {
        jumpParticle.Play();
        StartCoroutine(JumpRoutine(transform.position, playerPos));
        SoundManager.PlaySFX(jumpClip);
    }
    public void JumpAttackRangeView()
    {
        GameObject hitRange = Instantiate(viewModel.gameObject, playerPos + (Vector3.up * 0.1f), Quaternion.identity);
        
        if(hitRange.activeSelf != true)
            hitRange.SetActive(true);

        hitRange.transform.localScale = Vector2.one * jumpRange;
        hitRange.transform.rotation = Quaternion.Euler(90f, 0, 0);
        Destroy(hitRange, 1f);
    }
    public void JumpAttackEnd()
    {
        viewModel.gameObject.SetActive(false);
        jumpDownParticle.Play();
        TakeChargeBoom(jumpRange, 50);
        SoundManager.PlaySFX(jumpDownClip);
    }
    /// <summary>
    /// 플레이어 현재 위치 가져오기
    /// </summary>
    public void SetPlayerPos(Vector3 pos)
    {
        playerPos = pos;
    }
    /// <summary>
    /// 점프 공격 루틴
    /// </summary>
    IEnumerator JumpRoutine(Vector3 start, Vector3 end)
    {
        float currentAttackTime = 0f;   // 현재 재생되는 애니메이션 시간
        while (currentAttackTime <= jumpAttackTime)
        {
            float t = currentAttackTime / jumpAttackTime;
            Vector3 pos = Vector3.zero;
            pos.x = Vector3.Lerp(start, end, t).x;
            pos.z = Vector3.Lerp(start, end, t).z;
            pos.y = curve.Evaluate(t) * jumpHeight + Mathf.Lerp(start.y, end.y, t);
            transform.position = pos;
            currentAttackTime += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    /// <summary>
    /// 근접 공격
    /// </summary>
    public void AttackMelee()
    {
        // 전방 앞에 있는 몬스터들을 확인하고 피격 진행
        // 1. 전방에 있는 몬스터 확인
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 attackPos = playerPos;
        int hitCount = Physics.OverlapSphereNonAlloc(attackPos, state.AttackDis, overLapCollider, 1 << Layer.Player);
        for (int i = 0; i < hitCount; i++)
        {
            // 2. 각도 내에 있는지 확인
            Vector3 source = transform.position;
            source.y = 0;
            Vector3 destination = overLapCollider[i].transform.position;
            destination.y = 0;
            Vector3 targetDir = (destination - source).normalized;
            float targetAngle = Vector3.Angle(transform.forward, targetDir); // 아크코사인 필요 (느리다)

            if (targetAngle > 120 * 0.5f)
                continue;

            int finalDamage = state.Atk;
            // 데미지 주기
            Battle.TargetAttackWithDebuff(overLapCollider[i], finalDamage);
        }
    }

    /// <summary>
    /// 공격 가능 여부 확인
    /// </summary>
    public void AttackAble()
    {
        if (attackAble != null)
            return;

        attackAble = StartCoroutine(AttackDelayRoutine());
    }

    /// <summary>
    /// 공격 딜레이
    /// </summary>
    IEnumerator AttackDelayRoutine()
    {
        // 공격 딜레이 시작
        tree.SetVariableValue("AttackAble", false);
        yield return state.AtkDelay.GetDelay();
        // 공격 딜레이 끝
        tree.SetVariableValue("AttackAble", true);
    }

    #region Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (true == Physics.BoxCast(transform.position, transform.lossyScale / 2.0f, transform.forward, out RaycastHit hit, transform.rotation, 8))
        {
            // Hit된 지점까지 ray를 그려준다.
            Gizmos.DrawRay(transform.position, transform.forward * hit.distance);

            // Hit된 지점에 박스를 그려준다.
            Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.lossyScale);
        }
        else
        {
            // Hit가 되지 않았으면 최대 검출 거리로 ray를 그려준다.
            Gizmos.DrawRay(transform.position, transform.forward * 8);
        }

        // 거리 그리기
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, state.TraceDis);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, state.AttackDis);
    }
    #endregion
}
