using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] PlayerAttackCol _atkCollision;
    Animator _animator;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnTired()
    {
        _animator.SetTrigger("OnTired");
    }

    public void OnMovement(bool move)
    {
        _animator.SetBool("IsMove", move);
    }

    public void OnWalk(bool value)
    {
        _animator.SetBool("IsWalk", value);
    }

    public void OnJump()
    {
        _animator.SetTrigger("OnJump");
    }

    public void OnDodge()
    {
        _animator.SetTrigger("OnDodge");
    }

    public void OnRunAddMotion()
    {
        _animator.SetTrigger("OnRunAddMotion");
    }

    public void OnAttack()
    {
        _animator.SetTrigger("OnAttack");
    }

    public void OnChargStart()
    {
        _animator.SetTrigger("OnChargeSet");
    }

    public void OnCharging(float time)
    {
        _animator.SetFloat("ChargeTime", time);
    }

    public void OnChargingFire()
    {
        _animator.SetTrigger("OnChargingFire");
    }

    public void OnSkill()
    {
        _animator.SetTrigger("OnSkill");
    }

    public void IsGround(bool value)
    {
        _animator.SetBool("IsGround", value);
    }

    public void OnAttackCollision()
    {
        _atkCollision.Show();
    }

    public void OnAttackCollisionShow(int show)
    {
        bool value = (show == 1) ? true : false;

        _atkCollision.OnColEnable(value);
    }
}
