using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ST_Type
{
    ST_ATK,
    ST_CHARGE,
    ST_CHARGING,
    ST_DODGE,
}

public class Player : MonoBehaviour, IDamage
{
    PlayerController pCT;

    [Header("���ݷ�")]
    [SerializeField] int _attackPoint = 5;
    public int AtkPoint { get { return _attackPoint; } }
    [Header("ü��")]
    public float _hp;
    public float _hpMax;

    [Header("���¹̳�")]
    public float _st;
    public float _stMax;
    [SerializeField] float stPerSec;             // �ʴ� ���¹̳� ȸ����
    [SerializeField] float stAtk;                // ���� �� �Ҹ� ���¹̳�
    [SerializeField] float stDodge;              // ȸ�� �� �Ҹ� ���¹̳�
    [SerializeField] float stCharge;             // ���� ���� �Ҹ� ���¹̳� (sec)
    [SerializeField] float stChargingSec;        // ��¡ �� �Ҹ� ���¹̳�

    [Header("�ʻ��")]
    public float _skGague = 0f;                 // �ִ밪 100
    [SerializeField] float createOffset;
    [SerializeField] GameObject goGague;
    [SerializeField] float gagueCoolTime;
    bool isGagueCool = false;

    void Awake()
    {
        Init();

        pCT = GetComponent<PlayerController>();
        StartCoroutine(CheckST());
    }

    public void Init()
    {
        _hp = _hpMax;
        _st = _stMax;
        _skGague = 0f;

        ChangeHP();
        ChangeST();
        ChangeSK();
    }

    public void Damage(float point)
    {
        // ü�� ����
        _hp -= point;

        if(UIManager.i  != null)
        {
            ChangeHP();
        }

        // ���� ü���� 0���� ������ ��� ó��
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
                    value = stAtk;
                    break;
                }
            case ST_Type.ST_DODGE:
                {
                    value = stDodge;
                    break;
                }
            case ST_Type.ST_CHARGE:
                {
                    value = stCharge;
                    break;
                }
            case ST_Type.ST_CHARGING:
                {
                    value = stChargingSec * Time.deltaTime;
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
        _st = ClampValue(_st, -1 * stAtk, 0f, _stMax);
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
        _st = ClampValue(_st, stPerSec, 0f, _stMax);
        ChangeST();
    }

    public void GetSKGague(float value)
    {
        _skGague = ClampValue(_skGague, value, 0f, 100f);
        ChangeSK();
    }

    public bool UseSKGague()
    {
        if (_skGague == 100f)
        {
            _skGague = 0f;
            ChangeSK();
            return true;
        }
        
        return false;
    }

    public void CreateSKGague(Transform target)
    {
        if (isGagueCool == true)
            return;

        StartCoroutine(CoolTimeGague());

        float randX = Random.Range(-1f, 1f);
        float randZ = Random.Range(-1f, 1f);
        Vector3 offset = new Vector3(createOffset * randX, 0.8f, createOffset * randZ);
        
        GameObject go = Instantiate(goGague);
        go.transform.position = target.position + offset;
    }

    IEnumerator CoolTimeGague()
    {
        isGagueCool = true;

        yield return new WaitForSeconds(gagueCoolTime);

        isGagueCool = false;
    }

    // value = ���� ��, cal = +- ���� ��, min max = Clamp �ּ� �ִ밪
    float ClampValue(float value, float cal, float min, float max)
    {
        float v = value;
        v += cal;
        v = Mathf.Clamp(v, min, max);

        return v;
    }

    void ChangeHP()
    {
        float ratio = _hp / _hpMax;
        UIManager.i.ChangeHP((int)Bar_Type.HP_Player, ratio);
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
