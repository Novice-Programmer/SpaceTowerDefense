using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityLvUI : MonoBehaviour
{
    [SerializeField] GameObject _lockObject = null;
    [SerializeField] Button _selectButton = null;
    [SerializeField] Image _selectImage = null;
    [SerializeField] Text _nameTxt = null;
    [SerializeField] Text _requireTxt = null;

    [SerializeField] FacilityData _facilityData = null;

    FacilityLvBoxUI _parent;
    int _selectNumber = 0;
    bool _selected = false;

    public void ParentSetting(FacilityLvBoxUI parent, int selectNumber)
    {
        _parent = parent;
        _selectNumber = selectNumber;
    }

    public void FacilityBtnSetting(int facilityLevel, int labLevel)
    {
        if (facilityLevel >= _facilityData._facilityLevel)
        {
            _lockObject.SetActive(false);
            _selectButton.interactable = false;
            _nameTxt.text = "연구 완료";
        }
        else
        {
            if (facilityLevel != _facilityData._facilityLevel - 1)
            {
                _lockObject.SetActive(true);
                _selectButton.interactable = false;
            }
            else if (_facilityData._requireLabLevel > labLevel)
            {
                if (_facilityData._requireLabLevel == 0)
                    _requireTxt.text = "기본 연구 필요";
                else
                    _requireTxt.text = "연구 Lv." + _facilityData._requireLabLevel + " 필요";
                _lockObject.SetActive(true);
                _selectButton.interactable = false;
            }
            else
            {
                _lockObject.SetActive(false);
                _selectButton.interactable = true;
            }
        }
    }

    public void ClickButton()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.ButtonClick, null);
        if (_selected)
            _parent.FacilityCancle();
        else
            _parent.FacilityClick(_selectNumber, _facilityData);
    }

    public void Select()
    {
        _selectImage.color = Color.red;
        _selected = true;
    }


    public void NoneSelect()
    {
        _selectImage.color = Color.white;
        _selected = false;
    }
}
