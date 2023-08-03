using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamage
{
    [SerializeField] int _attackPoint = 5;
    public int AtkPoint { get { return _attackPoint; } }
    public float _hp;
    public float _hpMax;

    void Awake()
    {
        _hp = _hpMax;
    }

    
    void Update()
    {
        
    }

    public void Damage(float point)
    {
        // 체력 감소
        _hp -= point;

        if(UIManager.i  != null)
        {
            float ratio = _hp / _hpMax;

            UIManager.i.ChangeHP((int)HPBar_Type.HP_Plyaer, ratio);
        }

        // 현재 체력이 0보다 작으면 사망 처리
        if (_hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.i._alive = false;
    }
}
