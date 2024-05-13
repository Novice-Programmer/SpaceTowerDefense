using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KW9A : Tower
{
    [Header("KW9A")]
    [SerializeField] Transform _firePointParent = null;
    [SerializeField] Transform _nuclearPoint = null;
    Transform[] _firePoints;
    Projectile _rocketMissile;
    Projectile _rocketNuclear;

    private void Awake()
    {
        _rocketMissile = (Resources.Load("Tower/KW9A_Rocket") as GameObject).GetComponent<Projectile>();
        _rocketNuclear = (Resources.Load("Tower/KW9A_NuclearRocket") as GameObject).GetComponent<Projectile>();
        _firePoints = new Transform[_firePointParent.childCount];
        for (int i = 0; i < _firePoints.Length; i++)
        {
            _firePoints[i] = _firePointParent.GetChild(i);
        }
    }

    protected override void Attack()
    {
        base.Attack();
        StartCoroutine(MissileLaunch());
    }

    IEnumerator MissileLaunch()
    {
        for (int i = 0; i < _atkNumber; i++)
        {
            for (int j = 0; j < _target.Length; j++)
            {
                int _randInt = Random.Range(0, _firePoints.Length);
                Projectile missile = Instantiate(_rocketMissile, _firePoints[_randInt].position, _firePoints[_randInt].rotation);
                missile.ProjectileSetting(_target[j].GetComponent<ObjectGame>(), ETargetType.Enemy, _atk);
                yield return new WaitForSeconds(Random.Range(_atkSpd * 0.1f, _atkSpd * 0.2f));
                if (_target == null)
                {
                    break;
                }
            }
            yield return new WaitForSeconds(Random.Range(_atkSpd * 0.3f, _atkSpd * 0.5f));
            if (_target == null)
            {
                break;
            }
        }
        ChargingEP();
        AttackEnd();
    }

    protected override void SpecialAttack()
    {
        base.SpecialAttack();
        StartCoroutine(NuclearLaunch());
    }

    IEnumerator NuclearLaunch()
    {
        Projectile nuclear = Instantiate(_rocketNuclear, _nuclearPoint.position, _nuclearPoint.rotation);
        nuclear.ProjectileSetting(_target[0].position, ETargetType.Enemy, _spValue);
        yield return new WaitForSeconds(2 / _atkSpd);
        SkillEnd();
    }
}
