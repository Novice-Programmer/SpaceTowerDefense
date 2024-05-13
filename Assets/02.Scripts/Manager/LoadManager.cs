using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : TSingleton<LoadManager>
{
    private void Awake()
    {
    }
    public void Save()
    {

    }

    public void Load()
    {
        PlayerDataManager.Instance.LoadData(null, null, null, null, null, null);
    }
}
