using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject _atkCollision;
    Animator _animator;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void OnMovement(float horizontal, float vertical)
    {
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Vertical", vertical);
    }

    public void OnJump()
    {
        _animator.SetTrigger("OnJump");
    }

    public void OnDadge()
    {
        _animator.SetTrigger("OnDadge");
    }

    public void OnComboAttack()
    {
        _animator.SetTrigger("OnWeaponAttack");
    }

    public void OnAttackCollision()
    {
        _atkCollision.SetActive(true);
    }

    public bool IsMovement()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
        {
            return true;
        }
        
        return false;
    }

    public bool IsDodge()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dodge"))
        {
            return true;
        }

        return false;
    }

    public float GetDodgeTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }
}
