using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Data/ObstacleData", order = 6)]
public class ObstacleData : ScriptableObject
{
    public EObjectName objectName;
    public EAttackAble attackAble;
    public IntVector2 size;
    public string obstacleName;
    public string description;
    public int durability;
    public int reduceValue;
    public int buildCost;
    public float[] values;
}
