using UnityEngine;

public abstract class SelectUI : MonoBehaviour
{
    LobbyPlayer _lobbyPlayer;
    public virtual void Open(LobbyPlayer lobbyPlayer)
    {
        _lobbyPlayer = lobbyPlayer;
        SoundManager.Instance.PlayEffectSound(ESoundName.UIOpen, null);
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        SoundManager.Instance.PlayEffectSound(ESoundName.UIClose, null);
        _lobbyPlayer.SelectEnd();
        _lobbyPlayer = null;
    }
}
