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
    float _time;             // ���Է� �ð� üũ ����

    Vector3 _moveVec;
    Vector3 _dodgeVec;

    bool _isWalk = false;
    bool _isDodge = false;

    bool _modeDash = false;
    bool _modeDodge = false;

    public float _dashSpeedRatio = 1.5f;
    public float _dodgeSpeedRatio = 3f;
    public float _dodgeOutTime = 1f;

    PlayerAnimation _anim;
    Rigidbody _rigid;

    void Awake()
    {
        _anim = GetComponent<PlayerAnimation>();
        _rigid = GetComponent<Rigidbody>();

        _state = Player_State.IDLE;
    }

    void Update()
    {
        GetInput();
        Wait();
        Move();
        Walk();
        Turn();
        Dodge();        
    }

    public void ChangeState(int tag)
    {
        _state = (Player_State)tag;
    }

    public void ChangeState(Player_State state)
    {
        _state = state;
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
                    _state = Player_State.WAIT;
                    _anim.OnWaitPose();
                }
            }
            else
            {
                _time = 0f;
                // ��� ���¿��� �ʱ�ȭ �ϴ� ���� �����ϱ� ����
                if (_state != Player_State.WAIT)
                    ChangeState(Player_State.IDLE);
            }
        }
    }

    void Move()
    {
        _moveVec = new Vector3(_hAxis, 0f, _vAxis).normalized;
        float speed = _speed;

        if (_moveVec != Vector3.zero)
            ChangeState(Player_State.MOVE);

        _anim.OnMovement(true);

        // ���� Shift ����� ���� �޸���,�ȱ� ��ȯ
        if (_modeDash)
            speed = _speed * 2;

        if (_modeDodge)
            _moveVec = _dodgeVec;

        transform.position += _moveVec * _speed * Time.deltaTime;

        bool isMove = _moveVec != Vector3.zero;
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
            _modeDash = !_modeDash;
            _anim.OnDash(_modeDash);
        }
    }

    void Dodge()
    {
        // ���� ��ư�� ������ ���� (�ߺ� ���� ����)
        if(_isDodge && !_modeDodge)
        {
            _dodgeVec = _moveVec;
            _speed *= _dodgeSpeedRatio;
            _anim.OnDodge();
            _modeDodge = true;

            Invoke("DodgeOut", _dodgeOutTime);
        }
    }

    void DodgeOut()
    {
        _speed *= (1f / _dodgeSpeedRatio);
        _modeDodge = false;
    }
}

