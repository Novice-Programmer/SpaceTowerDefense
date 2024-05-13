using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SelectData
{
    public EObjectType objectType;
    public EObjectName objectName;
    public ResearchData[] researchDatas;

    public SelectData(EObjectType objectType, EObjectName objectName, ResearchData[] researchDatas)
    {
        this.objectType = objectType;
        this.objectName = objectName;
        this.researchDatas = researchDatas;
    }
}
