using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_State
{
    READY = 0,
    IDLE,
    WALK,
    ATTACK,
    SKILL1,
    SKILL2,
    WAIT,
    DIE
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

    [Header("스킬")]
    [SerializeField] Fx_Skill1 _skill1;

    [SerializeField] GameObject rangeSkill1;
    [SerializeField] GameObject rangeSkill2;
    GameObject _rangeSkill1;
    GameObject _rangeSkill2;

    void Awake()
    {
        _anim = GetComponent<EnemyAnimation>();
        _enemyController = GetComponent<EnemyController>();

        _hp = _hpMax;
        ChangeHP();

        // 스킬 공격 범위
        CreateRange();

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
                case Enemy_State.READY:
                    {
                        // 타겟 확인 시
                        if (_target != null)
                        {
                            // 걷기 진입
                            _state = Enemy_State.WALK;
                            _anim.IsWalk(true);
                        }
                        // 타겟 미확인 시
                        else
                            SearchPlayer();
                        break;
                    }
                case Enemy_State.IDLE:
                    {
                        _state = Enemy_State.WALK;
                        break;
                    }
                case Enemy_State.WALK:
                    {
                        Move();
                        break;
                    }
                case Enemy_State.ATTACK:
                    {
                        // 공격 쿨타임 체크
                        if (Attack(coolAtk))
                            // 쿨타임 초기화
                            coolAtk = _coolAtk;

                        break;
                    }
                case Enemy_State.SKILL1:
                    {
                        // 대기 상태 전환
                        _state = Enemy_State.WAIT;
                        // 타겟 방향 보기
                        _enemyController.FindTargetRot(_target);
                        // 쿨타임 초기화
                        coolSkill1 = _coolSkill1;
                        // 스킬 사용
                        _anim.OnSkill1();
                        Debug.Log("Skill1");
                        break;
                    }
                case Enemy_State.SKILL2:
                    {
                        // 대기 상태 전환
                        _state = Enemy_State.WAIT;
                        // 타겟 방향 보기
                        _enemyController.FindTargetRot(_target);
                        // 쿨타임 초기화
                        coolSkill2 = _coolSkill2;
                        // 스킬 사용
                        _anim.OnSkill2();
                        Debug.Log("Skill2");
                        break;
                    }
                case Enemy_State.WAIT:
                    {
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

            // 쿨타임 처리부
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

    public void ShowRangeSkill1(bool show)
    {
        _rangeSkill1.SetActive(show);
    }

    public void ShowRangeSkill2(bool show)
    {
        _rangeSkill2.SetActive(show);
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
            _anim.IsWalk(false);
            _state = Enemy_State.ATTACK;
        }

        _enemyController.FollowTarget(_target);
    }

    public Vector3 test;

    bool Attack(float cooltime)
    {
        float dist = Vector3.Distance(transform.position, _target.position);
        if (dist > 2f)
        {
            _state = Enemy_State.WALK;
            _anim.IsWalk(true);
            Debug.Log("추적");
            return false;
        }

        if(cooltime <= 0f)
        {
            _enemyController.FindTargetRot(_target);
            _anim.OnAttack();
            Debug.Log("공격");
            return true;
        }

        return false;
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

    void CreateRange()
    {
        if (rangeSkill1 != null)
        {
            GameObject go = Instantiate(rangeSkill1);
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            go.SetActive(false);
            _rangeSkill1 = go;
        }

        if (rangeSkill2 != null)
        {
            GameObject go = Instantiate(rangeSkill2);
            go.transform.SetParent(transform);
            go.transform.position = Vector3.zero;
            go.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
            go.SetActive(false);
            _rangeSkill2 = go;
        }
    }

    #region Animation Script
    public void OffSkill()
    {
        _state = Enemy_State.IDLE;
    }

    public void Skill1_Range(int value = 0)
    {
        bool show = false;

        if (value == 1)
            show = true;

        ShowRangeSkill1(show);
    }

    public void Skill1_Fire()
    {
        _anim.PauseAnim(0.75f);
        // 범위 없애줌
        Skill1_Range();
        // 이펙트 표시
        _skill1.Play();
    }
    #endregion
}
