using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBadBuff
{
    None,
    Radioactivity,
    Fire,
    Slow
}

[Serializable]
public class BadBuff : MonoBehaviour
{
    public EBadBuff type;
    public EWeakType weakType;
    public ObjectGame target;
    public int hp;
    public int mp;
    public int atk;
    public int def;
    public float atkSpd;
    public float movSpd;
    public float dotTime;
    public float checkTime;

    float _timeCheck;

    private void Update()
    {
        _timeCheck += Time.deltaTime;
        checkTime -= Time.deltaTime;
        if (checkTime <= 0)
        {
            Destroy(gameObject);
        }
        if (_timeCheck >= dotTime)
        {
            _timeCheck = 0;
            if (hp > 0)
            {
                target.Hit(hp, weakType);
            }
            if (mp > 0)
            {
                target.ReduceMP(mp);
            }
        }
    }

    public void TargetSetting(ObjectGame target)
    {
        this.target = target;
    }

    public void TargetDisable()
    {
        target = null;
        Destroy(gameObject);

    }
}
