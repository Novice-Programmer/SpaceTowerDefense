using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightMareDragonA : Enemy
{
    [Header("NMDA")]
    [SerializeField] Transform _specialAttackPos = null;
    [SerializeField] GameObject _attackEffect = null;
    [SerializeField] Projectile _specialAttack = null;
    [SerializeField] HitZone _attackZone = null;
    public override void TargetAttack()
    {
        base.TargetAttack();
        if (_target == null)
        {
            return;
        }
        _attackZone.HitZoneSetting(_atk, ETargetType.Player);
        _attackZone.gameObject.SetActive(true);
        Vector3 dir = _attackZone.transform.position - _target.position;
        Instantiate(_attackEffect, _target.GetComponent<ObjectGame>()._attackPos.position + dir.normalized * 0.5f, Quaternion.identity);
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
        SoundManager.Instance.PlayEffectSound(_attackSound, transform);
        _attackZone.gameObject.SetActive(false);
    }

    public override void TargetSpecialAttack()
    {
        base.TargetSpecialAttack();
        Projectile attack = Instantiate(_specialAttack, _specialAttackPos.position, _specialAttackPos.rotation);
        if (_target != null)
            attack.ProjectileSetting(_target.GetComponent<ObjectGame>(), ETargetType.Player, (int)(_atk * 2.5f));
        else
            attack.ProjectileSetting(transform.position + transform.forward, ETargetType.Player, (int)(_atk * 2.5f));
    }

    public override void SkillEnd()
    {
        base.SkillEnd();
    }
}
