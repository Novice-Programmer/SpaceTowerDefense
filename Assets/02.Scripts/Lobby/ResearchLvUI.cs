using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchLvUI : MonoBehaviour
{
    [SerializeField] GameObject _selectObject = null;
    [SerializeField] GameObject _lockObject = null;
    [SerializeField] Image _researchIcon = null;
    [SerializeField] Text _nameTxt = null;
    ResearchLvBoxUI _parent;

    ResearchData _researchData;

    public void ParentSetting(ResearchLvBoxUI parent)
    {
        _parent = parent;
    }

    public void ResearchSetting(ResearchData researchData, bool lockObject)
    {
        _selectObject.SetActive(false);
        _lockObject.SetActive(lockObject);
        _researchIcon.gameObject.SetActive(!lockObject);
        _researchIcon.sprite = researchData.icon;
        _researchData = researchData;
        if (lockObject)
        {
            _nameTxt.color = Color.red;
            _nameTxt.fontSize = 24;
            _nameTxt.text = "시설 연구 필요";
        }
        else
        {
            _nameTxt.color = Color.white;
            _nameTxt.fontSize = 28;
            _nameTxt.text = researchData.researchName;
        }
    }

    public void ResearchClick()
    {
        if (_selectObject.activeSelf)
        {
            _parent.SelectCancle(_researchData);
        }
        else
        {
            _parent.SelectChange(_researchData);
        }
        SoundManager.Instance.PlayEffectSound(ESoundName.ButtonClick, null);
    }

    public void NoneSelectCheck(EResearch research)
    {
        _selectObject.SetActive(_researchData.research == research);
    }
}
