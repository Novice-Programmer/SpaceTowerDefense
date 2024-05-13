using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacilityUI : SelectUI
{
    [SerializeField] Transform _container = null;
    [SerializeField] FacilityLvBoxUI[] _facilityLvBoxUIs = null;

    [SerializeField] Text _facilityNameTxt = null;
    [SerializeField] Text _facilityEffectNameTxt = null;
    [SerializeField] Text _facilityEffectTxt = null;
    [SerializeField] Text _facilityValueTxt = null;
    [SerializeField] Text _playerValueTxt = null;
    [SerializeField] Text _warningTxt = null;
    [SerializeField] Button _researchBtn = null;
    FacilityData _selectFacility;

    private void Awake()
    {
        _facilityLvBoxUIs = new FacilityLvBoxUI[_container.childCount];
        for (int i = 0; i < _container.childCount; i++)
        {
            _facilityLvBoxUIs[i] = _container.GetChild(i).GetComponent<FacilityLvBoxUI>();
            _facilityLvBoxUIs[i].ParentSetting(this);
        }
    }

    public override void Open(LobbyPlayer lobbyPlayer)
    {
        base.Open(lobbyPlayer);
        FacilityUpdate();
    }

    public override void Close()
    {
        base.Close();
        ClearView();
    }

    public void FacilityUpdate()
    {
        for (int i = 0; i < _facilityLvBoxUIs.Length; i++)
        {
            int level = PlayerDataManager.Instance.GetFacilityLevel(_facilityLvBoxUIs[i]._researchType);
            _facilityLvBoxUIs[i].FacilityBoxSetting(level);
        }
    }

    public void FacilitySelect(FacilityData facilityData)
    {
        ClearView();
        _selectFacility = facilityData;
        FacilityDataView();
    }

    void FacilityDataView()
    {
        _facilityNameTxt.text = _selectFacility._dataName;
        _facilityEffectNameTxt.text = "연구 효과";
        string str = "";
        for (int i = 0; i < _selectFacility._dataEffect.Length; i++)
        {
            str += _selectFacility._dataEffect[i] + "\n";
        }
        str.Remove(str.Length - 1);
        _facilityEffectTxt.text = str;
        _facilityValueTxt.text = "요구 우주 광물 : " + _selectFacility._researchValue;
        _playerValueTxt.text = "보유 우주 광물 : " + ResourceManager.Instance.SpaceMineralValue;
        bool possibility = ResourceManager.Instance.SpaceMineralValue >= _selectFacility._researchValue;
        _researchBtn.gameObject.SetActive(possibility);
        _warningTxt.gameObject.SetActive(!possibility);
    }

    void ClearView()
    {
        _selectFacility = null;
        _facilityNameTxt.text = "연구를 선택 해주세요";
        _facilityEffectNameTxt.text = "";
        _facilityEffectTxt.text = "";
        _facilityValueTxt.text = "";
        _playerValueTxt.text = "";
        for (int i = 0; i < _facilityLvBoxUIs.Length; i++)
        {
            _facilityLvBoxUIs[i].NoneSelect();
        }
        _warningTxt.gameObject.SetActive(false);
        _researchBtn.gameObject.SetActive(false);
    }

    public void SelectedCancle()
    {
        ClearView();
    }

    public void ClickResearchFacility()
    {
        PlayerDataManager.Instance.FacilityUpdate(_selectFacility._researchType, _selectFacility._facilityLevel);
        ResourceManager.Instance.SpaceMineralValue = -_selectFacility._researchValue;
        ClearView();
        FacilityUpdate();
        LobbyManager.Instance.FacilityUpdate();
        LobbyManager.Instance.SelectedMiniUpdate();
        SoundManager.Instance.PlayEffectSound(ESoundName.FacilityUpdate, null);
    }
}
