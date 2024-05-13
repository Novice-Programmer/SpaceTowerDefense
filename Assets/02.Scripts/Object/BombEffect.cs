using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    [SerializeField] EWeakType _weakType = EWeakType.None;
    [SerializeField] float _bombTime = 0.5f;
    [SerializeField] HitField _hitField = null;
    [SerializeField] bool _oneTarget = false;
    [SerializeField] bool _oneShot = false;
    [SerializeField] bool _delayShot = false;
    [SerializeField] float _delayTime = 1;

    ObjectGame _target;
    ETargetType _targetType;
    float[] _valeus;
    float _timeCheck = 0.0f;
    bool _dealCheck = true;

    private void Start()
    {
        if (_hitField != null)
        {
            HitField hitPad = Instantiate(_hitField, transform.position, Quaternion.identity);
            hitPad.HitPadSetting(_targetType, _valeus);
        }
    }

    private void Update()
    {
        _timeCheck += Time.deltaTime;
        if (_delayShot)
        {
            if (_timeCheck >= _delayTime)
            {
                _delayShot = false;
                GetComponent<BoxCollider>().enabled = true;
            }
        }
        if (_timeCheck >= _bombTime)
        {
            _dealCheck = false;
            GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void BombSetting(ETargetType targetType, params float[] values)
    {
        _targetType = targetType;
        _valeus = values;
    }

    public void BombSetting(ObjectGame target, ETargetType targetType, params float[] values)
    {
        _target = target;
        _targetType = targetType;
        _valeus = values;
        if (_oneTarget)
        {
            TargetBoom();
        }
    }

    void TargetBoom()
    { 
        _target.Hit((int)_valeus[0], _weakType);
        GetComponent<BoxCollider>().enabled = false;
    }

    void Bomb()
    {
        List<GameObject> hitObjectList = new List<GameObject>();
        string[] tags = CheckTarget.GetTargetTags(_targetType);
        for (int i = 0; i < tags.Length; i++)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tags[i]);
            for (int j = 0; j < objects.Length; j++)
            {
                hitObjectList.Add(objects[j]);
            }
        }

        GameObject[] hitObjects = hitObjectList.ToArray();
        float range;
        if (_valeus.Length < 2)
        {
            range = 0.5f;
        }
        else
        {
            range = _valeus[1];
        }

        foreach (GameObject hitObject in hitObjects)
        {
            if (Vector3.Distance(transform.position, hitObject.transform.position) < range)
            {
                if (_dealCheck)
                {
                    ObjectGame objectHit = hitObject.GetComponent<ObjectGame>();
                    objectHit.Hit((int)_valeus[0], _weakType);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckTarget.TargetTagCheck(_targetType, other.tag))
        {
            if (_oneShot)
            {
                ObjectGame objectHit = other.GetComponent<ObjectGame>();
                objectHit.Hit((int)_valeus[0], _weakType);
            }
            else
            {
                GetComponent<BoxCollider>().enabled = false;
                Bomb();
            }
        }
    }
}
