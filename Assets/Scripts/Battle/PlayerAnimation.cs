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

    public void OnWaitPose()
    {
        _animator.SetTrigger("WaitPose");
    }

    public void OnMovement(bool move)
    {
        _animator.SetBool("IsMove", move);
    }

    public void OnMovement(float horizontal, float vertical)
    {
        _animator.SetFloat("Horizontal", horizontal);
        _animator.SetFloat("Vertical", vertical);
    }

    public void OnDash(bool dash)
    {
        _animator.SetBool("IsDash", dash);
    }

    public void OnJump()
    {
        _animator.SetTrigger("OnJump");
    }

    public void OnDodge()
    {
        _animator.SetTrigger("OnDodge");
    }

    public void OnComboAttack()
    {
        _animator.SetTrigger("OnWeaponAttack");
    }

    public void OnAttackCollision()
    {
        _atkCollision.Show();
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
