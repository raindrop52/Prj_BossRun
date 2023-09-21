using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_State
{
    IDLE,
    WALK,
    ATTACK,
    SKILL1,
    SKILL2,
}

public class Enemy : MonoBehaviour, IDamage
{
    EnemyAnimation _anim;
    EnemyController _enemyController;
    [SerializeField] Enemy_State _state;

    public float _hp;
    public float _hpMax;
    public float _hitTime = 0.1f;
    bool _live = true;

    [SerializeField] float _searchRad = 7f;
    [SerializeField] Transform _target;

    [SerializeField] float _coolAtk = 0f;
    [SerializeField] float _coolSkill1 = 0f;
    [SerializeField] float _coolSkill2 = 0f;

    void Awake()
    {
        _anim = GetComponent<EnemyAnimation>();
        _enemyController = GetComponent<EnemyController>();

        _hp = _hpMax;
        ChangeHP();

        StartCoroutine(Enemy_AI());
    }

    IEnumerator Enemy_AI()
    {
        float coolAtk = 0f;
        float coolSkill1 = 0f;
        float coolSkill2 = 0f;

        while (_live)
        {
            float hpPer = _hp / _hpMax * 100f;

            switch (_state)
            {
                case Enemy_State.IDLE:
                    {
                        // 타겟 확인 시
                        if (_target != null)
                        {
                            // 걷기 진입
                            _state = Enemy_State.WALK;
                            _anim.IsWalk(true);
                            _enemyController.OnSpeed(true);
                        }
                        // 타겟 미확인 시
                        else
                            SearchPlayer();
                        break;
                    }
                case Enemy_State.WALK:
                    {
                        Move();
                        break;
                    }
                case Enemy_State.ATTACK:
                    {
                        if (Attack(coolAtk))
                            coolAtk = _coolAtk;

                        break;
                    }
                case Enemy_State.SKILL1:
                    {
                        coolSkill1 = _coolSkill1;
                        Skill1();
                        break;
                    }
                case Enemy_State.SKILL2:
                    {
                        coolSkill2 = _coolSkill2;
                        Skill2();
                        break;
                    }
            }

            if (hpPer <= 70f && coolSkill1 <= 0f)
            {
                _state = Enemy_State.SKILL1;
            }

            if (hpPer <= 35f && coolSkill2 <= 0f && _state != Enemy_State.SKILL1)
            {
                _state = Enemy_State.SKILL2;
            }

            if (coolAtk > 0f)
            {
                coolAtk -= Time.deltaTime;
                if (coolAtk < 0f)
                    coolAtk = 0f;
            }

            if (coolSkill1 > 0f && StateCheckerSign(Enemy_State.SKILL1))
            {
                coolSkill1 -= Time.deltaTime;
                if (coolSkill1 < 0f)
                    coolSkill1 = 0f;
            }

            if (coolSkill2 > 0f && StateCheckerSign(Enemy_State.SKILL2))
            {
                coolSkill2 -= Time.deltaTime;
                if (coolSkill2 < 0f)
                    coolSkill2 = 0f;
            }

            yield return null;
        }
    }

    // IDamage Interface 구현
    public void Damage(float hitPoint)
    {
        // 체력 감소
        _hp -= hitPoint;
        // 체력바 갱신
        ChangeHP();
        // 피격 애니메이션 재생
        //_anim.OnHit();
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

    void SearchPlayer()
    {
        // 무시되는 대상
        LayerMask mask = LayerMask.NameToLayer("Ground");
        Collider[] cols = Physics.OverlapSphere(transform.position, _searchRad, mask);

        foreach (Collider col in cols)
        {
            if (col.CompareTag("Player"))
            {
                _target = col.transform;
                break;
            }
        }
    }

    void Move()
    {
        float dist = Vector3.Distance(transform.position, _target.position);
        if (dist <= 2f)
        {
            _enemyController.OnSpeed(false);
            _anim.IsWalk(false);
            _state = Enemy_State.ATTACK;
        }
        
        _enemyController.FollowTarget(_target);
    }

    public Vector3 test;

    bool Attack(float cooltime)
    {
        float dsX = transform.position.x - _target.position.x;
        float dsZ = transform.position.z - _target.position.z;
        float dsXZ = Mathf.Sqrt(Mathf.Pow(dsX, 2) * Mathf.Pow(dsZ, 2));
        test = new Vector3(dsX, dsXZ, dsZ);
        float dist = Vector3.Distance(transform.position, _target.position);
        if (dist > 2f)
        {
            _state = Enemy_State.WALK;
            _enemyController.OnSpeed(true);
            _anim.IsWalk(true);
            Debug.Log("추적");
            return false;
        }

        if(cooltime <= 0f)
        {
            _anim.OnAttack();
            Debug.Log("공격");
            return true;
        }

        return false;
    }

    void Skill1()
    {
        _enemyController.OnSpeed(false);
        _anim.OnSkill1();
        _enemyController.OnSpeed(true);
    }

    void Skill2()
    {
        _enemyController.OnSpeed(false);
        _anim.OnSkill2();
        _enemyController.OnSpeed(true);
    }

    // state 값이 현재 상태와 같으면 true 다르다면 false
    bool StateChecker(Enemy_State state)
    {
        if (_state == state)
            return true;
        else
            return false;
    }

    // state 값이 현재 상태보다 작으면 true 다르다면 false
    bool StateCheckerSign(Enemy_State state)
    {
        if (_state < state)
            return true;
        else
            return false;
    }

    #region Animation Script
    public void OffSkill()
    {
        _state = Enemy_State.IDLE;
    }
    #endregion
}
