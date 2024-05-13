using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectGame : MonoBehaviour
{
    [Header("ObjectGame")]
    public EObjectType _objectType = EObjectType.None;
    public EStateType _stateType = EStateType.None;
    public bool _objectSelectActive = false;
    public bool _objectSelect = false;
    public Transform _attackPos = null;
    public virtual void Hit(int damage, EWeakType weakType)
    {

    }

    public virtual void ReduceMP(int mp)
    {

    }

    public virtual void BadBuff(BadBuff badBuff)
    {

    }

    public virtual bool BuffCheck(EBadBuff buffType)
    {
        return false;
    }

    public virtual void BadBuffUpdate(BadBuff badBuff)
    {
    }

    public virtual void Select(bool selectOff = true)
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.SelectObject, null);
    }
}
