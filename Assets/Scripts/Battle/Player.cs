using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ST_Type
{
    ST_ATK,
    ST_CHARGE,
    ST_CHARGING,
    ST_DODGE
}

public class Player : MonoBehaviour, IDamage
{
    PlayerController pCT;

    [Header("공격력")]
    [SerializeField] int _attackPoint = 5;
    public int AtkPoint { get { return _attackPoint; } }
    [Header("체력")]
    public float _hp;
    public float _hpMax;

    [Header("스태미너")]
    public float _st;
    public float _stMax;
    [SerializeField] float _stPerSec;             // 초당 스태미너 회복량
    [SerializeField] float _stAtk;                // 공격 시 소모 스태미너
    [SerializeField] float _stDodge;              // 회피 시 소모 스태미너
    [SerializeField] float _stCharge;             // 차지 시작 소모 스태미너 (sec)
    [SerializeField] float _stChargingSec;           // 차징 시 소모 스태미너

    [Header("필살기")]
    public float _skGague = 0f;         // 최대값 100
    public float _skGet;                // 공격 시 얻는 값

    void Awake()
    {
        _hp = _hpMax;
        _st = _stMax;

        pCT = GetComponent<PlayerController>();
        StartCoroutine(CheckST());
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

            UIManager.i.ChangeHP((int)Bar_Type.HP_Player, ratio);
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

    public float STTypeToValue(ST_Type type)
    {
        float value = 0f;

        switch (type)
        {
            case ST_Type.ST_ATK:
                {
                    value = _stAtk;
                    break;
                }
            case ST_Type.ST_DODGE:
                {
                    value = _stDodge;
                    break;
                }
            case ST_Type.ST_CHARGE:
                {
                    value = _stCharge;
                    break;
                }
            case ST_Type.ST_CHARGING:
                {
                    value = _stChargingSec * Time.deltaTime;
                    break;
                }
        }

        return value;
    }

    public bool CheckStamina(float value)
    {
        float st = _st - value;
        bool result = st >= 0 ? true : false;
        return result;
    }

    public void UseAtkSt()
    {
        _st = ClampValue(_st, -1 * _stAtk, 0f, _stMax);
        ChangeST();
    }

    public bool UseStamina(ST_Type type)
    {
        float value = STTypeToValue(type);
        
        if (CheckStamina(value))
        {
            _st = ClampValue(_st, -1*value, 0f, _stMax);
            ChangeST();
            return true;
        }
        else
            return false; 
    }

    IEnumerator CheckST()
    {
        while(true)
        {
            if (pCT != null)
            {
                if (pCT._state != Player_State.ATTACK)
                {
                    RestoreStamina();
                    yield return new WaitForSeconds(1f);
                }
            }

            yield return null;
        }
    }

    public void RestoreStamina()
    {
        _st = ClampValue(_st, _stPerSec, 0f, _stMax);
        ChangeST();
    }

    public void GetSKGague()
    {
        _skGague = ClampValue(_skGague, _skGet, 0f, 100f);
        ChangeSK();
    }

    public void UseSKGague()
    {
        _skGague = 0f;
        ChangeSK();
    }

    // value = 얻을 값, cal = +- 연산 값, min max = Clamp 최소 최대값
    float ClampValue(float value, float cal, float min, float max)
    {
        float v = value;
        v += cal;
        v = Mathf.Clamp(v, min, max);

        return v;
    }

    void ChangeST()
    {
        float ratio = _st / _stMax;
        UIManager.i.ChangeST(ratio);
    }

    void ChangeSK()
    {
        float ratio = _skGague / 100f;
        UIManager.i.ChangeSK(ratio);
    }
}
