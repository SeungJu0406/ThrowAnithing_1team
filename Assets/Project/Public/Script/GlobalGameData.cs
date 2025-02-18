using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 글로벌 게임 데이터
/// 담당자: 정현수
/// 사용 시 허락 맡으시오
/// </summary>
//public enum AmWeapon { Balance, Power, Speed }
[System.Serializable]
public partial class GlobalGameData
{
    // 보유 재화
    public int coin;
    // 최대 보유 재화
    public int maxCoin = 9999999;
    // 날짜와 시간
    public string saveDateTime;
    // 로비 특성 강화 슬롯
    public int[] upgradeLevels = new int[20];
    // 강화 비용 
    public int[] upgradeCosts = { 1000, 1000, 1000, 1000,
                                  5000, 5000, 5000, 5000,
                                  10000, 10000, 10000, 10000,
                                  30000, 30000, 30000, 30000,
                                  50000, 50000, 50000, 50000};
    public int usingCoin;
    public bool bringData = true;

    public enum AmWeapon { Balance, Power, Speed }
    public AmWeapon nowWeapon;
    // 로비 공유 특성 강화 로직
    public bool BuyUpgradeSlot(int slot)
    {
        if (!bringData)
        {
            // 슬롯 20개의 범위 내 인지 확인
            if (slot < 0 || slot >= upgradeLevels.Length)
            {
                Debug.Log("잘못된 슬롯 번호입니다.");
                return false;
            }

            // 강화 단계 확인
            int currentLevel = upgradeLevels[slot];
            if (currentLevel > 4)
            {
                Debug.Log("이미 최대 단계에 도달했습니다.");
                return false;
            }

            // 강화 비용 체크
            int cost = upgradeCosts[slot];
            if (coin < cost)
            {
                Debug.Log("코인이 부족합니다.");
                return false;
            }

            // 강화 진행
            coin -= cost;
            usingCoin += cost;
            upgradeLevels[slot]++;
            Debug.Log($"강화 완료: 항목 {slot + 1}, 현재 단계: {upgradeLevels[slot]}");
        }
        return true;
    }

    // 코인 습득 함수 (습득 시 GetCoin(코인수) 실행해야됨)
    public void GetCoin(int getcoin)
    {
        coin += getcoin;
        if (coin > maxCoin)
        {
            coin = maxCoin;  // 최대값을 초과하면 최대값으로 설정
        }
        Debug.Log($"{getcoin}Coin 습득 \n 총 Coin 수 : {coin}");
    }
}


