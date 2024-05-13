using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityLvBoxUI : MonoBehaviour
{
    public EResearchType _researchType = EResearchType.Facility;
    [SerializeField] Transform _lvUIContainer = null;
    [SerializeField] Transform _lineContainer = null;

    FacilityLvUI[] _facilityLvUIs = null;
    Image[] _lineImages = null;

    FacilityUI _parent;

    private void Awake()
    {
        _facilityLvUIs = new FacilityLvUI[_lvUIContainer.childCount];
        for (int i = 0; i < _lvUIContainer.childCount; i++)
        {
            _facilityLvUIs[i] = _lvUIContainer.GetChild(i).GetComponent<FacilityLvUI>();
            _facilityLvUIs[i].ParentSetting(this, i);
        }
        _lineImages = new Image[_lineContainer.childCount];
        for (int i = 0; i < _lineContainer.childCount; i++)
        {
            _lineImages[i] = _lineContainer.GetChild(i).GetComponent<Image>();
        }
    }

    public void FacilityBoxSetting(int facilityLv)
    {
        for (int i = 0; i < _facilityLvUIs.Length; i++)
        {
            _facilityLvUIs[i].FacilityBtnSetting(facilityLv, PlayerDataManager.Instance.GetFacilityLevel(EResearchType.Facility));
        }
        facilityLv = facilityLv > 2 ? facilityLv - 1 : facilityLv;
        for (int i = 0; i < facilityLv + 1; i++)
        {
            _lineImages[i].color = Color.white;
        }
    }

    public void ParentSetting(FacilityUI parent)
    {
        _parent = parent;
    }

    public void FacilityClick(int selectNumber, FacilityData facilityData)
    {
        _parent.FacilitySelect(facilityData);
        _facilityLvUIs[selectNumber].Select();
    }

    public void FacilityCancle()
    {
        _parent.SelectedCancle();
    }

    public void NoneSelect()
    {
        for (int i = 0; i < _facilityLvUIs.Length; i++)
        {
            _facilityLvUIs[i].NoneSelect();
        }
    }
}
