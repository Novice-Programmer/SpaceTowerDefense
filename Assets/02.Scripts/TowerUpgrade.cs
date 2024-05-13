using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    public static TowerUpgrade Instance { set; get; }

    [SerializeField] TowerUpgradeData[] _upgradeTower = null;
    Dictionary<EObjectName, Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>>> _towerTypeUpgrade 
        = new Dictionary<EObjectName, Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>>>();

    private void Awake()
    {
        Instance = this;
        TowerDictionarySetting();
    }

    void TowerDictionarySetting()
    {
        for (int i = 0; i < _upgradeTower.Length; i++)
        {
            Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>> upgradeType;
            if (_towerTypeUpgrade.ContainsKey(_upgradeTower[i].objectName))
            {
                upgradeType = _towerTypeUpgrade[_upgradeTower[i].objectName];
            }
            else
            {
                upgradeType = new Dictionary<EUpgradeType, Dictionary<int, TowerUpgradeData>>();
                _towerTypeUpgrade.Add(_upgradeTower[i].objectName, upgradeType);
            }

            Dictionary<int, TowerUpgradeData> levelType;
            if (upgradeType.ContainsKey(_upgradeTower[i].upgradeType))
            {
                levelType = _towerTypeUpgrade[_upgradeTower[i].objectName][_upgradeTower[i].upgradeType];
            }
            else
            {
                levelType = new Dictionary<int, TowerUpgradeData>();
                upgradeType.Add(_upgradeTower[i].upgradeType, levelType);
            }

            levelType.Add(_upgradeTower[i].level, _upgradeTower[i]);
        }
    }

    public TowerUpgradeData TowerGetUpgradeData(EObjectName objectName, EUpgradeType upgradeType, int level)
    {
        return _towerTypeUpgrade[objectName][upgradeType][level];
    }
}
