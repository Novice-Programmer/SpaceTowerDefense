using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        SceneControlManager.Instance.SceneChange(ESceneType.Lobby);
    }
}
