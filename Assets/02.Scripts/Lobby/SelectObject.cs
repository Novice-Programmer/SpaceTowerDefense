using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObject : MonoBehaviour
{
    public string _selectName = ""; 
    [SerializeField] SelectUI _selectUI =  null;


    public void OnPlayerEnter()
    {

    }

    public void OnPlayerExit()
    {

    }

    public void Select(LobbyPlayer lobbyPlayer)
    {
        _selectUI.Open(lobbyPlayer);
    }
}
