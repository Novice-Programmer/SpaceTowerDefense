using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitZone : MonoBehaviour
{
    [SerializeField] bool _multiTarget = false;
    int _damage = 0;
    ETargetType _targetType;

    public void HitZoneSetting(int damage, ETargetType targetType)
    {
        _damage = damage;
        _targetType = targetType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckTarget.TargetTagCheck(_targetType, other.tag))
        {
            ObjectGame objectHit = other.GetComponent<ObjectGame>();
            objectHit.Hit(_damage, EWeakType.None);
            if (!_multiTarget)
                gameObject.SetActive(false);
        }
    }
}
