using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : TSingleton<LobbyManager>
{
    [SerializeField] GameObject _playerUI = null;
    [SerializeField] Text _mineralValueTxt = null;
    [SerializeField] SelectedMiniUI[] _selectedMiniUIs = null;
    [SerializeField] Transform _container = null;
    [SerializeField] Laboratory _laboratory = null;

    bool _loadEndCheck = true;

    private void Awake()
    {
        Instance = this;
        Init();
        _selectedMiniUIs = new SelectedMiniUI[_container.childCount];
        for (int i = 0; i < _container.childCount; i++)
        {
            _selectedMiniUIs[i] = _container.GetChild(i).GetComponent<SelectedMiniUI>();
        }
    }

    private void Start()
    {
        MineralUpdate();
        SelectedMiniUpdate();
    }

    private void Update()
    {
        if (_loadEndCheck)
        {
            if (SceneControlManager.Instance.NowLoaddingState == ELoaddingState.None)
            {
                _loadEndCheck = false;
                SoundManager.Instance.PlayBGMSound(ESoundBGM.Lobby);
            }
        }
    }

    public void MineralUpdate()
    {
        _mineralValueTxt.text = ResourceManager.Instance.SpaceMineralValue.ToString();
    }

    public void PlanetSelect(bool open)
    {
        _playerUI.SetActive(!open);
    }

    public void SelectedMiniUpdate()
    {
        List<ObjectData> playerSelectObjects = PlayerDataManager.Instance.SelectList;

        for (int i = 0; i < playerSelectObjects.Count; i++)
        {
            _selectedMiniUIs[i].SelectedSetting(ObjectDataManager.Instance.GetIcon(playerSelectObjects[i].objectName));
        }
        for (int i = playerSelectObjects.Count; i < PlayerDataManager.Instance.MaxSelected; i++)
        {
            _selectedMiniUIs[i].NoneSelectSetting();
        }
        for (int i = PlayerDataManager.Instance.MaxSelected; i < _selectedMiniUIs.Length; i++)
        {
            _selectedMiniUIs[i].LockSelectSetting();
        }
    }

    public void FacilityUpdate()
    {
        _laboratory.FacilityUpdate();
    }
}
