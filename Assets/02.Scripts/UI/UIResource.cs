using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResource : MonoBehaviour
{
    [SerializeField] Text _towerPartsTxt = null;
    [SerializeField] Text _spaceMineralTxt = null;
    [SerializeField] ResourceAddUI _prefabResourceAddUI = null;
    [SerializeField] Transform _addContainer = null;
    List<ResourceAddUI> _resourceAddUIs = new List<ResourceAddUI>();

    private void LateUpdate()
    {
        if (_resourceAddUIs.Count > 2)
        {
            if (_resourceAddUIs[0] != null)
                _resourceAddUIs[0].RemoveStart();
        }
        if (_resourceAddUIs.Count > 3)
        {
            for (int i = 0; i < _resourceAddUIs.Count; i++)
            {
                if (_resourceAddUIs[i] == null)
                {
                    _resourceAddUIs[3].gameObject.SetActive(true);
                    _resourceAddUIs.RemoveAt(i);
                    break;
                }
            }
        }
    }
    public void UIValueChange(bool gameValue, int value)
    {
        ResourceAddUI resourceAddUI = Instantiate(_prefabResourceAddUI, _addContainer);
        resourceAddUI.AddSetting(gameValue, value);
        _resourceAddUIs.Add(resourceAddUI);
        if (_resourceAddUIs.Count > 3)
        {
            resourceAddUI.gameObject.SetActive(false);
        }
        _towerPartsTxt.text = ResourceManager.Instance.TowerPartValue.ToString();
        _spaceMineralTxt.text = ResourceManager.Instance.SpaceMineralValue.ToString();
    }
}
