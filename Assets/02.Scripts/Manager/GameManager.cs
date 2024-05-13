using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }
    int _wave = 0;
    int _getPartNumber = 0;
    int _maxOcc = 0;
    float _timeCheck = 0;
    float _occTime = 10f;
    float _startWaveTime = 15f;
    float _reduceTime = 0;
    bool _waveStart = false;
    bool _gameEnd = false;

    public int WaveNumber { get { return _wave; } }
    public bool GameEndCheck { get { return _gameEnd; } }

    List<GameObject> _installObjects = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameUI.Instance.GameUISetting();
        ResourceManager.Instance.GameResourceSetting();
        ResourceManager.Instance.TowerPartPayment(EPaymentType.Initial, _wave);
        _maxOcc = ResourceManager.Instance.MaxOccPlayment();
        _reduceTime = ResourceManager.Instance.ReduceOccTime();
        SoundManager.Instance.PlayBGMSound(ESoundBGM.Mars);
    }

    private void Update()
    {
        if (_gameEnd)
            return;
        if (SceneControlManager.Instance.NowLoaddingState == ELoaddingState.None)
        {
            _timeCheck += Time.deltaTime;
            if (_waveStart)
            {
                if (_timeCheck >= _occTime + _reduceTime)
                {
                    _timeCheck = 0;
                    _getPartNumber++;
                    if (_getPartNumber <= _maxOcc)
                    {
                        ResourceManager.Instance.TowerPartPayment(EPaymentType.Occasional, _wave);
                        SoundManager.Instance.PlayEffectSound(ESoundName.MoneyAdd, null);
                    }
                }
            }
            else
            {
                GameUI.Instance.TimeView((int)(_startWaveTime - _timeCheck));
                if (_timeCheck >= 15.0f)
                {
                    _timeCheck = 0;
                    WaveStart();
                }
            }
        }
    }

    void WaveStart()
    {
        _timeCheck = 0;
        _waveStart = true;
        SoundManager.Instance.PlayEffectSound(ESoundName.WaveStart, null);
        WaveManager.Instance.WaveStart(_wave);
        GameUI.Instance.WaveStart(_wave);
    }

    public void WaveEnd(bool stageClear)
    {
        _timeCheck = 0;
        _waveStart = false;
        _getPartNumber = 0;
        _wave++;
        SoundManager.Instance.PlayEffectSound(ESoundName.WaveEnd, null);
        ResourceManager.Instance.WaveClear(_wave);
        if (stageClear)
        {
            GameEnd(true);
        }
        else
        {
            ResourceManager.Instance.TowerPartPayment(EPaymentType.Regular, _wave);
            GameUI.Instance.WaveClear(_wave);
        }
    }

    public void Install(EObjectType objectType, EObjectName objectName, int installCost)
    {
        InputManager.Instance.Install(objectType, objectName, installCost);
    }

    public void TowerBuild(Ghost ghostData)
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.InstallObject, ghostData.transform);
        ResourceManager.Instance.TowerPartValue = -ghostData._installCost;
        Tower tower = Instantiate(ObjectDataManager.Instance.GetTower(ghostData._objectName), ghostData._fitPos, Quaternion.identity);
        tower.BuildingTower(ghostData);
        _installObjects.Add(tower.gameObject);
    }

    public void ObstacleBuild(Ghost ghostData)
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.InstallObject, ghostData.transform);
        ResourceManager.Instance.TowerPartValue = -ghostData._installCost;
        Obstacle obstacle = Instantiate(ObjectDataManager.Instance.GetObstacle(ghostData._objectName), ghostData._fitPos, Quaternion.identity);
        obstacle.BuildingObstacle(ghostData);
        _installObjects.Add(obstacle.gameObject);
    }

    public void GameEnd(bool clear)
    {
        _gameEnd = true;
        if (clear)
        {
            GameObject[] minerals = GameObject.FindGameObjectsWithTag("Mineral");
            for (int i = 0; i < minerals.Length; i++)
            {
                if (minerals[i].activeSelf)
                {
                    minerals[i].GetComponent<DropMineral>().GetMineral();
                }
            }
        }
        PoolManager.Instance.GameEnd();
        GameUI.Instance.GameEnd(clear);
        ResourceManager.Instance.GameEnd();
        for (int i = 0; i < _installObjects.Count; i++)
        {
            if (_installObjects[i] != null)
            {
                _installObjects[i].SetActive(false);
            }
        }
    }
}
