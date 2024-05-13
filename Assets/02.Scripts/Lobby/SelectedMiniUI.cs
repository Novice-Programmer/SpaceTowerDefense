using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMiniUI : MonoBehaviour
{
    [SerializeField] Sprite _lockSprite = null;
    [SerializeField] Image _background = null;
    [SerializeField] Image _icon = null;

    public void SelectedSetting(Sprite icon)
    {
        _icon.gameObject.SetActive(true);
        _icon.sprite = icon;
    }

    public void NoneSelectSetting()
    {
        _icon.gameObject.SetActive(false);
        _icon.color = Color.white;
        _background.color = Color.white;
    }

    public void LockSelectSetting()
    {
        _icon.gameObject.SetActive(true);
        _icon.sprite = _lockSprite;
        _icon.color = Color.red;
        _background.color = Color.red;
    }
}
