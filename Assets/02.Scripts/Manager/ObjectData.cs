using System;

[Serializable]
public class ObjectData
{
    public EObjectType objectType;
    public EObjectName objectName;

    public ObjectData()
    {

    }

    public ObjectData(EObjectType objectType, EObjectName objectName)
    {
        this.objectType = objectType;
        this.objectName = objectName;
    }
}
