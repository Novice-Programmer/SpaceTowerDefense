using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P013 : Tower
{
    [SerializeField] LineRenderer _laserRenderer = null;
    [SerializeField] Transform _firePoint = null;
    [SerializeField] GameObject _fireEffect = null;
    [SerializeField] GameObject _impactEffect = null;
    BombEffect _skillLaser;

    SoundSource _playAttackSound = null;
    bool _attack = false;
    float _timeCheck = 0;
    float _attackTime = 0;

    private void Awake()
    {
        _skillLaser = (Resources.Load("Tower/P013_Laser") as GameObject).GetComponent<BombEffect>();
    }

    private void Update()
    {
        if (_attack)
        {
            _timeCheck += Time.deltaTime;
            _attackTime += Time.deltaTime;
            if (_nearestTarget != null)
            {
                _laserRenderer.SetPosition(0, _firePoint.position);
                _laserRenderer.SetPosition(1, _nearestTarget.GetComponent<ObjectGame>()._attackPos.position);
                _impactEffect.transform.position = _nearestTarget.GetComponent<ObjectGame>()._attackPos.position;
                if (_attackTime >= 1 / (_atkSpd * 2.5f))
                {
                    _attackTime = 0;
                    _nearestTarget.GetComponent<ObjectGame>().Hit(_atk, EWeakType.Fire);
                    ChargingEP();
                    if (_ep == 100)
                    {
                        AttackEnd();
                    }
                }
            }
            else
            {
                AttackEnd();
            }
            if (_timeCheck >= _atkSpd * 2f)
            {
                AttackEnd();
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();
        _timeCheck = 0;
        _attack = true;
        _laserRenderer.enabled = true;
        _impactEffect.SetActive(true);
        _fireEffect.SetActive(true);
        _laserRenderer.SetPosition(0, _firePoint.position);
        _laserRenderer.SetPosition(1, _nearestTarget.position);
        _impactEffect.transform.position = _nearestTarget.position;
        _playAttackSound = SoundManager.Instance.PlayEffectSound(ESoundName.Laser, transform);
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        _attack = false;
        _laserRenderer.enabled = false;
        _impactEffect.SetActive(false);
        _fireEffect.SetActive(false);
        if (_playAttackSound != null && _playAttackSound.gameObject.activeSelf)
            _playAttackSound._audioSource.Stop();
    }

    protected override void SpecialAttack()
    {
        base.SpecialAttack();
        StartCoroutine(LaserLaunch());
    }

    IEnumerator LaserLaunch()
    {
        BombEffect laser = Instantiate(_skillLaser, _firePoint.position, _firePoint.rotation);
        laser.BombSetting(ETargetType.Enemy, _spValue);
        SoundManager.Instance.PlayEffectSound(ESoundName.ChargingLaser, transform);
        yield return new WaitForSeconds(2.0f);
        SkillEnd();
    }

    protected override void SkillEnd()
    {
        base.SkillEnd();
    }
}
