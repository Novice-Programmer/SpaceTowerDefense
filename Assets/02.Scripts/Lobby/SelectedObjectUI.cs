using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObjectUI : MonoBehaviour
{
    [SerializeField] Button _cancleBtn = null;
    [SerializeField] GameObject _dontSelect = null;
    [SerializeField] GameObject _selectObject = null;
    [SerializeField] Image _selectImage = null;
    [SerializeField] Text _selectNameTxt = null;

    GameObjectSelectUI _parent;

    EObjectType _objectType;
    EObjectName _objectName;

    public void ParentSetting(GameObjectSelectUI parent)
    {
        _parent = parent;
    }

    public void SelectSetting(EObjectType objectType, EObjectName objectName)
    {
        _objectType = objectType;
        _objectName = objectName;
        _cancleBtn.interactable = true;
        _dontSelect.SetActive(false);
        _selectObject.SetActive(true);
        _selectImage.sprite = ObjectDataManager.Instance.GetImage(_objectName);
        _selectNameTxt.text = ObjectDataManager.Instance.GetName(_objectType, _objectName);
    }

    public void NoneSelectSetting()
    {
        _cancleBtn.interactable = false;
        _selectObject.SetActive(false);
        _selectImage.sprite = null;
        _selectNameTxt.text = "";
        _dontSelect.SetActive(false);
    }

    public void SelectStop()
    {
        _cancleBtn.interactable = false;
    }

    public void ClickButton()
    {
        _parent.SelectedCancle(_objectType, _objectName);
    }
}
