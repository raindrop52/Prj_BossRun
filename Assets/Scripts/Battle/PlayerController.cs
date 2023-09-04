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
    Player player;
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
    bool _isSkill = false;

    bool _modeWalk = false;
    bool _modeDodge = false;

    bool _waitDodge = false;            // 회피 쿨타임 제어 변수

    public float _atkSpeed = 1f;        // Clamp(0.8 ~ 1.7)
    public float _atkDelayTime = 0.5f;  // 공격 딜레이 시간
    bool _atkDelay = false;             // 공격 딜레이

    public float _walkSpeedRatio = 1.5f;
    public float _dodgeSpeedRatio = 3f;
    public float _dodgeOutTime = 1f;
    public float _dodgeCoolTime = 3f;

    public float _runOffTime = 3f;      // 달리기 애니메이션 추가모션 발동 최소 시간

    public bool _isCharging = false;        // 애니메이션 차지 동작 변수
    public bool _isScrCharging = false;      // 스크립트 차지 동작 변수
    public bool _forceChargeStart = false;   // 강제 시작
    public bool _maxCharging = false;
    public float _chargingTime = 0f;
    public float _maxChargeTIme = 3f;
    [SerializeField] bool _isRunningCharging = false;         // 차지 코루틴 체크

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
        player = GetComponent<Player>();

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
        if (_modeWalk)
        {
            _runnigTime += Time.deltaTime;
            speed = _speed * _walkSpeedRatio;
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
            _modeWalk = !_modeWalk;
            _anim.OnWalk(_modeWalk);
        }
    }

    void Dodge()
    {
        // 닷지 버튼이 눌리면 동작 (중복 동작 방지)
        if(_isDodge && !_modeDodge && !_waitDodge)
        {
            //스태미나 소모
            if (!player.UseStamina(ST_Type.ST_DODGE))
                return;

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
            if (!_atkDelay)
            {
                float value = player.STTypeToValue(ST_Type.ST_ATK);
                if (player.CheckStamina(value))
                {
                    StartCoroutine(ATKDelay());
                    Debug.Log("공격 : 스태미너 ("+ player._st +")");
                }
            }
        }
    }

    public void UseStAtk()
    {
        player.UseAtkSt();
    }

    IEnumerator ATKDelay()
    {
        _atkDelay = true;
        AttackState();

        _time = 0f;         // 대기 동작에 진입하지 않도록 초기화
        _anim.OnAttack();   // 공격 애니메이션 동작

        yield return new WaitForSeconds(_atkDelayTime);

        _atkDelay = false;
    }

    void Charging()
    {
        // 마우스 우클릭 시 차지
        if (Input.GetMouseButtonDown(1) && _state == Player_State.IDLE)
        {
            if (!player.UseStamina(ST_Type.ST_CHARGE))
                return;

            _chargingTime = 0f;                 // 차지시간 초기화
            _anim.OnChargStart();               // 차징 애니메이션 동작
            _isScrCharging = true;              // 스크립트 차징 true
            _chargingFx.SetActive(true);        // 차지 Fx 온
        }        

        // 스크립트 차징이 true일때만 동작 ( 이중 동작을 방지하기 위함 )
        if (_state == Player_State.ATTACK && _isScrCharging)
        {
            if (Input.GetMouseButton(1))
            {
                if (!_maxCharging)
                {
                    // 차지가 최대치면 스태미너 소모를 하지 않도록하고
                    // 유지할 스태미너가 없으면 강제로 발동
                    if (!player.UseStamina(ST_Type.ST_CHARGING))
                    {
                        _forceChargeStart = true;
                        Debug.Log("강제 발동");
                        if(!_isRunningCharging)
                            StartCoroutine(MinusChargingTime());

                        return;
                    }
                }

                _chargingTime += Time.deltaTime;

                if (_chargingTime >= _maxChargeTIme)
                {
                    _maxCharging = true;
                    _chargingTime = _maxChargeTIme;
                    _chargingFx.SetActive(false);
                    _fullChargFx.SetActive(true);
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                Debug.Log("우클릭");
                if (!_isRunningCharging)
                    StartCoroutine(MinusChargingTime());
            }
        }
    }

    IEnumerator MinusChargingTime()
    {
        _isRunningCharging = true;
        _isScrCharging = false;             // 스크립트 차징을 바로 false 하여 1회만 동작

        // 차징 이펙트 숨기기
        HideFx(true);

        // 차지 시간 세팅
        _anim.OnCharging(_chargingTime);
        // 차지 발사
        _anim.OnChargingFire();

        float timer = _chargingTime;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            _anim.OnCharging(timer);

            yield return null;
        }

        _maxCharging = false;

        Debug.Log("동작");

        yield return null;

        _isRunningCharging = false;
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

    void AttackState()
    {
        if (_state != Player_State.ATTACK)
            ChangeState(Player_State.ATTACK);
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
        AttackState();

        if (!_isCharging)
            _isCharging = true;
    }

    void ChargingOut()
    {
        _chargingTime = 0f;
        if (_forceChargeStart)
            _forceChargeStart = false;
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

