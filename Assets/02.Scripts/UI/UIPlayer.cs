using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    [SerializeField] Slider _hpSlider = null;
    [SerializeField] Text _hpTxt = null;

    public void HPSetting(int maxHP)
    {
        _hpSlider.maxValue = maxHP;
        _hpSlider.value = maxHP;
        _hpTxt.text = maxHP + " / " + maxHP;
    }

    public void ChangeHP(int hp)
    {
        _hpSlider.value = hp;
        _hpTxt.text = hp + " / " + _hpSlider.maxValue;
    }
}
