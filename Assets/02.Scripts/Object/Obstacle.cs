using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EAttackAble
{
    None,
    AttackAble,
    AttackDisable
}

public class Obstacle : ObjectGame
{
    public EObjectName _objectName = EObjectName.None;
    public EAttackAble _attackAble = EAttackAble.None;
    public ObstacleGameData _gameObstacleData;
    public int _sellGetCost;
    [SerializeField] HitField _hitPad = null;
    [SerializeField] int _durability;
    [SerializeField] int _reduceValue;
    [SerializeField] float[] _values;
    WorldStatusUI _statusUI = null;
    Tile _parentTile;
    IntVector2 _gridPosition;
    IntVector2 _dimensions;
    EFitType _fitType;

    private void Start()
    {
        _statusUI = ObjectDataManager.Instance.StatusInit();
        _gameObstacleData = ObjectDataManager.Instance.GetObstacleGameData(_objectName);
        DataSetting();
        ObjectDataManager.Instance.MarkerSetting(transform, _objectName);
    }

    public override void Hit(int damage, EWeakType weakType)
    {
        _durability -= damage;
        DurabilityCheck();
    }

    protected void DurabilityCheck()
    {
        if (_durability <= 0)
        {
            _durability = 0;
            DestroyObstacle();
        }
        _statusUI.HPChange(_durability);
    }

    void DataSetting()
    {
        _values = new float[_gameObstacleData.values.Length];
        for (int i = 0; i < _values.Length; i++)
        {
            _values[i] = _gameObstacleData.values[i] + _gameObstacleData.values[i] * ResearchManager.Instance.GameResearchData.valueIncreaseRate * 0.01f;
        }
        _hitPad.HitPadSetting(ETargetType.Enemy, _values);
        _durability = _gameObstacleData.durability + (int)(_gameObstacleData.durability * ResearchManager.Instance.GameResearchData.valueIncreaseRate * 0.01f);
        _reduceValue = _gameObstacleData.reduceValue - (int)(_gameObstacleData.reduceValue * ResearchManager.Instance.GameResearchData.valueIncreaseRate * 0.01f);
        _statusUI.StatusSetting(transform, _durability, 5, false);
        _sellGetCost = _gameObstacleData.buildCost / 5;
    }

    public void BuildingObstacle(Ghost ghost)
    {
        _parentTile = ghost._parentTile;
        _gridPosition = ghost._gridPos;
        _dimensions = ghost._demision;
        _fitType = ghost._fitType;

        Vector3 rotateEulerV = Vector3.zero;

        switch (ghost._rotateType)
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
        StartCoroutine(BuildSuccess());
    }

    IEnumerator BuildSuccess()
    {
        yield return new WaitForSeconds(0.5f);
        _objectSelectActive = true;
    }

    public void DestroyObstacle()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.SellObject, null);
        _parentTile.Clear(_gridPosition, _dimensions, _fitType);
        if (_objectSelect)
        {
            Select(false);
        }
        StopCoroutine(BuildSuccess());
        Destroy(_statusUI.gameObject);
        Destroy(gameObject);
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
            GameUI.Instance.ObstacleClick(this);
        }
        else
        {
            GameUI.Instance.ViewUIOff();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_attackAble == EAttackAble.AttackDisable)
        {
            if (other.CompareTag("Enemy"))
            {
                _durability -= _reduceValue;
                DurabilityCheck();
            }
        }
    }
}
