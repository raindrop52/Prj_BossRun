using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    Animator _anim;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void OnHit()
    {
        _anim.SetTrigger("OnHit");
    }

    public void OnSkill1()
    {
        _anim.SetTrigger("OnSkill1");
    }

    public void OnSkill2()
    {
        _anim.SetTrigger("OnSkill2");
    }

    public void OnAttack()
    {
        _anim.SetTrigger("OnAttack");
    }

    public void IsWalk(bool value)
    {
        _anim.SetBool("IsWalk", value);
    }

    public void IsStun(bool value)
    {
        _anim.SetBool("IsStun", value);
    }

    public void PauseAnim(float time)
    {
        _anim.speed = 0f;
        Invoke("PlayAnim", time);
    }

    void PlayAnim()
    {
        _anim.speed = 1f;
    }
}
