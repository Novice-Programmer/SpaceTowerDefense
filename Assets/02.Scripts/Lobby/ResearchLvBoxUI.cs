using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchLvBoxUI : MonoBehaviour
{
    [SerializeField] int step = 0;
    ResearchUI _parent;
    [SerializeField] ResearchLvUI[] _lvUIs = null; 

    [Header("Select")]
    [SerializeField] GameObject _lockObj = null;
    [SerializeField] Image _selectIcon = null;

    public void ParentSetting(ResearchUI researchUI)
    {
        _parent = researchUI;
        for(int i = 0; i < _lvUIs.Length; i++)
        {
            _lvUIs[i].ParentSetting(this);
        }
    }

    public void ResearchChange(EResearchType researchType)
    {
        ResearchData selectResearch = ResearchManager.Instance.GetResearchData(PlayerDataManager.Instance.GetSelectResearch(researchType, step));
        bool lockObject = !PlayerDataManager.Instance.GetResearchCheck(researchType, step);
        _lockObj.SetActive(lockObject);
        if (selectResearch != null)
        {
            _selectIcon.gameObject.SetActive(true);
            _selectIcon.sprite = selectResearch.icon;
        }
        else
        {
            _selectIcon.gameObject.SetActive(false);
            _selectIcon.sprite = null;
        }

        for(int i = 0; i < _lvUIs.Length; i++)
        {
            _lvUIs[i].gameObject.SetActive(false);
        }

        // 연구목록 가져와서 반영 , 선택 정보 셀렉트
        ResearchData[] stepResearch = ResearchManager.Instance.GetStepResearch(researchType, step);
        for(int i = 0; i < stepResearch.Length; i++)
        {
            _lvUIs[i].gameObject.SetActive(true);
            _lvUIs[i].ResearchSetting(stepResearch[i],lockObject);
            if(selectResearch!=null)
            {
                _lvUIs[i].NoneSelectCheck(selectResearch.research);
            }
        }
    }

    public void SelectChange(ResearchData researchData)
    {
        for(int i = 0; i < _lvUIs.Length; i++)
        {
            if (_lvUIs[i].gameObject.activeSelf)
            {
                _lvUIs[i].NoneSelectCheck(researchData.research);
            }
        }
        _parent.ResearchChange(researchData);
        _selectIcon.gameObject.SetActive(true);
        _selectIcon.sprite = researchData.icon;
    }

    public void SelectCancle(ResearchData researchData)
    {
        for (int i = 0; i < _lvUIs.Length; i++)
        {
            if (_lvUIs[i].gameObject.activeSelf)
            {
                _lvUIs[i].NoneSelectCheck(EResearch.None);
            }
        }
        _parent.ResearchCancle(researchData);
        _selectIcon.gameObject.SetActive(false);
        _selectIcon.sprite = null;
    }
}
