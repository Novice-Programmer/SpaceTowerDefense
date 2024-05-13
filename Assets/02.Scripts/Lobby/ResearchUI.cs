using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchUI : SelectUI
{
    [Header("Tab")]
    [SerializeField] Button[] _tabBtns = null;
    GameObject[] _selectBorders = null;

    [Header("Main")]
    [SerializeField] ResearchLvBoxUI[] _boxUIs = null;

    [Header("Sub")]
    [SerializeField] Text _researchAllTxt = null;
    [SerializeField] Text _researchSelectNameTxt = null;
    [SerializeField] Text _researchSelectTxt = null;

    EResearchType _nowTabType = EResearchType.Tower;

    private void Awake()
    {
        for (int i = 0; i < _boxUIs.Length; i++)
        {
            _boxUIs[i].ParentSetting(this);
        }
        _selectBorders = new GameObject[_tabBtns.Length];
        for (int i = 0; i < _tabBtns.Length; i++)
        {
            _selectBorders[i] = _tabBtns[i].transform.GetChild(0).gameObject;
        }
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
        TabUpdate((EResearchType)tabNumber);
        ResearchViewUpdate((EResearchType)tabNumber);
        SoundManager.Instance.PlayEffectSound(ESoundName.ButtonClick, null);
    }

    public void TabUpdate(EResearchType researchType)
    {
        Dictionary<int, List<ResearchData>> stepResearch = ResearchManager.Instance.GetStepResearch(researchType);
        for (int i = 0; i < _boxUIs.Length; i++)
        {
            _boxUIs[i].gameObject.SetActive(false);
        }
        foreach (int step in stepResearch.Keys)
        {
            if (step != 0)
            {
                _boxUIs[step - 1].gameObject.SetActive(true);
                _boxUIs[step - 1].ResearchChange(researchType);
            }
        }
        _nowTabType = researchType;
        _researchSelectNameTxt.text = "";
        _researchSelectTxt.text = "";
    }

    void ResearchViewUpdate(EResearchType researchType)
    {
        _researchAllTxt.text = ResearchManager.Instance.GetAllResearchString(researchType);
    }

    public void ResearchChange(ResearchData researchData)
    {
        PlayerDataManager.Instance.ResearchUpdate(_nowTabType, researchData.step, researchData.research);
        ResearchViewUpdate(_nowTabType);
        _researchSelectNameTxt.text = researchData.researchName;
        _researchSelectTxt.text = ResearchManager.Instance.GetSelectResearchString(_nowTabType, researchData.research);
        SoundManager.Instance.PlayEffectSound(ESoundName.ResearchChange, null);
    }

    public void ResearchCancle(ResearchData researchData)
    {
        PlayerDataManager.Instance.ResearchUpdate(_nowTabType, researchData.step, EResearch.None);
        ResearchViewUpdate(_nowTabType);
        _researchSelectNameTxt.text = "";
        _researchSelectTxt.text = "";
        SoundManager.Instance.PlayEffectSound(ESoundName.ResearchChange, null);
    }

    public override void Open(LobbyPlayer lobbyPlayer)
    {
        base.Open(lobbyPlayer);
        TabUpdate(EResearchType.Tower);
        ResearchViewUpdate(EResearchType.Tower);
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
    }
}
