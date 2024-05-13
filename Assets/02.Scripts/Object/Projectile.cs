using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float _maxTime = 30.0f;
    [Header("Status")]
    [SerializeField] BombEffect _bombObject = null;
    [SerializeField] ESoundName _launchSound = ESoundName.Rocket;
    [SerializeField] ESoundName _boomSound = ESoundName.RocketBomb;

    [SerializeField] float[] _values = null;

    [Header("TransValue")]
    [SerializeField] float _speed = 10f;
    [SerializeField] float _rotateSpeed = 15f;
    [SerializeField] bool _parabola = false;
    [SerializeField] float _upSpeed = 10f;
    [SerializeField] float _maxUp = 10.0f;
    [SerializeField] float _minY = 0.2f;

    ObjectGame _target;
    Vector3 _targetPos;
    ETargetType _targetType;
    bool _guidance;

    float _timeCheck = 0.0f;

    private void Start()
    {
        SoundManager.Instance.PlayEffectSound(_launchSound, transform);
    }

    private void Update()
    {
        _timeCheck += Time.deltaTime;
        if (_timeCheck >= _maxTime)
        {
            BombEffect bomb = Instantiate(_bombObject, transform.position, transform.rotation);
            bomb.BombSetting(_targetType, _values);
            Destroy(gameObject);
        }
        if (_parabola)
        {
            if (transform.position.y > _maxUp)
            {
                _parabola = false;
                return;
            }
            transform.Translate(Vector3.up * _upSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            Vector3 dir;
            Vector3 pos;
            if (_guidance)
            {
                if (_target == null)
                {
                    _guidance = false;
                    return;
                }
                else if (!_target._objectSelectActive)
                {
                    _guidance = false;
                    return;
                }
                dir = _target._attackPos.position - transform.position;
                if (dir.y > -_minY)
                {
                    dir.y = 0;
                }
                pos = _target._attackPos.position;
            }
            else
            {
                dir = _targetPos - transform.position;
                pos = _targetPos;
            }

            transform.Translate(dir.normalized * _speed * Time.deltaTime, Space.World);
            transform.forward = Vector3.Lerp(transform.forward, dir.normalized, _rotateSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pos) < 0.05f)
            {
                Bomb();
                return;
            }
        }
    }

    public void ProjectileSetting(ObjectGame target, ETargetType targetType, params float[] values)
    {
        _target = target;
        _targetPos = _target._attackPos.position;
        _guidance = true;
        _targetType = targetType;
        _values = values;
    }

    public void ProjectileSetting(Vector3 targetPos, ETargetType targetType, params float[] values)
    {
        _targetPos = targetPos;
        _guidance = false;
        _targetType = targetType;
        _values = values;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Field"))
        {
            Bomb();
        }

        else if (CheckTarget.TargetTagCheck(_targetType, other.tag))
        {
            ObjectGame objectGame = other.GetComponent<ObjectGame>();
            Bomb(objectGame);
        }
    }

    void Bomb(ObjectGame objectGame = null)
    {
        SoundManager.Instance.PlayEffectSound(_boomSound, transform);
        BombEffect bomb = Instantiate(_bombObject, transform.position, transform.rotation);
        if (objectGame != null)
        {
            bomb.BombSetting(objectGame, _targetType, _values);
        }
        else
        {
            bomb.BombSetting(_targetType, _values);
        }
        Destroy(gameObject);
    }
}
