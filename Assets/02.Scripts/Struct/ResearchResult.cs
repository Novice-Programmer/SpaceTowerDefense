using System;
using System.Collections.Generic;
using UnityEngine;

public enum ESpecialResearch
{
    KW9A_MultipleRocket,
    KW9A_BigBangRocket,
    FireWall_IceWall,
    FireWall_DiamondWall,
    None
}

[Serializable]
public class ResearchResult
{
    [Header("Tower")]
    public float hpAddRate; // 체력 증가량
    public float epAddRate; // 전력 증가량
    public float atkAddRate; // 공격력 증가량
    public float atkSpdAddRate; // 공격 속도 증가량
    public float atkRangeAddRate; // 공격 범위 증가량
    public float defAddRate; // 방어력 증가량
    public float spAddRate; // 특수 능력치 증가량
    public float towerCostReduceRate; // 비용 감소량
    public int startATKUpgrade; // 시작 공격 업그레이드
    public int startDEFUpgrade; // 시작 방어 업그레이드
    public int startSPUpgrade; // 시작 특수 업그레이드
    public int maxATKUpgradeAdd; // 최대 공격 업그레이드 증가
    public int maxDEFUpgradeAdd; // 최대 방어 업그레이드 증가
    public int maxSPUpgradeAdd; // 최대 특수 업그레이드 증가

    [Header("Obstacle")]
    public float valueIncreaseRate; // 특수 능력치 증가량
    public float obstacleCostReduceRate; // 비용 감소량

    [Header("Resource")]
    public float towerPartAddRate; // 부품 증가량
    public float mineralAddRate; // 미네랄 증가량
    public float occasionalAddRate; // 수시 최대 지급 증가
    public float occasionalReduceTime; // 수시 시간 감소량

    [Header("All")]
    public List<ESpecialResearch> specialResearchs = new List<ESpecialResearch>();
}
