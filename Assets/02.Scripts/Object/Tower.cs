using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Tower : ObjectGame
{
    [Header("TowerInfo")]
    public EObjectName _objectName = EObjectName.None;
    public int _level;
    public int _hp;
    public int _maxHP;
    [SerializeField] protected int _ep;
    [SerializeField] protected int _chargeEP;
    [SerializeField] protected int _atk;
    [SerializeField] protected int _def;
    [SerializeField] protected float _atkSpd;
    [SerializeField] protected float _atkRange;
    [SerializeField] protected int _atkNumber;
    [SerializeField] protected int _targetNumber;
    public int _levelATK;
    public int _levelDEF;
    public int _levelSP;
    [SerializeField] protected float[] _spValue;
    [SerializeField] protected IntVector2 _dimensions;
    protected WorldStatusUI _statusUI;

    [Header("Setup")]
    [SerializeField] Transform _partToRotate = null;
    [SerializeField] float _turnSpeed = 10.0f;
    [SerializeField] GameObject _rangeObject = null;
    MeshRenderer[] _materials = null;
    SkinnedMeshRenderer[] _materials2 = null;
    [SerializeField] List<Material> _activeMaterials = null;
    [SerializeField] Material _breakDownMaterial = null;
    Vector3 _rangeSize;

    public TowerGameData _gameTowerData;
    public TowerUpgradeData _upgradeATK = null;
    public TowerUpgradeData _upgradeDEF = null;
    public TowerUpgradeData _upgradeSP = null;

    protected Transform[] _target;
    protected Transform _nearestTarget;

    float _attackCountdown = 0;
    int _totalCost = 0;

    [SerializeField] Tile _parentTile;
    IntVector2 _gridPosition;
    EFitType _fitType;

    [SerializeField] State _state = null;

    private void Start()
    {
        _rangeSize = _rangeObject.transform.localScale;
        _statusUI = ObjectDataManager.Instance.StatusInit();
        DataSetting();
        StartCoroutine(BuildSuccess());
        InvokeRepeating("UpdateTarget", 0.0f, 0.5f);
        StartCoroutine(StateAction());
        _materials = GetComponentsInChildren<MeshRenderer>();
        _materials2 = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < _materials.Length; i++)
        {
            _activeMaterials.Add(_materials[i].material);
        }
        for (int i = 0; i < _materials2.Length; i++)
        {
            _activeMaterials.Add(_materials2[i].material);
        }
        StateChange(EStateType.AttackSearch);
        ObjectDataManager.Instance.MarkerSetting(transform, _objectName);
    }

    void StateChange(EStateType stateType)
    {
        _stateType = stateType;
        _state.StateUpdate(_stateType);
    }

    #region Aciton


    protected virtual void Attack()
    {
        _attackCountdown = 9999;
    }

    protected void ChargingEP()
    {
        _ep += _chargeEP;
        if (_ep > 100)
        {
            _ep = 100;
        }
        _statusUI.MPChange(_ep);
    }

    protected virtual void AttackEnd()
    {
        _attackCountdown = 1 / _atkSpd;
    }

    protected virtual void SpecialAttack()
    {
        _attackCountdown = 9999;
        _ep = 0;
        _statusUI.MPChange(_ep);
    }

    protected virtual void SkillEnd()
    {
        _ep = 0;
        _statusUI.MPChange(_ep);
        _attackCountdown = 1 / _atkSpd;
    }

    public override void Hit(int damage, EWeakType weakType)
    {
        if (_hp <= 0)
        {
            return;
        }

        _hp -= damage - _def;
        if (_hp <= 0)
        {
            _hp = 0;
            Braekdown();
        }
        _statusUI.HPChange(_hp);
        if (_objectSelect)
            GameUI.Instance.SelectHPValueChange();
    }

    void Braekdown()
    {
        StateChange(EStateType.Breakdown);
        for (int i = 0; i < _materials.Length; i++)
        {
            _materials[i].material = _breakDownMaterial;
        }
        for (int i = 0; i < _materials2.Length; i++)
        {
            _materials2[i].material = _breakDownMaterial;
        }
    }

    void UpdateTarget()
    {
        if (_stateType == EStateType.AttackSearch || _stateType == EStateType.Attack)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            List<Enemy> enemiesRank = new List<Enemy>();
            _nearestTarget = null;
            float shortestDistance = _atkRange;
            for (int i = 0; i < enemies.Length; i++)
            {
                if (Vector3.Distance(transform.position, enemies[i].transform.position) < _atkRange)
                {
                    Enemy enemy = enemies[i].GetComponent<Enemy>();
                    if (enemy._stateType != EStateType.Die)
                    {
                        enemiesRank.Add(enemy);
                        float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                        if (distanceToEnemy < shortestDistance)
                        {
                            shortestDistance = distanceToEnemy;
                            _nearestTarget = enemy.transform;
                        }
                    }
                }
            }

            if (enemiesRank.Count > 0)
            {
                _target = new Transform[_targetNumber];
                for (int i = 0; i < enemiesRank.Count - 1; i++)
                {
                    for (int j = i + 1; j < enemiesRank.Count; j++)
                    {
                        if (enemiesRank[i]._rating < enemiesRank[j]._rating)
                        {
                            Enemy temp = enemiesRank[i];
                            enemiesRank[i] = enemiesRank[j];
                            enemiesRank[j] = temp;
                        }
                        else if (enemiesRank[i]._rating == enemiesRank[j]._rating)
                        {
                            float distanceToEnemyI = Vector3.Distance(transform.position, enemiesRank[i].transform.position);
                            float distanceToEnemyJ = Vector3.Distance(transform.position, enemiesRank[j].transform.position);
                            if (distanceToEnemyI > distanceToEnemyJ)
                            {
                                Enemy temp = enemiesRank[i];
                                enemiesRank[i] = enemiesRank[j];
                                enemiesRank[j] = temp;
                            }
                        }
                    }
                }

                StateChange(EStateType.Attack);
                if (_target.Length <= enemiesRank.Count)
                {
                    for (int i = 0; i < _target.Length; i++)
                    {
                        _target[i] = enemiesRank[i].transform;
                    }
                }
                else
                {
                    for (int i = 0; i < _target.Length; i += enemiesRank.Count)
                    {
                        for (int j = 0; j < enemiesRank.Count; j++)
                        {
                            if (i + j >= _target.Length)
                            {
                                break;
                            }
                            _target[i + j] = enemiesRank[j].transform;
                        }
                    }
                }
            }

            else
            {
                _target = null;
            }
        }
    }

    IEnumerator StateAction()
    {
        while (true)
        {
            if (_stateType == EStateType.Breakdown)
            {
                yield return null;
            }
            else
            {
                if (_target == null)
                {
                    if (_stateType == EStateType.Attack)
                    {
                        StateChange(EStateType.AttackSearch);
                    }
                    else if (_stateType == EStateType.AttackSearch)
                    {
                        _partToRotate.Rotate(_partToRotate.rotation.x, _turnSpeed * Time.deltaTime, _partToRotate.rotation.z);
                    }
                }

                else
                {
                    if (_stateType == EStateType.Attack)
                    {
                        Vector3 dir = Vector3.zero;
                        for (int i = 0; i < _target.Length; i++)
                        {
                            dir = _target[i].position - transform.position;
                        }
                        Quaternion lookRotation = Quaternion.LookRotation(dir / _target.Length);
                        Vector3 rotateValue = Quaternion.Lerp(_partToRotate.rotation, lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;
                        _partToRotate.rotation = Quaternion.Euler(_partToRotate.rotation.x, rotateValue.y, _partToRotate.rotation.z);

                        if (_attackCountdown <= 0)
                        {
                            _attackCountdown = 9999;
                            if (_ep == 100)
                            {
                                SpecialAttack();
                            }
                            else
                            {
                                Attack();
                            }
                            _attackCountdown = 1 / _atkSpd;
                        }
                    }
                }
                _attackCountdown -= Time.deltaTime;
                if (_attackCountdown <= 0)
                {
                    _attackCountdown = 0;
                }
            }
            yield return null;
        }
    }

    #endregion

    #region UIAction

    public void BuildingTower(Ghost ghostTower)
    {
        _parentTile = ghostTower._parentTile;
        _gridPosition = ghostTower._gridPos;
        _fitType = ghostTower._fitType;

        Vector3 rotateEulerV = Vector3.zero;

        switch (ghostTower._rotateType)
        {
            case ERotateType.degree90:
                rotateEulerV = new Vector3(0, 90, 0);
                break;
            case ERotateType.degree180:
                rotateEulerV = new Vector3(0, 180, 0);
                break;
            case ERotateType.degree270:
                rotateEulerV = new Vector3(0, 270, 0);
                break;
        }
        transform.rotation = Quaternion.Euler(rotateEulerV + _parentTile.transform.rotation.eulerAngles);
    }

    IEnumerator BuildSuccess()
    {
        yield return new WaitForSeconds(0.5f);
        _objectSelectActive = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _atkRange);
    }

    public void TowerRepair()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.Repair, null);
        _hp = _maxHP;
        _statusUI.HPChange(_hp);
        _target = null;
        if (_stateType == EStateType.Breakdown)
        {
            StateChange(EStateType.AttackSearch);
            for (int i = 0; i < _materials.Length; i++)
            {
                _materials[i].material = _activeMaterials[i];
            }
            for (int i = 0; i < _materials2.Length; i++)
            {
                _materials2[i].material = _activeMaterials[i + _materials.Length];
            }
        }
    }

    public void SellTower()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.SellObject, null);
        _parentTile.Clear(_gridPosition, _dimensions, _fitType);
        Destroy(_statusUI.gameObject);
        Destroy(gameObject);
    }

    public override void Select(bool selectOff = true)
    {
        base.Select(selectOff);
        _objectSelect = !_objectSelect;
        if (!_objectSelectActive || !selectOff)
        {
            _objectSelect = false;
        }
        _statusUI.SelectViewStatus(_objectSelect);
        if (_objectSelect)
        {
            GameUI.Instance.TowerClick(this);
        }
        else
        {
            GameUI.Instance.ViewUIOff();
        }
        _rangeObject.SetActive(_objectSelect);
    }

    public void TowerUpgrade(EUpgradeType upgradeType)
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.Upgrade, null);
        int maxHP = _maxHP;
        StatusCheck();
        if (upgradeType == EUpgradeType.Defence)
        {
            _statusUI.StatusSetting(_maxHP);
            int addHP = _maxHP - maxHP;
            _hp += addHP;
            _statusUI.HPChange(_hp);
        }
    }

    #endregion

    #region Data


    void DataSetting()
    {
        _gameTowerData = ObjectDataManager.Instance.GetTowerGameData(_objectName);
        _ep = 0;
        _levelATK = 0;
        _levelDEF = 0;
        _levelSP = 0;
        _level = 0;
        ResearchResult researchResult = ResearchManager.Instance.GameResearchData;
        if (researchResult.startATKUpgrade != 0)
        {
            _upgradeATK = ObjectDataManager.Instance.GetUpgradeData(_objectName, EUpgradeType.Attack, researchResult.startATKUpgrade);
        }
        if (researchResult.startDEFUpgrade != 0)
        {
            _upgradeDEF = ObjectDataManager.Instance.GetUpgradeData(_objectName, EUpgradeType.Defence, researchResult.startDEFUpgrade);
        }
        if (researchResult.startSPUpgrade != 0)
        {
            _upgradeSP = ObjectDataManager.Instance.GetUpgradeData(_objectName, EUpgradeType.Special, researchResult.startSPUpgrade);
        }
        StatusCheck();

        _hp = _maxHP;
        _statusUI.StatusSetting(transform, _maxHP, 15);
        _totalCost = _gameTowerData.buildCost;
    }

    void StatusCheck()
    {
        ResearchResult researchResult = ResearchManager.Instance.GameResearchData;

        if (_upgradeATK != null)
        {
            _atk = (int)(_gameTowerData.atk + _upgradeATK.addValue[0] + (_gameTowerData.atk + _upgradeATK.addValue[0]) * researchResult.atkAddRate * 0.01f);
            _atkSpd = _gameTowerData.atkSpd + _upgradeATK.addValue[1] + (_gameTowerData.atkSpd + _upgradeATK.addValue[1]) * researchResult.atkSpdAddRate * 0.01f;
            _atkRange = _gameTowerData.atkRange + _upgradeATK.addValue[2] + (_gameTowerData.atkRange + _upgradeATK.addValue[2]) * researchResult.atkRangeAddRate * 0.01f;
            _atkNumber = _gameTowerData.atkNumber + (int)_upgradeATK.addValue[3];
            _targetNumber = _gameTowerData.targetNumber + (int)_upgradeATK.addValue[4];
        }
        else
        {
            _atk = _gameTowerData.atk + (int)(_gameTowerData.atk * researchResult.atkAddRate * 0.01f);
            _atkSpd = _gameTowerData.atkSpd + _gameTowerData.atkSpd * researchResult.atkSpdAddRate * 0.01f;
            _atkRange = _gameTowerData.atkRange + _gameTowerData.atkRange * researchResult.atkRangeAddRate * 0.01f;
            _atkNumber = _gameTowerData.atkNumber;
            _targetNumber = _gameTowerData.targetNumber;
        }
        if (_upgradeDEF != null)
        {
            _maxHP = (int)(_gameTowerData.hp + _upgradeDEF.addValue[0] + (_gameTowerData.hp + _upgradeDEF.addValue[0]) * researchResult.hpAddRate * 0.01f);
            _def = (int)(_gameTowerData.def + _upgradeDEF.addValue[1] + (_gameTowerData.def + _upgradeDEF.addValue[1]) * researchResult.defAddRate * 0.01f);
            _chargeEP = (int)(_gameTowerData.ep + _upgradeDEF.addValue[2] + (_gameTowerData.ep + _upgradeDEF.addValue[2]) * researchResult.epAddRate * 0.01f);
        }
        else
        {
            _maxHP = _gameTowerData.hp + (int)(_gameTowerData.hp * researchResult.hpAddRate * 0.01f);
            _def = _gameTowerData.def + (int)(_gameTowerData.def * researchResult.defAddRate * 0.01f);
            _chargeEP = _gameTowerData.ep + (int)(_gameTowerData.ep * researchResult.epAddRate * 0.01f);
        }

        _spValue = new float[_gameTowerData.spValue.Length];
        for (int i = 0; i < _spValue.Length; i++)
        {
            if (_upgradeSP != null)
            {
                _spValue[i] = _gameTowerData.spValue[i] + _upgradeSP.addValue[i];
            }
            else
            {
                _spValue[i] = _gameTowerData.spValue[i];
            }
        }

        _rangeObject.transform.localScale = _rangeSize * _atkRange;
    }

    public int UpgradeCost(EUpgradeType upgradeType)
    {
        ResearchResult researchResult = ResearchManager.Instance.GameResearchData;
        int cost = 0;

        switch (upgradeType)
        {
            case EUpgradeType.Attack:
                if (_upgradeATK != null)
                {
                    cost = _upgradeATK.nextCost + (int)(_upgradeATK.nextCost * researchResult.towerCostReduceRate * 0.01f);
                }
                else
                {
                    cost = _gameTowerData.atkUpgradeCost;
                }
                break;
            case EUpgradeType.Defence:
                if (_upgradeDEF != null)
                {
                    cost = _upgradeDEF.nextCost + (int)(_upgradeDEF.nextCost * researchResult.towerCostReduceRate * 0.01f);
                }
                else
                {
                    cost = _gameTowerData.defUpgradeCost;
                }
                break;
            case EUpgradeType.Special:
                if (_upgradeSP != null)
                {
                    cost = _upgradeSP.nextCost + (int)(_upgradeSP.nextCost * researchResult.towerCostReduceRate * 0.01f);
                }
                else
                {
                    cost = _gameTowerData.spUpgradeCost;
                }
                break;
        }

        return cost;
    }

    public int TowerRepairCost()
    {
        int cost;
        float hpRate = 1 - (float)_hp / _maxHP;
        cost = (int)(hpRate * _totalCost * 0.3f);
        cost += (int)(cost * ResearchManager.Instance.GameResearchData.towerCostReduceRate * 0.01f);
        return cost;
    }

    public int TowerGetSellNumber()
    {
        int sellNumber = (int)(_totalCost * 0.5f);
        return sellNumber;
    }

    public void TotalCostAdd(int cost)
    {
        _totalCost += cost;
    }
    #endregion

}
