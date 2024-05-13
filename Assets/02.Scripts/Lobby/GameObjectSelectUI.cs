using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectSelectUI : SelectUI
{
    [Header("ObjectList")]
    [SerializeField] Button[] _tabBtns = null;
    [SerializeField] GameObject[] _selectBorders = null;
    [SerializeField] SelectObjectUI[] _selectObjectUIs = null;
    [SerializeField] GameObject _selectContainer = null;
    [Header("Selected")]
    [SerializeField] SelectedObjectUI[] _selectedObjectUIs = null;
    [SerializeField] GameObject _selectedContainer = null;
    [Header("SelectDataView")]
    [SerializeField] Text _objectNameTxt = null;
    [SerializeField] Text _objectDesTxt = null;

    List<TowerGameData> _towerGameDatas;
    List<ObstacleGameData> _obstacleGameDatas;
    EObjectType _nowType = EObjectType.Tower;

    private void Awake()
    {
        _selectedObjectUIs = new SelectedObjectUI[_selectedContainer.transform.childCount];
        for (int i = 0; i < _selectedContainer.transform.childCount; i++)
        {
            _selectedObjectUIs[i] = _selectedContainer.transform.GetChild(i).GetComponent<SelectedObjectUI>();
            _selectedObjectUIs[i].ParentSetting(this);
        }

        _selectObjectUIs = new SelectObjectUI[_selectContainer.transform.childCount];
        for (int i = 0; i < _selectContainer.transform.childCount; i++)
        {
            _selectObjectUIs[i] = _selectContainer.transform.GetChild(i).GetComponent<SelectObjectUI>();
            _selectObjectUIs[i].ParentSetting(this);
            _selectObjectUIs[i].gameObject.SetActive(false);
        }

        _towerGameDatas = ObjectDataManager.Instance.GetAllToweGameData();
        _obstacleGameDatas = ObjectDataManager.Instance.GetAllObstacleGameData();

        int maxCount = _towerGameDatas.Count > _obstacleGameDatas.Count ? _towerGameDatas.Count : _obstacleGameDatas.Count;

        for (int i = 0; i < maxCount; i++)
        {
            _selectObjectUIs[i].gameObject.SetActive(true);
        }
    }

    public void TabUpdate(EObjectType objectType)
    {
        _nowType = objectType;
        List<ObjectData> playerSelectObjects = PlayerDataManager.Instance.SelectList;
        List<ObjectData> playerAvailableObjects = PlayerDataManager.Instance.AvailableList;

        if (objectType == EObjectType.Tower)
        {
            for (int i = 0; i < _towerGameDatas.Count; i++)
            {
                bool select = false;
                for (int j = 0; j < playerSelectObjects.Count; j++)
                {
                    if (playerSelectObjects[j].objectName == _towerGameDatas[i].objectName)
                        select = true;
                }
                bool available = false;
                for (int j = 0; j < playerAvailableObjects.Count; j++)
                {
                    if (playerAvailableObjects[j].objectName == _towerGameDatas[i].objectName)
                        available = true;
                    if (!PlayerDataManager.Instance.Selected)
                        available = false;
                }
                _selectObjectUIs[i].SelectObjectUISetting(_towerGameDatas[i], select, available);
            }
        }
        else
        {
            for (int i = 0; i < _obstacleGameDatas.Count; i++)
            {
                bool select = false;
                for (int j = 0; j < playerSelectObjects.Count; j++)
                {
                    if (playerSelectObjects[j].objectName == _obstacleGameDatas[i].objectName)
                        select = true;
                }
                bool available = false;
                for (int j = 0; j < playerAvailableObjects.Count; j++)
                {
                    if (playerAvailableObjects[j].objectName == _obstacleGameDatas[i].objectName)
                        available = true;
                    if (!PlayerDataManager.Instance.Selected)
                        available = false;
                }
                _selectedObjectUIs[i].gameObject.SetActive(true);
                _selectObjectUIs[i].SelectObjectUISetting(_obstacleGameDatas[i], select, available);
            }
        }
        CleanSelectData();
    }

    public void SelectedUpdate()
    {
        List<ObjectData> playerSelectObjects = PlayerDataManager.Instance.SelectList;
        for (int i = 0; i < playerSelectObjects.Count; i++)
        {
            _selectedObjectUIs[i].SelectSetting(playerSelectObjects[i].objectType, playerSelectObjects[i].objectName);
        }

        for (int i = playerSelectObjects.Count; i < PlayerDataManager.Instance.MaxSelected; i++)
        {
            _selectedObjectUIs[i].NoneSelectSetting();
        }
        LobbyManager.Instance.SelectedMiniUpdate();
    }

    public void SelectedCancle(EObjectType objectType, EObjectName objectName)
    {
        for (int i = 0; i < _selectedObjectUIs.Length; i++)
        {
            _selectedObjectUIs[i].SelectStop();
        }
        PlayerDataManager.Instance.SelectedCancleUpdate(objectType, objectName);
        SelectedUpdate();
        TabUpdate(_nowType);
        SoundManager.Instance.PlayEffectSound(ESoundName.Installation, null);
    }

    public override void Open(LobbyPlayer lobbyPlayer)
    {
        base.Open(lobbyPlayer);
        _towerGameDatas = ObjectDataManager.Instance.GetAllToweGameData();
        _obstacleGameDatas = ObjectDataManager.Instance.GetAllObstacleGameData();
        TabUpdate(EObjectType.Tower);
        SelectedUpdate();
    }

    public override void Close()
    {
        base.Close();
        for (int i = 0; i < _selectBorders.Length; i++)
        {
            _selectBorders[i].SetActive(false);
            _tabBtns[i].interactable = true;
        }
        _selectBorders[0].SetActive(true);
        _tabBtns[0].interactable = false;
        CleanSelectData();
    }

    public void ClickTabButton(int tabNumber)
    {
        for (int i = 0; i < _tabBtns.Length; i++)
        {
            if (i == tabNumber)
            {
                _tabBtns[i].interactable = false;
                _selectBorders[i].SetActive(true);
            }
            else
            {
                _tabBtns[i].interactable = true;
                _selectBorders[i].SetActive(false);
            }
        }
        TabUpdate((EObjectType)(tabNumber + 2));
        SelectedUpdate();
        SoundManager.Instance.PlayEffectSound(ESoundName.ButtonClick, null);
    }

    void CleanSelectData()
    {
        _objectNameTxt.text = "";
        _objectDesTxt.text = "";
    }

    public void ClickSelectObjectButton(EObjectType objectType, EObjectName objectName)
    {
        if (PlayerDataManager.Instance.Selected)
        {
            PlayerDataManager.Instance.SelectObjectUpdate(objectType, objectName);
            TabUpdate(_nowType);
            SelectedUpdate();
        }

        _objectNameTxt.text = ObjectDataManager.Instance.GetName(objectType, objectName);
        if (objectType == EObjectType.Tower)
        {
            TowerData towerData = ObjectDataManager.Instance.GetTowerData(objectName);
            _objectDesTxt.text = towerData.description;
        }
        else
        {
            ObstacleData obstacleData = ObjectDataManager.Instance.GetObstacleData(objectName);
            _objectDesTxt.text = obstacleData.description;
        }
        SoundManager.Instance.PlayEffectSound(ESoundName.Installation, null);
    }
}
