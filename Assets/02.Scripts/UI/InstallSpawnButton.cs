using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InstallSpawnButton : MonoBehaviour
{
    [SerializeField] InstallData _buttonData;
    [SerializeField] GameObject _lockBtn = null;
    [SerializeField] Image _towerImage = null;
    [SerializeField] Text _partCostTxt = null;

    int _installCost = 0;

    public void ButtonDataSetting(InstallData buttonData)
    {
        _buttonData = buttonData;
        _towerImage.sprite = _buttonData.objectImage;
        _installCost = _buttonData.installCost;
        _partCostTxt.text = _buttonData.installCost.ToString();
    }

    public void InstallMoneyCheck()
    {
        if (ResourceManager.Instance.TowerPartValue >= _installCost)
        {
            _partCostTxt.color = Color.white;
            _lockBtn.SetActive(false);
        }
        else
        {
            _partCostTxt.color = Color.red;
            _lockBtn.SetActive(true);
        }
    }

    public void InstallClick()
    {
        GameUI.Instance.ViewUIOff();
        GameManager.Instance.Install(_buttonData.objectType, _buttonData.objectName, _buttonData.installCost);
        InputManager.Instance.UITouch();
    }
}
