using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EUIState
{
    Normal,
    Building,
    Paused,
    GameOver,
    BuildingWithDrag
}

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { set; get; }
    public struct UIPointer
    {
        public PointerInfo pointer;
        public Ray ray;
        public RaycastHit? raycast;
        public bool overUI;
    }

    public EUIState UIState { get; private set; }

    [SerializeField] UIResource _uiResource = null;
    [SerializeField] UIInfo _uiInfo = null;
    [SerializeField] UIMap _uiMap = null;
    [SerializeField] UIWave _uiWave = null;
    [SerializeField] UIPlayer _uiPlayer = null;
    [SerializeField] UIResultWindow _uiresultWnd = null;
    [SerializeField] Text _startTimeTxt = null;
    [SerializeField] float _hideTime = 3.0f;

    int _enemyDie = 0;

    CanvasGroup _hideCanvas;

    private void Awake()
    {
        Instance = this;
        _hideCanvas = _startTimeTxt.gameObject.GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_hideCanvas.alpha > 0)
        {
            _hideCanvas.alpha -= Time.deltaTime / _hideTime;
        }
    }

    public void GameUISetting()
    {
        _uiInfo.InstallButtonSetting();
    }

    public void ViewUIOff()
    {
        _uiInfo.ViewOff();
    }

    public void ResourceValueChange(bool gameValue, int value)
    {
        if (!GameManager.Instance.GameEndCheck)
        {
            _uiInfo.UIValueChange();
            _uiResource.UIValueChange(gameValue, value);
        }
    }

    public void SelectHPValueChange()
    {
        _uiInfo.RepairCheck();
    }

    public void TowerClick(Tower tower)
    {
        _uiInfo.ClickTower(tower);
        InputManager.Instance.UITouch();
    }

    public void ObstacleClick(Obstacle obstacle)
    {
        _uiInfo.ClickObstacle(obstacle);
        InputManager.Instance.UITouch();
    }

    public void EnemyClick(Enemy enemy)
    {
        _uiInfo.ClickEnemy(enemy);
        InputManager.Instance.UITouch();
    }

    public void StageUIInit(Wave[] waves)
    {
        _uiWave.StageEnemyUIInit(waves);
    }

    public void WaveUISetting(int wave)
    {
        _uiWave.NextWave(wave);
    }

    public void AppearanceEnemy(EObjectName objectName)
    {
        if (ObjectDataManager.Instance.EnemyRating(objectName) == ERatingType.Boss)
        {
            _hideCanvas.alpha = 1f;
            SoundManager.Instance.PlayEffectSound(ESoundName.BossAper, null);
            _startTimeTxt.text = "보스 몬스터 [" + ObjectDataManager.Instance.EnemyName(objectName) + "]가 등장했습니다!!!";
        }
        _uiWave.WaveEnemyAppearance(objectName);
    }

    public void WaveClear(int wave)
    {
        _uiWave.NextWave(wave);
        _uiMap.WaveSetting(wave);
    }

    public void GameEnd(bool clear)
    {
        _uiresultWnd.gameObject.SetActive(true);
        _uiresultWnd.GameEnd(clear, GameManager.Instance.WaveNumber, _enemyDie);
    }

    public void CommanderSetting(int maxHP)
    {
        _uiPlayer.HPSetting(maxHP);
    }

    public void CommanderHit(int hp)
    {
        _uiPlayer.ChangeHP(hp);
    }

    public void EnemyDie()
    {
        _enemyDie++;
        _uiWave.WaveEnemyDie();
    }

    public void TimeView(int time)
    {
        _hideCanvas.alpha = 1f;
        if (time > 5)
        {
            _startTimeTxt.color = Color.white;
        }
        else
        {
            _startTimeTxt.color = Color.red;
        }
        if (time > 2)
            _startTimeTxt.text = time + "초 뒤 적들이 입구로 도달합니다.";
        else
            _startTimeTxt.text = "적들이 입구 근처에 있습니다.";
    }

    public void WaveStart(int wave)
    {
        _hideCanvas.alpha = 1f;
        _startTimeTxt.text = "적들이 입구에서 몰려옵니다.";
        _uiMap.WaveSetting(wave);
        _uiWave.WaveStart();
    }
}
