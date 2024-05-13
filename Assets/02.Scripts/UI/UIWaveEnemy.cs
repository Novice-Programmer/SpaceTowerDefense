using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaveEnemy : MonoBehaviour
{
    [SerializeField] Image _rankImage = null;
    [SerializeField] Image _iconImage = null;

    public void WaveEnemyInfo(Sprite iconSprite,Sprite rankSprite)
    {
        _rankImage.sprite = rankSprite;
        _iconImage.sprite = iconSprite;
    }
}
