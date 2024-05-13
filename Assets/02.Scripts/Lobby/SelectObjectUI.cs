using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectObjectUI : MonoBehaviour
{
    [SerializeField] GameObject _selectObject = null;
    [SerializeField] GameObject _lockObject = null;
    [SerializeField] Button _selectButton = null;
    [SerializeField] Image _selectIcon = null;
    [SerializeField] Text _selectName = null;

    GameObjectSelectUI _parent;
    EObjectType _objectType;
    EObjectName _objectName;

    public void SelectObjectUISetting(TowerGameData towerGameData, bool select, bool available)
    {
        _selectObject.SetActive(select);
        _selectButton.interactable = !select & available;
        _lockObject.SetActive(!available);
        _objectName = towerGameData.objectName;
        _objectType = EObjectType.Tower;
        _selectIcon.sprite = ObjectDataManager.Instance.GetImage(_objectName);
        _selectName.text = towerGameData.towerName;
    }

    public void SelectObjectUISetting(ObstacleGameData obstacleGameData, bool select, bool available)
    {
        _selectObject.SetActive(select);
        _selectButton.interactable = !select & available;
        _lockObject.SetActive(!available);
        _objectName = obstacleGameData.objectName;
        _objectType = EObjectType.Obstacle;
        _selectIcon.sprite = ObjectDataManager.Instance.GetImage(_objectName);
        _selectName.text = obstacleGameData.obstacleName;
    }

    public void ParentSetting(GameObjectSelectUI parent)
    {
        _parent = parent;
    }

    public void ClickButton()
    {
        _parent.ClickSelectObjectButton(_objectType, _objectName);
    }
}
