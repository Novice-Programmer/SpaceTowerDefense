using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour
{
    List<InstallSpawnButton> _spawnButtons = new List<InstallSpawnButton>();
    [SerializeField] Animator _viewAnimator = null;
    [Header("Install")]
    [SerializeField] GameObject _installContainer = null;
    [SerializeField] InstallSpawnButton _prefabInstallBtn = null;
    [SerializeField] Transform _insBtnContainer = null;
    [Header("TowerInfo")]
    [SerializeField] GameObject _towerInfoContainer = null;
    [SerializeField] Text _towerName = null;
    [SerializeField] Image _towerIconImage = null;
    [SerializeField] Button _towerRepairBtn = null;
    [SerializeField] GameObject _towerRepairIcon = null;
    [SerializeField] Text _towerRepairCostTxt = null;
    [SerializeField] Text _towerSellGetTxt = null;
    [SerializeField] Button _towerUpgradeDEFBtn = null;
    [SerializeField] Text _towerDEFTxt = null;
    [SerializeField] Button _towerUpgradeATKBtn = null;
    [SerializeField] Text _towerATKTxt = null;
    [SerializeField] Button _towerUpgradeSPBtn = null;
    [SerializeField] Text _towerSPTxt = null;

    [Header("ObstacleInfo")]
    [SerializeField] GameObject _obstacleInfoContainer = null;
    [SerializeField] Text _obstacleName = null;
    [SerializeField] Image _obstacleIconImage = null;
    [SerializeField] Text _obstacleSellGetTxt = null;

    [Header("EnemyInfo")]
    [SerializeField] GameObject _enemyInfoContainer = null;
    [SerializeField] Text _enemyName = null;
    [SerializeField] Image _enemyIconImage = null;
    [SerializeField] Text _enemyDEC = null;
    [SerializeField] Text _enemyATK = null;
    [SerializeField] Text _enemyDEF = null;
    [SerializeField] Text _enemyHP = null;

    bool _view = false;
    Tower _selectTower;
    Obstacle _selectObstacle;

    private void Awake()
    {
    }

    public void InstallButtonSetting()
    {
        InstallData[] installTowerDatas = ObjectDataManager.Instance.GetInstallData();
        for (int i = 0; i < installTowerDatas.Length; i++)
        {
            InstallSpawnButton spawnButton = Instantiate(_prefabInstallBtn, _insBtnContainer);
            spawnButton.ButtonDataSetting(installTowerDatas[i]);
            _spawnButtons.Add(spawnButton);
        }
    }

    public void ClickInstallViewBtn()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.InfoView, null);
        InputManager.Instance.UITouch();
        if (InputManager.TouchMode != ETouchMode.Touch)
        {
            return;
        }
        _installContainer.SetActive(true);
        _towerInfoContainer.SetActive(false);
        _obstacleInfoContainer.SetActive(false);
        _enemyInfoContainer.SetActive(false);
        if (_view)
        {
            ViewOff();
        }
        else
        {
            ViewOn();
        }
    }

    public void ClickTower(Tower tower)
    {
        if (!_view)
        {
            ViewOn();
        }
        _installContainer.SetActive(false);
        _towerInfoContainer.SetActive(true);
        _obstacleInfoContainer.SetActive(false);
        _enemyInfoContainer.SetActive(false);
        TowerUISetting(tower);
        InputManager.Instance.UITouch();
    }

    public void ClickObstacle(Obstacle obstacle)
    {
        if (!_view)
        {
            ViewOn();
        }
        _installContainer.SetActive(false);
        _towerInfoContainer.SetActive(false);
        _obstacleInfoContainer.SetActive(true);
        _enemyInfoContainer.SetActive(false);
        ObstacleUISetting(obstacle);
        InputManager.Instance.UITouch();
    }

    public void ClickEnemy(Enemy enemy)
    {
        if (!_view)
        {
            ViewOn();
        }
        _installContainer.SetActive(false);
        _towerInfoContainer.SetActive(false);
        _obstacleInfoContainer.SetActive(false);
        _enemyInfoContainer.SetActive(true);
        EnemyUISetting(enemy);
        InputManager.Instance.UITouch();
    }

    void TowerUISetting(Tower tower)
    {
        _selectTower = tower;
        _towerName.text = tower._gameTowerData.towerName;
        _towerIconImage.sprite = ObjectDataManager.Instance.GetImage(tower._objectName);
        TowerCostCheck();
    }

    void TowerCostCheck()
    {
        RepairCheck();
        _towerSellGetTxt.text = _selectTower.TowerGetSellNumber().ToString();
        bool upgradeDEFCheck = ResourceManager.Instance.TowerPartValue >= _selectTower.UpgradeCost(EUpgradeType.Defence);
        _towerDEFTxt.color = Color.white;
        if (_selectTower._upgradeDEF != null)
        {
            upgradeDEFCheck &= _selectTower._gameTowerData.maxDEFUpgrade > _selectTower._upgradeDEF.level;
            if (_selectTower._gameTowerData.maxDEFUpgrade > _selectTower._upgradeDEF.level)
                _towerDEFTxt.text = "DEF Up " + _selectTower.UpgradeCost(EUpgradeType.Defence);
            else
                _towerDEFTxt.text = "Max DEF Upgrade";
        }

        else
        {
            _towerDEFTxt.text = "DEF Up " + _selectTower.UpgradeCost(EUpgradeType.Defence);
        }

        if (ResourceManager.Instance.TowerPartValue < _selectTower.UpgradeCost(EUpgradeType.Defence))
        {
            _towerDEFTxt.color = Color.red;
        }
        _towerUpgradeDEFBtn.interactable = upgradeDEFCheck;

        bool upgradeATKCheck = ResourceManager.Instance.TowerPartValue >= _selectTower.UpgradeCost(EUpgradeType.Attack);
        _towerATKTxt.color = Color.white;
        if (_selectTower._upgradeATK != null)
        {
            upgradeATKCheck &= _selectTower._gameTowerData.maxATKUpgrade > _selectTower._upgradeATK.level;
            if (_selectTower._gameTowerData.maxATKUpgrade > _selectTower._upgradeATK.level)
                _towerATKTxt.text = "ATK Up " + _selectTower.UpgradeCost(EUpgradeType.Attack);
            else
                _towerATKTxt.text = "Max ATK Upgrade";
        }
        else
        {
            _towerATKTxt.text = "ATK Up " + _selectTower.UpgradeCost(EUpgradeType.Attack);
        }
        _towerUpgradeATKBtn.interactable = upgradeATKCheck;
        if (ResourceManager.Instance.TowerPartValue < _selectTower.UpgradeCost(EUpgradeType.Attack))
        {
            _towerATKTxt.color = Color.red;
        }


        bool upgradeSPCheck = ResourceManager.Instance.TowerPartValue >= _selectTower.UpgradeCost(EUpgradeType.Special);
        _towerSPTxt.color = Color.white;
        if (_selectTower._upgradeSP != null)
        {
            upgradeSPCheck &= _selectTower._gameTowerData.maxSPUpgrade > _selectTower._upgradeSP.level;
            if (_selectTower._gameTowerData.maxSPUpgrade > _selectTower._upgradeSP.level)
                _towerSPTxt.text = "SP Up " + _selectTower.UpgradeCost(EUpgradeType.Special);
            else
                _towerSPTxt.text = "Max SP Upgrade";
        }
        else
        {
            _towerSPTxt.text = "SP Up " + _selectTower.UpgradeCost(EUpgradeType.Special);
        }
        _towerUpgradeSPBtn.interactable = upgradeSPCheck;
        if (ResourceManager.Instance.TowerPartValue < _selectTower.UpgradeCost(EUpgradeType.Special))
        {
            _towerSPTxt.color = Color.red;
        }
    }

    public void RepairCheck()
    {
        if (_selectTower == null)
        {
            return;
        }
        bool towerRepairCheck;
        _towerRepairCostTxt.text = _selectTower.TowerRepairCost().ToString();
        _towerRepairIcon.SetActive(true);
        if (_selectTower._hp > (_selectTower._maxHP * 0.9f))
        {
            towerRepairCheck = false;
            _towerRepairIcon.SetActive(false);
            _towerRepairCostTxt.text = "내구도 충분";
        }
        else if (ResourceManager.Instance.TowerPartValue < _selectTower.TowerRepairCost())
            towerRepairCheck = false;
        else
            towerRepairCheck = true;
        _towerRepairBtn.interactable = towerRepairCheck;
    }

    void ObstacleUISetting(Obstacle obstacle)
    {
        _obstacleName.text = obstacle._gameObstacleData.obstacleName;
        _obstacleIconImage.sprite = ObjectDataManager.Instance.GetImage(obstacle._objectName);
        _obstacleSellGetTxt.text = obstacle._sellGetCost.ToString();
        _selectObstacle = obstacle;
    }

    void EnemyUISetting(Enemy enemy)
    {
        _enemyName.text = enemy._enemyData.enemyFullName;
        _enemyIconImage.sprite = ObjectDataManager.Instance.GetImage(enemy._objectName);
        _enemyDEC.text = enemy._enemyData.description;
        _enemyATK.text = "ATK " + enemy._enemyData.atk.ToString();
        _enemyDEF.text = "DEF " + enemy._enemyData.def.ToString();
        _enemyHP.text = "Max HP " + enemy._enemyData.hp.ToString();
    }

    public void UIValueChange()
    {
        if (_selectTower != null)
        {
            TowerCostCheck();
        }
        for (int i = 0; i < _spawnButtons.Count; i++)
        {
            _spawnButtons[i].InstallMoneyCheck();
        }
    }

    public void ViewOn()
    {
        _view = true;
        _viewAnimator.SetBool("View", true);
    }

    public void ViewOff()
    {
        _view = false;
        _viewAnimator.SetBool("View", false);
    }

    public void ClickTowerRepair()
    {
        int towerPartValue = ResourceManager.Instance.TowerPartValue;
        if (towerPartValue > _selectTower.TowerRepairCost())
        {
            ResourceManager.Instance.TowerPartValue = -_selectTower.TowerRepairCost();
            _selectTower.TowerRepair();
            _selectTower.TotalCostAdd(_selectTower.TowerRepairCost());
        }
        InputManager.Instance.ObjectSelectClose();
        _selectTower = null;
        InputManager.Instance.UITouch();
    }

    public void ClickTowerSell()
    {
        ResourceManager.Instance.TowerPartValue = _selectTower.TowerGetSellNumber();
        InputManager.Instance.ObjectSelectClose();
        _selectTower.SellTower();
        _selectTower = null;
        InputManager.Instance.UITouch();
    }

    public void ClickObstacleSell()
    {
        ResourceManager.Instance.TowerPartValue = _selectObstacle._sellGetCost;
        InputManager.Instance.ObjectSelectClose();
        _selectObstacle.DestroyObstacle();
        _selectObstacle = null;
        InputManager.Instance.UITouch();
    }

    public void ClickTowerUpgradeDEF()
    {
        ResourceManager.Instance.TowerPartValue = -_selectTower.UpgradeCost(EUpgradeType.Defence);
        _selectTower.TotalCostAdd(_selectTower.UpgradeCost(EUpgradeType.Defence));
        _selectTower._upgradeDEF = ObjectDataManager.Instance.GetUpgradeData(_selectTower._objectName, EUpgradeType.Defence, ++_selectTower._levelDEF);
        _selectTower.TowerUpgrade(EUpgradeType.Defence);
        InputManager.Instance.ObjectSelectClose();
        _selectTower = null;
        InputManager.Instance.UITouch();
    }

    public void ClickTowerUpgradeATK()
    {
        ResourceManager.Instance.TowerPartValue = -_selectTower.UpgradeCost(EUpgradeType.Attack);
        _selectTower.TotalCostAdd(_selectTower.UpgradeCost(EUpgradeType.Attack));
        _selectTower._upgradeATK = ObjectDataManager.Instance.GetUpgradeData(_selectTower._objectName, EUpgradeType.Attack, ++_selectTower._levelATK);
        _selectTower.TowerUpgrade(EUpgradeType.Attack);
        InputManager.Instance.ObjectSelectClose();
        _selectTower = null;
        InputManager.Instance.UITouch();
    }

    public void ClickTowerUpgradeSP()
    {
        ResourceManager.Instance.TowerPartValue = -_selectTower.UpgradeCost(EUpgradeType.Special);
        _selectTower.TotalCostAdd(_selectTower.UpgradeCost(EUpgradeType.Special));
        _selectTower._upgradeSP = ObjectDataManager.Instance.GetUpgradeData(_selectTower._objectName, EUpgradeType.Special, ++_selectTower._levelSP);
        _selectTower.TowerUpgrade(EUpgradeType.Special);
        InputManager.Instance.ObjectSelectClose();
        _selectTower = null;
        InputManager.Instance.UITouch();
    }
}
