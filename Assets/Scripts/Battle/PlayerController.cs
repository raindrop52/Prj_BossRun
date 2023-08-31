using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player_State
{
    IDLE,
    WAIT,
    MOVE,
    ATTACK,
    DIE
}

public class PlayerController : MonoBehaviour
{
    public Player_State _state;
    public float _speed;

    float _hAxis;
    float _vAxis;

    public float _waitTime = 3f;
    float _time;                        // Ű���� ���Է� �ð� üũ ����
    float _runnigTime;                  // �޸��� �ð�

    Vector3 _moveVec;
    Vector3 _dodgeVec;

    bool _isWalk = false;
    bool _isDodge = false;
    bool _isSkill = false;

    bool _modeRun = false;
    bool _modeDodge = false;

    bool _waitDodge = false;            // ȸ�� ��Ÿ�� ���� ����

    public float _atkSpeed = 1f;        // Clamp(0.8 ~ 1.7) 

    public float _dashSpeedRatio = 1.5f;
    public float _dodgeSpeedRatio = 3f;
    public float _dodgeOutTime = 1f;
    public float _dodgeCoolTime = 3f;

    public float _runOffTime = 3f;      // �޸��� �ִϸ��̼� �߰���� �ߵ� �ּ� �ð�

    public bool _isCharging = false;
    public float _chargingTime = 0f;
    public float _maxChargeTIme = 3f;
    public GameObject _chargingFx;
    public GameObject _fullChargFx;

    [SerializeField] bool _lockMove;
    [SerializeField] bool _lockDodge;
    [SerializeField] bool _lockAttack;
    [SerializeField] bool _lockCharge;
    [SerializeField] bool _lockSkill;

    [SerializeField] KeyCode _codeSkill = KeyCode.Q;
    [SerializeField] float _jumpPower = 5f;
    [SerializeField] GameObject _groundChecker;
    [SerializeField] float _rayDist = 0.165f;

    PlayerAnimation _anim;
    Rigidbody _rigid;

    void Awake()
    {
        _anim = GetComponent<PlayerAnimation>();
        _rigid = GetComponent<Rigidbody>();

        _state = Player_State.IDLE;

        HideFx(true);
    }

    void Update()
    {
        GetInput();
        Wait();
        WalkRun();
        if(!_lockMove)
            Move();
        Turn();
        if(!_lockDodge)
            Dodge();
        if(!_lockAttack)
            Attack();
        if(!_lockCharge)
            Charging();
        if (!_lockSkill)
            UseSkill();
    }

    void GetInput()
    {
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");
        _isWalk = Input.GetButtonDown("Walk");
        _isDodge = Input.GetButtonDown("Dodge");
    }
    
    void Wait()
    {
        // �������� ������ ��������, ������ ���ۻ���
        if (_hAxis == 0 && _vAxis == 0)
        {
            // �Է� n�� �̻� ���� ���� ��� ����� ����
            if (_state == Player_State.IDLE)
            {
                _time += Time.deltaTime;

                if (_time >= _waitTime)
                {
                    ChangeState(Player_State.WAIT);
                    _anim.OnTired();
                }
            }
            else
            {
                _time = 0f;
            }
        }
    }

    void Move()
    {
        if (_state == Player_State.ATTACK)
            return;

        _moveVec = new Vector3(_hAxis, 0f, _vAxis).normalized;
        float speed = _speed;

        if (_moveVec != Vector3.zero)        
            ChangeState(Player_State.MOVE);
                
        // ���� Shift ����� ���� �޸���,�ȱ� ��ȯ
        if (_modeRun)
        {
            _runnigTime += Time.deltaTime;
            speed = _speed * _dashSpeedRatio;
        }
        else
            _runnigTime = 0f;

        if (_modeDodge)
        {
            _moveVec = _dodgeVec;
            speed = _speed * _dodgeSpeedRatio;
        }

        transform.position += _moveVec * speed * Time.deltaTime;

        bool isMove = _moveVec != Vector3.zero;
        if(isMove == false)
        {
            if (_runnigTime >= _runOffTime)
                _anim.OnRunAddMotion();

            _runnigTime = 0f;
        }

        _anim.OnMovement(isMove);
    }

    void Turn()
    {
        transform.LookAt(transform.position + _moveVec);
    }

    void WalkRun()
    {
        if (_isWalk)
        {
            _modeRun = !_modeRun;
            _anim.OnRun(_modeRun);
        }
    }

    void Dodge()
    {
        // ���� ��ư�� ������ ���� (�ߺ� ���� ����)
        if(_isDodge && !_modeDodge && !_waitDodge)
        {
            _waitDodge = true;
            _dodgeVec = _moveVec;
            _anim.OnDodge();
            _modeDodge = true;

            Invoke("DodgeOut", _dodgeOutTime);
            Invoke("DodgeCoolTime", _dodgeCoolTime);
        }
    }

    void DodgeOut()
    {
        _modeDodge = false;
    }

    void DodgeCoolTime()
    {
        _waitDodge = false;
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttackState();

            _time = 0f;         // ��� ���ۿ� �������� �ʵ��� �ʱ�ȭ
            _anim.OnAttack();   // ���� �ִϸ��̼� ����
        }
    }

    void Charging()
    {
        // ���콺 ��Ŭ�� �� ����
        if (Input.GetMouseButtonDown(1) && _state == Player_State.IDLE)
        {
            _chargingTime = 0f;
            _anim.OnChargStart();
            _chargingFx.SetActive(true);
        }

        if (_state == Player_State.ATTACK)
        {
            if (Input.GetMouseButton(1) && _isCharging)
            {
                _chargingTime += Time.deltaTime;

                if (_chargingTime >= _maxChargeTIme)
                {
                    _chargingTime = _maxChargeTIme;
                    _chargingFx.SetActive(false);
                    _fullChargFx.SetActive(true);
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                StartCoroutine(MinusChargingTime());
            }
        }
    }

    IEnumerator MinusChargingTime()
    {
        // ��¡ ����Ʈ �����
        HideFx(true);

        // ���� �ð� ����
        _anim.OnCharging(_chargingTime);
        // ���� �߻�
        _anim.OnChargingFire();

        float timer = _chargingTime;

        while(timer > 0)
        {
            timer -= Time.deltaTime;
            
            _anim.OnCharging(timer);
            
            yield return null;
        }
    }

    void HideFx(bool hide)
    {
        _chargingFx.SetActive(!hide);
        _fullChargFx.SetActive(!hide);
    }

    void UseSkill()
    {
        if (Input.GetKeyDown(_codeSkill) && !_isSkill)
        {
            AttackState();
            _isSkill = true;
            _anim.OnSkill();
        }
    }

    IEnumerator CheckGround()
    {
        if(_groundChecker != null)
        {
            _anim.IsGround(false);
            int mask = 1 << LayerMask.NameToLayer("Ground");
            RaycastHit hit;

            yield return new WaitForSeconds(0.1f);

            while(true)
            {
                Debug.DrawRay(_groundChecker.transform.position, Vector3.down * _rayDist, Color.red);
                if (Physics.Raycast(_groundChecker.transform.position, Vector3.down, out hit, _rayDist, mask))
                {
                    _anim.IsGround(true);
                    break;
                }

                yield return null;
            }
        }

        yield return null;
    }

    #region Animation �Լ�
    public void ChangeState(int tag)
    {
        _state = (Player_State)tag;
    }

    public void ChangeState(Player_State state)
    {
        _state = state;
    }

    void AttackState()
    {
        if (_state != Player_State.ATTACK)
            ChangeState(Player_State.ATTACK);
    }

    void StartCharging()
    {
        AttackState();

        if (!_isCharging)
            _isCharging = true;
    }

    void ChargingOut()
    {
        _chargingTime = 0f;
        _isCharging = false;
    }

    void SkillJump()
    {
        _rigid.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);
        StartCoroutine(CheckGround());
    }

    void SkillOut()
    {
        _isSkill = false;
    }
    #endregion
}

