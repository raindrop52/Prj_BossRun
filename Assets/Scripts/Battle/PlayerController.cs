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
    float _time;                        // 키보드 미입력 시간 체크 변수
    float _runnigTime;                  // 달리기 시간

    Vector3 _moveVec;
    Vector3 _dodgeVec;

    bool _isWalk = false;
    bool _isDodge = false;

    bool _modeRun = false;
    bool _modeDodge = false;

    bool _waitDodge = false;            // 회피 쿨타임 제어 변수

    public float _dashSpeedRatio = 1.5f;
    public float _dodgeSpeedRatio = 3f;
    public float _dodgeOutTime = 1f;
    public float _dodgeCoolTime = 3f;

    public float _runOffTime = 3f;      // 달리기 애니메이션 추가모션 발동 최소 시간

    public bool _isCharging = false;
    public float _chargingTime = 0f;
    public float _maxChargeTIme = 3f;
    public GameObject _chargingFx;
    public GameObject _fullChargFx;

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
        Move();
        Walk();
        Turn();
        Dodge();
        Attack();
        Charging();
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
        // 움직임이 없으면 정지상태, 있으면 동작상태
        if (_hAxis == 0 && _vAxis == 0)
        {
            // 입력 n초 이상 반응 없는 경우 대기모션 동작
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
                
        // 왼쪽 Shift 토글을 통해 달리기,걷기 변환
        if (_modeRun)
        {
            _runnigTime += Time.deltaTime;
            speed = _speed * 3f;
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

    void Walk()
    {
        if (_isWalk)
        {
            _modeRun = !_modeRun;
            _anim.OnRun(_modeRun);
        }
    }

    void Dodge()
    {
        // 닷지 버튼이 눌리면 동작 (중복 동작 방지)
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
            if(_state != Player_State.ATTACK)
                ChangeState(Player_State.ATTACK);

            _time = 0f;         // 대기 동작에 진입하지 않도록 초기화
            _anim.OnAttack();   // 공격 애니메이션 동작
        }
    }

    void Charging()
    {
        // 마우스 우클릭 시 차지
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
        // 차징 이펙트 숨기기
        HideFx(true);

        // 차지 시간 세팅
        _anim.OnCharging(_chargingTime);
        // 차지 발사
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

    #region Animation 함수
    public void ChangeState(int tag)
    {
        _state = (Player_State)tag;
    }

    public void ChangeState(Player_State state)
    {
        _state = state;
    }

    void StartCharging()
    {
        if (_state != Player_State.ATTACK)
            ChangeState(Player_State.ATTACK);

        if (!_isCharging)
            _isCharging = true;
    }

    void ChargingOut()
    {
        _chargingTime = 0f;
        _isCharging = false;
    }
    #endregion
}

