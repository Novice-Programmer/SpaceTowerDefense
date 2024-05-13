using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ESceneType
{
    Start,
    Lobby,
    Ingame,
}

public enum ELoaddingState
{
    None = 0,
    LoadSceneStart,
    LoadingScene,
    LoadSceneEnd,
    LoadStageStart,
    LoadingStage,
    LoadStageEnd,
    LoadEnd
}

public class SceneControlManager : TSingleton<SceneControlManager>
{
    public ESceneType SceneType { get { return _nowSceneType; } }

    GameObject _prefabLoading;
    Loading _wnd;

    ESceneType _nowSceneType = ESceneType.Start;
    ESceneType _oldSceneType = ESceneType.Start;

    ELoaddingState _currentStateLoad;

    public ELoaddingState NowLoaddingState
    {
        get { return _currentStateLoad; }
    }

    private void Awake()
    {
        Init();
        Instance = this;
        _prefabLoading = Resources.Load("UI/Loading") as GameObject;
    }

    public void SceneChange(ESceneType sceneType)
    {
        _oldSceneType = _nowSceneType;
        _nowSceneType = sceneType;
        StartCoroutine(LoadingScene());
    }

    IEnumerator LoadingScene()
    {
        _wnd = Instantiate(_prefabLoading, transform).GetComponent<Loading>();
        _wnd.OpenLoaddingWnd(_nowSceneType);

        AsyncOperation aOper;
        string sceneName = _nowSceneType.ToString() + "Scene";

        _currentStateLoad = ELoaddingState.LoadSceneStart;
        aOper = SceneManager.LoadSceneAsync(sceneName);
        while (!aOper.isDone)
        {
            _currentStateLoad = ELoaddingState.LoadingScene;
            _wnd.SettingLoadRate(aOper.progress);
            yield return null;
        }
        _wnd.SettingLoadRate(1);
        _currentStateLoad = ELoaddingState.LoadSceneEnd;

        /*
        if (_nowSceneType == ESceneType.Ingame)
        {
            string stageName = "Stage";
            _currentStateLoad = ELoaddingState.LoadSceneStart;
            sceneName = stageName;
            aOper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!aOper.isDone)
            {
                _currentStateLoad = ELoaddingState.LoadingStage;
                _wnd.SettingLoadRate(aOper.progress);
                yield return null;
            }
            Scene aScene = SceneManager.GetSceneByName(sceneName);
            _currentStateLoad = ELoaddingState.LoadStageEnd;
            _wnd.SettingLoadRate(1);
            SceneManager.SetActiveScene(aScene);
        }
        */

        _currentStateLoad = ELoaddingState.LoadEnd;
        SoundManager.Instance.PlayEffectSound(ESoundName.LoadEnd, null);
        yield return new WaitForSeconds(3.0f);

        _currentStateLoad = ELoaddingState.None;
        Destroy(_wnd.gameObject);
    }
}
