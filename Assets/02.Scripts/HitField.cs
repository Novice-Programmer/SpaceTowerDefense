using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitField : MonoBehaviour
{
    [SerializeField] BadBuff _badBuff = null;
    [SerializeField] EBadBuff _badBuffType = EBadBuff.None;
    [SerializeField] ETargetType _targetType = ETargetType.Enemy;
    [SerializeField] bool _eternity = false;
    [SerializeField] float _continueTime;
    float[] _values;

    float _timeCheck = 0.0f;

    private void Update()
    {
        if (_eternity)
            return;

        _timeCheck += Time.deltaTime;
        if (_timeCheck >= _continueTime)
        {
            Destroy(gameObject);
        }
    }

    public void HitPadSetting(ETargetType targetType, float[] values)
    {
        _targetType = targetType;
        _values = values;
        switch (_badBuffType)
        {
            case EBadBuff.Radioactivity:
                _continueTime = values[2];
                _badBuff.checkTime = (int)_values[3];
                _badBuff.dotTime = 1.0f;
                _badBuff.hp = (int)_values[4];
                break;
            case EBadBuff.Fire:
                _badBuff.hp = (int)_values[0];
                _badBuff.checkTime = _values[1];
                _badBuff.dotTime = _values[2];
                break;
            case EBadBuff.Slow:
                _badBuff.atkSpd = _values[0];
                _badBuff.movSpd = _values[1];
                _badBuff.checkTime = _values[2];
                break;
        }
        _badBuff.type = _badBuffType;
    }

    private void OnTriggerStay(Collider other)
    {
        if (CheckTarget.TargetTagCheck(_targetType, other.tag))
        {
            ObjectGame objectHit = other.GetComponent<ObjectGame>();
            if (objectHit.BuffCheck(_badBuffType))
            {
                BadBuff badBuff = Instantiate(_badBuff, other.transform);
                badBuff.transform.position = new Vector3(badBuff.transform.position.x, badBuff.transform.position.y + 1f, badBuff.transform.position.z);
                badBuff.target = objectHit;
                objectHit.BadBuff(badBuff);
            }
            else
            {
                objectHit.BadBuffUpdate(_badBuff);
            }
        }
    }
}
