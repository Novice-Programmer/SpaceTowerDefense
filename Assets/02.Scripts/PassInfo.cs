using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EPassType
{
    None,
    Select,
    Hit
}

public class PassInfo : ObjectGame
{
    [SerializeField] ObjectGame _passData = null;
    [SerializeField] EPassType _passType = EPassType.None;

    public override void Select(bool selectOff = true)
    {
        if (_passType == EPassType.Select)
            _passData.Select(selectOff);
    }

    public override void Hit(int damage, EWeakType weakType)
    {
        if (_passType == EPassType.Hit)
            _passData.Hit(damage, weakType);
    }
}
