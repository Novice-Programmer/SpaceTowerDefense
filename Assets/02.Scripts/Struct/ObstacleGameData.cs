using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ObstacleGameData
{
    public EAttackAble attackAble;
    public EObjectName objectName;
    public string obstacleName;
    public string description;
    public int durability;
    public int reduceValue;
    public int buildCost;
    public float[] values;
    public ESpecialResearch specialResearch; // 특수 업그레이드

    public ObstacleGameData(ObstacleData obstacleData)
    {
        objectName = obstacleData.objectName;
        attackAble = obstacleData.attackAble;
        obstacleName = obstacleData.obstacleName;
        description = obstacleData.description;
        durability = obstacleData.durability;
        reduceValue = obstacleData.reduceValue;
        buildCost = obstacleData.buildCost;
        values = new float[obstacleData.values.Length];
        for(int i = 0; i < values.Length; i++)
        {
            values[i] = obstacleData.values[i];
        }
        specialResearch = ESpecialResearch.None;
    }

    public void CostCheck()
    {
        int reduceCost = (int)(buildCost * ResearchManager.Instance.GameResearchData.obstacleCostReduceRate * 0.01f);
        if (buildCost + reduceCost > 0)
        {
            buildCost += reduceCost;
        }
    }
}