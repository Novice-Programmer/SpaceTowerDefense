using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : SelectObject
{
    [SerializeField] SubLaboratory[] _subObjects = null;
    Dictionary<EResearchType, SubLaboratory> _typeLab = new Dictionary<EResearchType, SubLaboratory>();

    private void Awake()
    {
        for(int i = 0; i < _subObjects.Length; i++)
        {
            _typeLab.Add(_subObjects[i]._labType, _subObjects[i]);
        }
    }

    private void Start()
    {
        LevelCheck(PlayerDataManager.Instance.GetFacilityData());
    }

    void LevelCheck(Dictionary<EResearchType, int> researchType)
    {
        foreach(EResearchType type in researchType.Keys)
        {
            _typeLab[type].SubSetting(researchType[type]);
        }
    }

    public void FacilityUpdate()
    {
        LevelCheck(PlayerDataManager.Instance.GetFacilityData());
    }
}
