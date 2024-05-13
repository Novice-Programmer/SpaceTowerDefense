using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDragonU : Enemy
{
    [Header("BDU")]
    [SerializeField] HitZone _attackZone = null;
    [SerializeField] ParticleSystem _attackEffect = null;
    [SerializeField] HitZone _skillZone = null;
    [SerializeField] ParticleSystem _skillEffect = null;

    public override void TargetAttack()
    {
        base.TargetAttack();
        _attackEffect.gameObject.SetActive(true);
        _attackEffect.Play();
        SoundManager.Instance.PlayEffectSound(_attackSound, transform);
        if (_target != null)
        {
            _attackZone.HitZoneSetting(_atk, ETargetType.Player);
        }
        else
        {
            _attackZone.HitZoneSetting(_atk, ETargetType.Player);
        }
        _attackZone.gameObject.SetActive(true);
    }

    public void EndAttack()
    {
        _attackEffect.gameObject.SetActive(false);
        _attackZone.gameObject.SetActive(false);
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
    }

    public void WingSound()
    {
        SoundManager.Instance.PlayEffectSound(ESoundName.Wing, transform);
    }

    public override void TargetSpecialAttack()
    {
        base.TargetSpecialAttack();
        _skillEffect.gameObject.SetActive(true);
        _skillEffect.Play();
        SoundManager.Instance.PlayEffectSound(_attackSound, transform);
        if (_target != null)
        {
            _skillZone.HitZoneSetting((int)(_atk * 1.5f), ETargetType.Player);
        }
        else
        {
            _skillZone.HitZoneSetting((int)(_atk * 1.5f), ETargetType.Player);
        }
        _skillZone.gameObject.SetActive(true);
    }

    public void EndSkill()
    {
        _skillEffect.gameObject.SetActive(false);
        _skillZone.gameObject.SetActive(false);
    }

    public override void SkillEnd()
    {
        base.SkillEnd();
    }
}
