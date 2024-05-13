using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : TSingleton<WaveManager>
{
    [SerializeField] Wave[] _waves = null;
    public Transform _startPoint = null;
 
    int _waveNumber = 0;
    int _nowWaveEnemyNumber = 0;
    bool _stageClearCheck = false;

    public int StageMaxWave { get { return _waves.Length; } }

    private void Awake()
    {
        _dontDestroy = false;
        Init();
        Instance = this;
    }

    private void Start()
    {
        PoolManager.Instance.WaveEnemyInit(_waves);
        int maxEnemyNumber = _waves[0]._spawnDatas.Length;
        for(int i = 0; i < _waves.Length; i++)
        {
            if (maxEnemyNumber < _waves[i]._spawnDatas.Length)
            {
                maxEnemyNumber = _waves[i]._spawnDatas.Length;
            }
        }
        PoolManager.Instance.MineralInit(maxEnemyNumber);
        PoolManager.Instance.StatusUIInit(maxEnemyNumber);
        GameUI.Instance.StageUIInit(_waves);
    }

    public void WaveStart(int waveNumber)
    {
        _waveNumber = waveNumber;
        _nowWaveEnemyNumber = _waves[waveNumber]._spawnDatas.Length;
        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        Wave nowWave = _waves[_waveNumber];
        yield return new WaitForSeconds(nowWave._waveDelay);
        for (int i = 0; i < nowWave._spawnDatas.Length; i++)
        {
            yield return new WaitForSeconds(nowWave._spawnDatas[i]._delayTime);
            GameObject enemy = PoolManager.Instance.PoolGetObject(nowWave._spawnDatas[i]._enemyData.fileName);
            enemy.transform.position = _startPoint.position;
            enemy.GetComponent<Enemy>().Active();
            GameUI.Instance.AppearanceEnemy(nowWave._spawnDatas[i]._enemyData.objectName);
        }
        yield return null;

    }

    public void WaveEnemyDie()
    {
        GameUI.Instance.EnemyDie();
        _nowWaveEnemyNumber--;
        if (_nowWaveEnemyNumber == 0)
        {
            WaveClear();
        }
    }

    void WaveClear()
    {
        _stageClearCheck = _waveNumber >= _waves.Length - 1;
        GameManager.Instance.WaveEnd(_stageClearCheck);
    }
}
