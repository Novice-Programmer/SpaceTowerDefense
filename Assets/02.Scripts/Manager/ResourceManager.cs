using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPaymentType
{
    Initial,
    Regular,
    Occasional
}

public class ResourceManager : TSingleton<ResourceManager>
{
    int _ingameValue = 0;
    int _mineralValue = 0;
    public int TowerPartValue { get { return _ingameValue; } set { _ingameValue += value; ValueChange(true, value); } }
    public int SpaceMineralValue { get { return _mineralValue; } set { _mineralValue += value; ValueChange(false, value); } }

    [SerializeField] StageResourceData[] _stageResourceDatas = null;

    StageResourceData _stageData;

    private void Awake()
    {
        Init();
        Instance = this;
    }

    private void Start()
    {
        _mineralValue = 6000;
    }

    public int MaxOccPlayment()
    {
        return _stageData.occasionalNumber + (int)(_stageData.occasionalNumber * ResearchManager.Instance.GameResearchData.occasionalAddRate * 0.01f);
    }

    public float ReduceOccTime()
    {
        return 10 * ResearchManager.Instance.GameResearchData.occasionalReduceTime * 0.01f;
    }

    public void GameResourceSetting()
    {
        _stageData = _stageResourceDatas[StageManager.Instance.NowStage];
    }

    public void TowerPartPayment(EPaymentType paymentType, int wave)
    {
        switch (paymentType)
        {
            case EPaymentType.Initial:
                TowerPartValue = _stageData.initialTP + (int)(_stageData.initialTP * 0.01f * ResearchManager.Instance.GameResearchData.towerPartAddRate);
                break;
            case EPaymentType.Regular:
                TowerPartValue = _stageData.regularTP + (int)((_stageData.regularTP + wave * _stageData.waveAddTP) * 0.01f * ResearchManager.Instance.GameResearchData.towerPartAddRate);
                break;
            case EPaymentType.Occasional:
                int addValue = _stageData.occasionalTP + (int)(_stageData.occasionalTP * 0.01f * ResearchManager.Instance.GameResearchData.towerPartAddRate);
                addValue += (int)(wave * _stageData.occasionalTP * 0.15f);
                addValue += (int)(wave * _stageData.occasionalTP * 0.005f * ResearchManager.Instance.GameResearchData.towerPartAddRate);
                TowerPartValue = addValue;
                break;
        }
    }

    public void WaveClear(int wave)
    {
        if (wave == 0)
        {
            SpaceMineralValue = _stageData.basicClearMineral + (int)(_stageData.basicClearMineral * 0.01f * ResearchManager.Instance.GameResearchData.mineralAddRate);
        }
        else if (wave == _stageData.maxWave)
        {
            if (_stageData.stage > PlayerDataManager.Instance.ClearStage)
            {
                PlayerDataManager.Instance.StageClear(_stageData.stage);
                SpaceMineralValue = _stageData.firstAllClearMineral;
            }
            SpaceMineralValue = _stageData.stageClearMineral + (int)(_stageData.stageClearMineral * 0.01f * ResearchManager.Instance.GameResearchData.mineralAddRate);
        }
        SpaceMineralValue = _stageData.waveClearMineral + (int)(_stageData.waveClearMineral * 0.01f * ResearchManager.Instance.GameResearchData.mineralAddRate);
    }

    public void GameEnd()
    {
        int addValue = (int)(_ingameValue * 0.1f);
        _mineralValue += addValue + (int)(addValue * 0.01f * ResearchManager.Instance.GameResearchData.mineralAddRate);
        _ingameValue = 0;
    }

    void ValueChange(bool gameValueCheck, int value)
    {
        if (SceneControlManager.Instance.SceneType == ESceneType.Ingame)
        {
            GameUI.Instance.ResourceValueChange(gameValueCheck, value);
        }
        else
        {
            LobbyManager.Instance.MineralUpdate();
        }
    }
}
