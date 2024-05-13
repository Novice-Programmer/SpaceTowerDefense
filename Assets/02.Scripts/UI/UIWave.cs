using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWave : MonoBehaviour
{
    [SerializeField] UIWaveEnemy _prefabWaveEnemy = null;
    [SerializeField] Transform _waveEnemyContainer = null;
    [SerializeField] Text _waveLiveNumberTxt = null;
    [SerializeField] Text _waveWaitNumberTxt = null;
    [SerializeField] Animator _waveWaitAnimator = null;

    int _enemyWaitNumber;
    int _enemyLiveNumber;

    Dictionary<int, List<EObjectName>> _waveEnemyList = new Dictionary<int, List<EObjectName>>();
    Dictionary<EObjectName, List<UIWaveEnemy>> _enemyTypeUIDic = new Dictionary<EObjectName, List<UIWaveEnemy>>();

    private void Start()
    {
        _waveWaitAnimator.SetBool("View", true);
    }

    public void NextWave(int wave)
    {
        _waveWaitAnimator.SetBool("View", true);
        WaveEnemyUISetting(wave);
    }

    public void StageEnemyUIInit(Wave[] waves)
    {
        Dictionary<EObjectName, int> enemy = new Dictionary<EObjectName, int>();
        for (int i = 0; i < waves.Length; i++)
        {
            Wave wave = waves[i];
            List<EObjectName> enemies = new List<EObjectName>();
            Dictionary<EObjectName, int> enemyCheck = new Dictionary<EObjectName, int>();
            for (int j = 0; j < wave._spawnDatas.Length; j++)
            {
                EObjectName enemyType = wave._spawnDatas[j]._enemyData.objectName;
                enemies.Add(enemyType);
                if (enemyCheck.ContainsKey(enemyType))
                {
                    enemyCheck[enemyType]++;
                }
                else
                {
                    enemyCheck.Add(enemyType, 1);
                }
            }
            foreach (EObjectName enemyType in enemyCheck.Keys)
            {
                if (enemy.ContainsKey(enemyType))
                {
                    if (enemy[enemyType] < enemyCheck[enemyType])
                    {
                        enemy[enemyType] = enemyCheck[enemyType];
                    }
                }
                else
                {
                    enemy.Add(enemyType, enemyCheck[enemyType]);
                }
            }

            _waveEnemyList.Add(i, enemies);
        }

        int enemyNumber = 0;


        foreach (EObjectName enemyType in enemy.Keys)
        {
            enemyNumber += enemy[enemyType];
        }

        foreach (EObjectName enemyType in enemy.Keys)
        {
            List<UIWaveEnemy> waveEnemyUIList = new List<UIWaveEnemy>();
            for (int i = 0; i < enemy[enemyType]; i++)
            {
                UIWaveEnemy waveEnemyUI = Instantiate(_prefabWaveEnemy, _waveEnemyContainer);
                waveEnemyUI.WaveEnemyInfo(ObjectDataManager.Instance.GetImage(enemyType), ObjectDataManager.Instance.GetImage(enemyType, true));
                waveEnemyUIList.Add(waveEnemyUI);
                waveEnemyUI.gameObject.SetActive(false);
            }
            _enemyTypeUIDic.Add(enemyType, waveEnemyUIList);
        }
        WaveEnemyUISetting(0);
    }

    void WaveEnemyUISetting(int wave)
    {
        _enemyWaitNumber = _waveEnemyList[wave].Count;
        _enemyLiveNumber = _waveEnemyList[wave].Count;
        _waveWaitNumberTxt.text = _enemyWaitNumber.ToString();
        _waveLiveNumberTxt.text = _enemyLiveNumber.ToString();
        _waveLiveNumberTxt.gameObject.SetActive(false);
        for (int i = 0; i < _waveEnemyList[wave].Count; i++)
        {
            WaveEnemyView(_waveEnemyList[wave][i]);
        }
    }

    void WaveEnemyView(EObjectName objectName)
    {
        List<UIWaveEnemy> typeEnemy = _enemyTypeUIDic[objectName];
        for (int i = 0; i < typeEnemy.Count; i++)
        {
            if (!typeEnemy[i].gameObject.activeSelf)
            {
                typeEnemy[i].gameObject.SetActive(true);
                break;
            }
        }
    }

    public void WaveEnemyAppearance(EObjectName objectName)
    {
        List<UIWaveEnemy> typeEnemy = _enemyTypeUIDic[objectName];
        for (int i = 0; i < typeEnemy.Count; i++)
        {
            if (typeEnemy[i].gameObject.activeSelf)
            {
                typeEnemy[i].gameObject.SetActive(false);
                _enemyWaitNumber--;
                _waveWaitNumberTxt.text = _enemyWaitNumber.ToString();
                break;
            }
        }
        if (_enemyWaitNumber <= 0)
        {
            _waveWaitAnimator.SetBool("View", false);
        }
    }

    public void WaveEnemyDie()
    {
        _enemyLiveNumber--;
        _waveLiveNumberTxt.text = _enemyLiveNumber.ToString();
    }

    public void WaveStart()
    {
        _waveLiveNumberTxt.gameObject.SetActive(true);
    }
}
