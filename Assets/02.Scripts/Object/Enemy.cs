using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ERatingType
{
    None,
    Normal,
    SpecialEnemy,
    MineralEnemy,
    Boss
}

public abstract class Enemy : ObjectGame
{
    [Header("Enemy")]
    public EnemyData _enemyData;
    public EObjectName _objectName;
    public ERatingType _rating = ERatingType.Normal;
    protected int _wavePointIndex = 0;
    protected bool _action = false;
    protected bool _isDead = false;

    protected EWeakType _weakType;
    [SerializeField] protected int _hp;
    [SerializeField] protected int _mp;
    [SerializeField] protected int _atk; // 공격력
    [SerializeField] protected int _def; // 방어력
    [SerializeField] protected float _atkSpd; // 공격 속도
    [SerializeField] protected float _movSpd; // 이동 속도
    [SerializeField] protected float _moveCheckSize; // 이동 확인 범위
    [SerializeField] NavMeshObstacle _enemyObstacle = null;

    [SerializeField] List<BadBuff> _badBuffs;

    NavMeshAgent _enemyAI;
    Animator _enemyAnim;
    WorldStatusUI _statusUI = null;
    BoxCollider _boxCollider;
    public Transform _target;
    EObjectType _prevTarget;
    bool _searchFail = true;

    [SerializeField] float _obstacleCheckRange = 2.0f;
    [SerializeField] float _noAttackTime = 4.0f;

    string _towerTag = "Tower";
    string _obstacleTag = "Obstacle";
    float _timeCheck = 0;
    float _attackTime = 0;
    int _attackNumber = 0;

    [SerializeField] State _state = null;
    [SerializeField] protected ESoundName _attackSound = ESoundName.TowerHit;
    [SerializeField] ESoundName _dieSound = ESoundName.NMDADie;
    private void Awake()
    {
        _enemyAI = GetComponent<NavMeshAgent>();
        _enemyAnim = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Active()
    {
        gameObject.SetActive(true);
        _enemyAI.enabled = true;
        _enemyObstacle.enabled = false;
        _boxCollider.enabled = true;
        _target = null;
        _isDead = false;
        _wavePointIndex = 0;
        _enemyAI.destination = WayPointContainer._wayPoints[_wavePointIndex].position;
        _badBuffs = new List<BadBuff>();
        _atk = _enemyData.atk;
        _def = _enemyData.def;
        _atkSpd = _enemyData.atkSpd;
        _movSpd = _enemyData.movSpd;
        _enemyAI.speed = _movSpd;
        _enemyAI.acceleration = _movSpd * 2.0f;
        StateChange(EStateType.Move);
        _action = false;
        _timeCheck = 0;
        _attackTime = 0;
        _attackNumber = 0;
        _attackPos.gameObject.SetActive(true);
        _hp = _enemyData.hp;
        _mp = 0;
        GameObject go = PoolManager.Instance.PoolGetAvailableObject("StatusUI");
        _statusUI = go.GetComponent<WorldStatusUI>();
        _statusUI.StatusSetting(transform, _enemyData.hp, 4f);
        _statusUI.MPChange(_mp);
        _objectName = _enemyData.objectName;
        StartCoroutine(ActiveSuccess());
        StartCoroutine(BadBuffCheck());
        ObjectDataManager.Instance.MarkerSetting(transform, _objectName);
    }
    IEnumerator ActiveSuccess()
    {
        yield return new WaitForSeconds(0.5f);
        _objectSelectActive = true;
    }

    public void Disactive()
    {
        WaveManager.Instance.WaveEnemyDie();
        StopAllCoroutines();
        _statusUI._available = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_action)
        {
            return;
        }
        else
        {
            _timeCheck += Time.deltaTime;
            switch (_stateType)
            {
                case EStateType.AttackSearch:
                    AttackSearch();
                    break;
                case EStateType.Attack:
                    Attack();
                    break;
                case EStateType.Move:
                    Move();
                    break;
                case EStateType.AttackCommander:
                    AttackCommander();
                    break;
            }
        }
    }

    void StateChange(EStateType stateType)
    {
        _stateType = stateType;
        _state.StateUpdate(_stateType);
    }

    IEnumerator BadBuffCheck()
    {
        while (!_isDead)
        {
            if (_badBuffs.Count > 0)
            {
                StatusCheck();
            }
            yield return null;
        }
    }

    void StatusCheck()
    {
        if (_badBuffs.Count > 0)
        {
            int atkBadValue = 0;
            int defBadValue = 0;
            float atkSpdBadValue = 0;
            float movSpdBadValue = 0;

            for (int i = 0; i < _badBuffs.Count; i++)
            {
                if (_badBuffs[i] == null)
                {
                    _badBuffs.RemoveAt(i);
                    break;
                }
                atkBadValue += _badBuffs[i].atk;
                defBadValue += _badBuffs[i].def;
                atkSpdBadValue += _badBuffs[i].atkSpd;
                movSpdBadValue += _badBuffs[i].movSpd;
            }
            _atk = _enemyData.atk - (int)(_enemyData.atk * atkBadValue * 0.01f);
            if (_atk <= 0)
            {
                _atk = 1;
            }
            _def = _enemyData.def - (int)(_enemyData.def * defBadValue * 0.01f);
            _atkSpd = _enemyData.atkSpd - _enemyData.atkSpd * atkSpdBadValue * 0.01f;
            if (_atkSpd < 0.01f)
            {
                _atkSpd = 0.01f;
            }
            _movSpd = _enemyData.movSpd - _enemyData.movSpd * movSpdBadValue * 0.01f;
            if (_movSpd < 0.1f)
            {
                _movSpd = 0.1f;
            }
        }
        else
        {
            _atk = _enemyData.atk;
            _def = _enemyData.def;
            _atkSpd = _enemyData.atkSpd;
            _movSpd = _enemyData.movSpd;
        }

        if (_enemyAI.speed > 0)
            _enemyAI.speed = _movSpd;
        _enemyAI.acceleration = _movSpd * 2.0f;
    }

    #region 액션

    void Move()
    {
        _attackNumber = 0;
        _enemyAI.speed = _movSpd;
        _enemyAnim.SetBool("Move", true);
        _enemyObstacle.enabled = false;
        if (_timeCheck >= _enemyData.checkTime)
        {
            _timeCheck = 0;
            float checkPer = Random.Range(0.0f, 100.0f);
            if (checkPer <= _enemyData.atkRate)
            {
                _searchFail = true;
                StateChange(EStateType.AttackSearch);
                return;
            }
        }
        ViewObstacle();
    }

    void ViewObstacle()
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag(_obstacleTag);
        float shortestDistance = _obstacleCheckRange;
        GameObject nearestObstacle = null;
        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle.GetComponent<Obstacle>()._attackAble == EAttackAble.AttackDisable)
                continue;
            float distanceToObstacle = Vector3.Distance(transform.position, obstacle.transform.position);
            if (distanceToObstacle < shortestDistance)
            {
                shortestDistance = distanceToObstacle;
                nearestObstacle = obstacle;
            }
        }

        if (nearestObstacle != null)
        {
            _prevTarget = EObjectType.Obstacle;
            StateChange(EStateType.Attack);
            _attackTime = 0;
            _timeCheck = 0;
            _target = nearestObstacle.transform;
        }
        else
        {
            _target = null;
        }
    }

    void AttackSearch()
    {
        _enemyAI.speed = _movSpd;
        _enemyAnim.SetBool("Move", true);
        _enemyObstacle.enabled = false;
        if (_timeCheck >= _enemyData.atkTime)
        {
            _timeCheck = 0;
            if (_searchFail)
                GetNextWayPoint(_wavePointIndex);
            StateChange(EStateType.Move);
            return;
        }

        GameObject[] towers = GameObject.FindGameObjectsWithTag(_towerTag);
        float shortestDistance = _enemyData.sightRange;
        GameObject nearestTower = null;
        foreach (GameObject tower in towers)
        {
            if (tower.GetComponent<Tower>()._stateType == EStateType.Breakdown)
                continue;
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower;
            }
        }

        if (nearestTower != null)
        {
            _prevTarget = EObjectType.Tower;
            StateChange(EStateType.Attack);
            _searchFail = false;
            _attackTime = 0;
            _target = nearestTower.GetComponent<Tower>().transform;
            _enemyAI.destination = nearestTower.transform.position;
        }
        else
        {
            _target = null;
        }

        if (_target == null)
            ViewObstacle();
    }

    void Attack()
    {
        _enemyAI.enabled = true;

        if (_target != null)
        {
            if (_target.GetComponent<ObjectGame>()._objectType == EObjectType.Tower)
            {
                if (_timeCheck >= _noAttackTime && _attackNumber == 0)
                {
                    _timeCheck = 0;
                    GetNextWayPoint(_wavePointIndex);
                    StateChange(EStateType.Move);
                    return;
                }

                if (_target.GetComponent<Tower>()._stateType == EStateType.Breakdown)
                {
                    _target = null;
                    GetNextWayPoint(_wavePointIndex);
                    StateChange(EStateType.Attack);
                    return;
                }
            }
        }

        else
        {
            if (_prevTarget != EObjectType.Obstacle)
            {
                GetNextWayPoint(_wavePointIndex);
                StateChange(EStateType.AttackSearch);
                return;
            }
            else
            {
                _timeCheck = 0;
                GetNextWayPoint(_wavePointIndex);
                StateChange(EStateType.Move);
                return;
            }
        }

        if (_timeCheck >= _enemyData.atkTime)
        {
            _timeCheck = 0;
            GetNextWayPoint(_wavePointIndex);
            StateChange(EStateType.Move);
            return;
        }

        AttackRangeCheck(_target.GetComponent<ObjectGame>()._objectType);
    }

    void AttackCommander()
    {
        AttackRangeCheck(_target.GetComponent<ObjectGame>()._objectType);
    }

    void AttackRangeCheck(EObjectType objectType)
    {
        float addRange;
        if (objectType == EObjectType.Commander)
        {
            addRange = 1.0f;
        }
        else
        {
            addRange = 0.0f;
        }

        if (Vector3.Distance(transform.position, _target.position) < _enemyData.atkRange + addRange)
        {
            _enemyAI.speed = 0;
            _enemyAnim.SetBool("Move", false);
            _enemyAI.enabled = false;
            _enemyObstacle.enabled = true;
            Vector3 yOutTargetPos = new Vector3(_target.position.x, 0, _target.position.z);
            Vector3 yOutTransformPos = new Vector3(transform.position.x, 0, transform.position.z);
            Quaternion lookRotation = Quaternion.LookRotation(yOutTargetPos - yOutTransformPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            if (_attackTime <= 0.0f)
            {
                if (Quaternion.Angle(transform.rotation, lookRotation) <= 5.0f)
                {
                    _attackTime = 9999;
                    _attackNumber++;
                    _action = true;
                    if (_mp < 100)
                    {
                        _enemyAnim.SetTrigger("Attack");
                    }
                    else
                    {
                        _enemyAnim.SetTrigger("Skill");
                    }
                }
            }
        }
        else
        {
            _enemyAI.speed = _movSpd;
            _enemyAnim.SetBool("Move", true);
            _enemyObstacle.enabled = false;
        }

        _attackTime -= Time.deltaTime;
    }

    public virtual void TargetAttack()
    {
    }

    public virtual void AttackEnd()
    {
        _mp += _enemyData.mp;
        _statusUI.MPChange(_mp);
        _attackTime = 1 / _atkSpd;
        _action = false;
    }

    public virtual void TargetSpecialAttack()
    {
    }

    public virtual void SkillEnd()
    {
        _mp = 0;
        _statusUI.MPChange(_mp);
        _attackTime = 1 / _atkSpd;
        _action = false;
    }

    void Die()
    {
        StateChange(EStateType.Die);
        _action = true;
        _isDead = true;
        _enemyAI.enabled = false;
        _enemyObstacle.enabled = false;
        _boxCollider.enabled = false;
        _attackPos.gameObject.SetActive(false);
        _objectSelectActive = false;
        if (_objectSelect)
        {
            InputManager.Instance.ObjectSelectClose();
        }
        for (int i = 0; i < _badBuffs.Count; i++)
        {
            if (_badBuffs[i] != null)
            {
                _badBuffs[i].TargetDisable();
            }
        }
        SoundManager.Instance.PlayEffectSound(_dieSound, transform);
        if (gameObject.activeSelf)
        {
            StopCoroutine(BadBuffCheck());
            _enemyAnim.SetTrigger("Die");
        }
    }

    public void DieEnd()
    {
        float rate = Random.Range(0.0f, 100.0f);
        if (rate < _enemyData.mineralGetRate)
        {
            GameObject mineralObj = PoolManager.Instance.PoolGetObject("Mineral");
            DropMineral mineral = mineralObj.GetComponent<DropMineral>();
            mineral.MineralDrop(Random.Range(_enemyData.minMineral, _enemyData.maxMineral), transform.position, transform.right);
        }
        Disactive();
    }

    #endregion

    public void GetNextWayPoint(int wayPointNumber)
    {
        _wavePointIndex = wayPointNumber;
        _wavePointIndex++;
        if (_wavePointIndex >= WayPointContainer._wayPoints.Length)
        {
            StateChange(EStateType.AttackCommander);
            _target = GameObject.FindGameObjectWithTag("Commander").transform;
            _enemyAI.destination = _target.transform.position;
            return;
        }
        _enemyAI.destination = WayPointContainer._wayPoints[_wavePointIndex].position;
    }

    public override void Hit(int damage, EWeakType weakType)
    {
        if (_stateType == EStateType.Die)
            return;
        if (weakType != EWeakType.None && weakType == _weakType)
        {
            damage += (int)(damage * 0.2f);
        }
        int resultDamage = damage - _def;
        if (resultDamage <= 0)
        {
            resultDamage = 1;
        }
        _hp -= resultDamage;
        if (_hp <= 0)
        {
            _hp = 0;
        }
        _statusUI.HPChange(_hp);
        if (_hp == 0)
        {
            Die();
        }
    }

    public override void BadBuff(BadBuff badBuff)
    {
        _badBuffs.Add(badBuff);
    }

    public override bool BuffCheck(EBadBuff buffType)
    {
        for (int i = 0; i < _badBuffs.Count; i++)
        {
            if (_badBuffs[i].type == buffType)
                return false;
        }
        return true;
    }

    public override void BadBuffUpdate(BadBuff badBuff)
    {
        for (int i = 0; i < _badBuffs.Count; i++)
        {
            if (_badBuffs[i].type == badBuff.type)
            {
                _badBuffs[i].hp = badBuff.hp;
                _badBuffs[i].mp = badBuff.mp;
                _badBuffs[i].atk = badBuff.atk;
                _badBuffs[i].def = badBuff.def;
                _badBuffs[i].atkSpd = badBuff.atkSpd;
                _badBuffs[i].movSpd = badBuff.movSpd;
                _badBuffs[i].dotTime = badBuff.dotTime;
                _badBuffs[i].checkTime = badBuff.checkTime;
                break;
            }
        }
    }

    public override void ReduceMP(int mp)
    {
        _mp -= mp;
        if (_mp < 0)
        {
            _mp = 0;
        }
        _statusUI.MPChange(_mp);
    }

    public override void Select(bool selectOff = true)
    {
        base.Select(selectOff);
        _objectSelect = !_objectSelect;
        if (!_objectSelectActive)
        {
            _objectSelect = false;
        }
        _statusUI.SelectViewStatus(_objectSelect);
        if (_objectSelect)
        {
            GameUI.Instance.EnemyClick(this);
        }
        else
        {
            GameUI.Instance.ViewUIOff();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _enemyData.sightRange);
    }
}
