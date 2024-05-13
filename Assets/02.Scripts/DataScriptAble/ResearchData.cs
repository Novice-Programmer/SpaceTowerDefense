using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EResearchType
{
    Tower,
    Obstacle,
    Resource,
    Facility
}

public enum EResearch
{
    None,
    TowerLv0=1,
    TowerLv1_1,
    TowerLv1_2,
    TowerLv1_3,
    TowerLv2_1,
    TowerLv2_2,
    TowerLv2_3,
    TowerLv3_1,
    TowerLv3_2,
    ObstacleLv0 = 101,
    ObstacleLv1_1,
    ObstacleLv1_2,
    ObstacleLv2_1,
    ObstacleLv2_2,
    ObstacleLv3_1,
    ObstacleLv3_2,
    ResourceLv0 = 201,
    ResourceLv1_1,
    ResourceLv1_2,
    ResourceLv2_1,
    ResourceLv2_2,
    ResourceLv3_1,
    ResourceLv3_2,
}


[CreateAssetMenu(fileName = "GameData", menuName = "Data/ResearchData", order = 4)]
public class ResearchData : ScriptableObject
{
    public EResearch research;
    public int step;
    public string researchName;
    public string description;
    public Sprite icon;
    public EResearchType type;
}
