using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLaboratory : MonoBehaviour
{
    public EResearchType _labType = EResearchType.Facility;
    [SerializeField] GameObject[] _levelObjects = null;

    public void SubSetting(int level)
    {
        for (int i = 0; i < _levelObjects.Length; i++)
        {
            _levelObjects[i].SetActive(false);
        }
        if (level == -1)
        {
            if(_labType == EResearchType.Facility)
            {
                _levelObjects[0].SetActive(true);
            }
        }

        else if (level > -1 && level < 2)
        {
            _levelObjects[0].SetActive(true);
        }
        else
        {
            _levelObjects[level - 1].SetActive(true);
        }
    }
}
