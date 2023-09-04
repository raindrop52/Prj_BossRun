using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    EnemyAnimation _anim;
    EnemyController _enemyController;

    public float _hp;
    public float _hpMax;
    public float _hitTime = 0.1f;

    void Awake()
    {
        _anim = GetComponent<EnemyAnimation>();
        _enemyController = GetComponent<EnemyController>();
        _hp = _hpMax;
        ChangeHP();
    }

    // IDamage Interface 구현
    public void Damage(float hitPoint)
    {
        // 체력 감소
        _hp -= hitPoint;
        // 체력바 갱신
        ChangeHP();
        // 피격 애니메이션 재생
        _anim.OnHit();
        // 색상 변경
        _enemyController.HitEvent(_hitTime);
    }

    void ChangeHP()
    {
        if (UIManager.i != null)
        {
            float ratio = _hp / _hpMax;
            UIManager.i.ChangeHP((int)Bar_Type.HP_Boss, ratio);
        }
    }
}
