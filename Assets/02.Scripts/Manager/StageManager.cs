using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : TSingleton<StageManager>
{
    int _nowStage = 0;
    
    public int NowStage { get { return _nowStage; } }

    private void Awake()
    {
        Init();
        Instance = this;
    }
}
