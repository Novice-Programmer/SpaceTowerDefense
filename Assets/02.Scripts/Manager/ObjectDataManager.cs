using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EObjectType
{
    None,
    Enemy,
    Tower,
    Obstacle,
    Commander
}

public enum EObjectName
{
    None,
    KW9A,
    P013,
    NMDA,
    FireWall,
    Commander,
    Swamp,
    BDU
}

public class ObjectDataManager : TSingleton<ObjectDataManager>
{
    [SerializeField] Ghost[] _prefabGhosts = null;

    [Header("Tower")]
    [SerializeField] TowerData[] _towerAllDatas = null;
    [SerializeField] TowerUpgradeData[] _upgradeAllData = null;
    [SerializeField] Tower[] _prefabTowers = null;
    [SerializeField] Sprite[] _towerImages = null;

    Dictionary<EObjectName, Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>>> _towerUpgradeDic
    = new Dictionary<EObjectName, Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>>>();

    Dictionary<EObjectName, TowerGameData> _gameTowerDatas = new Dictionary<EObjectName, TowerGameData>();

    [Header("Obstacle")]
    [SerializeField] ObstacleData[] _obstacleAllDatas = null;
    [SerializeField] Obstacle[] _prefabObstacle = null;
    [SerializeField] Sprite[] _obstacleImages = null;

    Dictionary<EObjectName, ObstacleGameData> _gameObstacleDatas = new Dictionary<EObjectName, ObstacleGameData>();

    [Header("Enemy")]
    [SerializeField] EnemyData[] _enemyAllDatas = null;
    [SerializeField] Sprite[] _enemyIconSprites = null;
    [SerializeField] Sprite[] _enemyRankSprites = null;

    [Header("Mark")]
    [SerializeField] Transform _markerContainer = null;
    [SerializeField] Marker _prefabMarker = null;
    [SerializeField] Sprite[] _markIconSprites = null;
    [SerializeField] Sprite[] _markBackgroundSprites = null;

    [Header("Status")]
    [SerializeField] Transform _statusContainer = null;
    [SerializeField] WorldStatusUI _prefabStatusUI = null;

    private void Awake()
    {
        Init();
        Instance = this;
        TowerDictionarySetting();
    }

    private void Start()
    {
        LoadManager.Instance.Load();
        ResearchManager.Instance.ResearchDictionarySetting();
        GameDataSetting();
    }

    void TowerDictionarySetting()
    {
        for (int i = 0; i < _upgradeAllData.Length; i++)
        {
            Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>> upgradeType;
            if (_towerUpgradeDic.ContainsKey(_upgradeAllData[i].objectName))
            {
                upgradeType = _towerUpgradeDic[_upgradeAllData[i].objectName];
            }
            else
            {
                upgradeType = new Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>>();
                _towerUpgradeDic.Add(_upgradeAllData[i].objectName, upgradeType);
            }

            Dictionary<int, TowerUpgradeData> levelType;
            if (upgradeType.ContainsKey(_upgradeAllData[i].upgradeType))
            {
                levelType = _towerUpgradeDic[_upgradeAllData[i].objectName][_upgradeAllData[i].upgradeType];
            }
            else
            {
                levelType = new Dictionary<int, TowerUpgradeData>();
                upgradeType.Add(_upgradeAllData[i].upgradeType, levelType);
            }

            levelType.Add(_upgradeAllData[i].level, _upgradeAllData[i]);
        }
    }

    public void GameDataSetting()
    {
        for (int i = 0; i < _towerAllDatas.Length; i++)
        {
            TowerGameData towerData = new TowerGameData(_towerAllDatas[i]);
            towerData.CostCheck();
            _gameTowerDatas.Add(towerData.objectName, towerData);
        }
        for (int i = 0; i < _obstacleAllDatas.Length; i++)
        {
            ObstacleGameData obstacleGameData = new ObstacleGameData(_obstacleAllDatas[i]);
            obstacleGameData.CostCheck();
            _gameObstacleDatas.Add(obstacleGameData.objectName, obstacleGameData);
        }
    }

    public void GameDataUpdate()
    {
        Dictionary<EObjectName, TowerGameData> updateTowerDatas = new Dictionary<EObjectName, TowerGameData>();
        foreach (TowerGameData towerGameData in _gameTowerDatas.Values)
        {
            TowerGameData updateData = new TowerGameData(GetTowerData(towerGameData.objectName));
            updateData.CostCheck();
            updateTowerDatas.Add(updateData.objectName, updateData);
        }
        Dictionary<EObjectName, ObstacleGameData> updateObstacleDatas = new Dictionary<EObjectName, ObstacleGameData>();
        foreach (ObstacleGameData obstacleGameData in _gameObstacleDatas.Values)
        {
            ObstacleGameData updateData = new ObstacleGameData(GetObstacleData(obstacleGameData.objectName));
            updateData.CostCheck();
            updateObstacleDatas.Add(updateData.objectName, updateData);
        }
        _gameTowerDatas = updateTowerDatas;
        _gameObstacleDatas = updateObstacleDatas;
    }

    public TowerData GetTowerData(EObjectName towerName)
    {
        for (int i = 0; i < _towerAllDatas.Length; i++)
        {
            if (towerName == _towerAllDatas[i].objectName)
            {
                return _towerAllDatas[i];
            }
        }
        return null;
    }

    public ObstacleData GetObstacleData(EObjectName obstacleName)
    {
        for (int i = 0; i < _obstacleAllDatas.Length; i++)
        {
            if (obstacleName == _obstacleAllDatas[i].objectName)
            {
                return _obstacleAllDatas[i];
            }
        }
        return null;
    }

    public void ResearchCheck(EObjectName objectName)
    {
        if (_gameTowerDatas.ContainsKey(objectName))
        {
            ResearchUpdate(objectName);
        }
    }

    void ResearchUpdate(EObjectName objectName)
    {
        _gameTowerDatas[objectName].CostCheck();
    }

    public TowerUpgradeData GetUpgradeData(EObjectName objectName, EUpgradeType upgradeType, int level)
    {
        return _towerUpgradeDic[objectName][upgradeType][level];
    }

    public void GameTowerDataUpdate(EObjectName objectName, TowerGameData towerGameData)
    {
        if (_gameTowerDatas.ContainsKey(objectName))
        {
            _gameTowerDatas[objectName] = towerGameData;
        }
        else
        {
            _gameTowerDatas.Add(objectName, towerGameData);
        }
    }

    public TowerGameData GetTowerGameData(EObjectName objectName)
    {
        return _gameTowerDatas[objectName];
    }

    public ObstacleGameData GetObstacleGameData(EObjectName objectName)
    {
        return _gameObstacleDatas[objectName];
    }

    public InstallData[] GetInstallData()
    {
        List<InstallData> installDatas = new List<InstallData>();

        List<ObjectData> selectDatas = PlayerDataManager.Instance.SelectList;

        for (int i = 0; i < selectDatas.Count; i++)
        {
            InstallData installData = new InstallData();
            if (selectDatas[i].objectType == EObjectType.Tower)
            {
                TowerGameData towerGameData = _gameTowerDatas[selectDatas[i].objectName];
                installData.objectType = EObjectType.Tower;
                installData.installCost = towerGameData.buildCost;
            }
            else
            {
                ObstacleGameData obstacleGameData = _gameObstacleDatas[selectDatas[i].objectName];
                installData.objectType = EObjectType.Obstacle;
                installData.installCost = obstacleGameData.buildCost;
            }
            installData.objectName = selectDatas[i].objectName;
            installData.objectImage = GetImage(selectDatas[i].objectName);
            installDatas.Add(installData);
        }

        return installDatas.ToArray();
    }

    public Ghost GetBuildGhost(EObjectName objectName)
    {
        switch (objectName)
        {
            case EObjectName.KW9A:
                return _prefabGhosts[0];
            case EObjectName.FireWall:
                return _prefabGhosts[1];
            case EObjectName.P013:
                return _prefabGhosts[2];
            case EObjectName.Swamp:
                return _prefabGhosts[3];
        }
        return null;
    }

    public Tower GetTower(EObjectName objectName)
    {
        for (int i = 0; i < _prefabTowers.Length; i++)
        {
            if (_prefabTowers[i]._objectName == objectName)
            {
                return _prefabTowers[i];
            }
        }
        return null;
    }

    public Obstacle GetObstacle(EObjectName objectName)
    {
        for (int i = 0; i < _prefabObstacle.Length; i++)
        {
            if (_prefabObstacle[i]._objectName == objectName)
            {
                return _prefabObstacle[i];
            }
        }
        return null;
    }

    public Sprite GetImage(EObjectName objectName, bool backgroundImage = false)
    {
        switch (objectName)
        {
            case EObjectName.KW9A:
                return _towerImages[0];

            case EObjectName.NMDA:
                if (backgroundImage)
                {
                    return _enemyRankSprites[0];
                }
                else
                {
                    return _enemyIconSprites[0];
                }
            case EObjectName.FireWall:
                return _obstacleImages[0];
            case EObjectName.P013:
                return _towerImages[1];
            case EObjectName.Swamp:
                return _obstacleImages[1];
            case EObjectName.BDU:
                if (backgroundImage)
                {
                    return _enemyRankSprites[3];
                }
                else
                {
                    return _enemyIconSprites[1];
                }
        }
        return null;
    }

    public string GetName(EObjectType objectType, EObjectName objectName)
    {
        if (objectType == EObjectType.Tower)
        {
            return GetTowerData(objectName).towerName;
        }
        else
        {
            return GetObstacleData(objectName).obstacleName;
        }
    }

    public void MarkerSetting(Transform target, EObjectName objectName)
    {
        Marker marker = Instantiate(_prefabMarker, _markerContainer);
        Sprite icon = _markIconSprites[0];
        Sprite background = _markBackgroundSprites[0];
        switch (objectName)
        {
            case EObjectName.KW9A:
                icon = _markIconSprites[0];
                background = _markBackgroundSprites[0];
                break;
            case EObjectName.NMDA:
                icon = _markIconSprites[1];
                background = _markBackgroundSprites[1];
                break;
            case EObjectName.FireWall:
                icon = _markIconSprites[2];
                background = _markBackgroundSprites[2];
                break;
            case EObjectName.Commander:
                icon = _markIconSprites[3];
                background = null;
                break;
            case EObjectName.P013:
                icon = _markIconSprites[4];
                background = _markBackgroundSprites[0];
                break;
            case EObjectName.Swamp:
                icon = _markIconSprites[5];
                background = _markBackgroundSprites[2];
                break;
            case EObjectName.BDU:
                icon = _markIconSprites[6];
                background = _markBackgroundSprites[1];
                break;
        }

        marker.MarkerSetting(target, icon, background);
    }

    public Sprite GetIcon(EObjectName objectName)
    {
        switch (objectName)
        {
            case EObjectName.KW9A:
                return _markIconSprites[0];
            case EObjectName.FireWall:
                return _markIconSprites[2];
            case EObjectName.P013:
                return _markIconSprites[4];
            case EObjectName.Swamp:
                return _markIconSprites[5];
        }
        return null;
    }

    public WorldStatusUI StatusInit()
    {
        return Instantiate(_prefabStatusUI, _statusContainer);
    }

    public List<TowerGameData> GetAllToweGameData()
    {
        List<TowerGameData> towerGameDatas = new List<TowerGameData>();

        foreach (TowerGameData towerGameData in _gameTowerDatas.Values)
        {
            towerGameDatas.Add(towerGameData);
        }
        return towerGameDatas;
    }

    public List<ObstacleGameData> GetAllObstacleGameData()
    {
        List<ObstacleGameData> obstacleGameDatas = new List<ObstacleGameData>();

        foreach (ObstacleGameData obstacleGameData in _gameObstacleDatas.Values)
        {
            obstacleGameDatas.Add(obstacleGameData);
        }
        return obstacleGameDatas;
    }

    public string EnemyName(EObjectName objectName)
    {
        for(int i = 0; i < _enemyAllDatas.Length; i++)
        {
            if (_enemyAllDatas[i].objectName == objectName)
                return _enemyAllDatas[i].enemyFullName;
        }
        return null;
    }

    public ERatingType EnemyRating(EObjectName objectName)
    {
        for (int i = 0; i < _enemyAllDatas.Length; i++)
        {
            if (_enemyAllDatas[i].objectName == objectName)
                return _enemyAllDatas[i].ratingType;
        }
        return ERatingType.None;
    }
}
