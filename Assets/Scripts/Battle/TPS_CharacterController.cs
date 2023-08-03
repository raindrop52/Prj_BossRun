using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_CharacterController : MonoBehaviour
{
    [SerializeField]
    Transform _characterBody;
    [SerializeField]
    Transform _cameraArm;
    [SerializeField]
    Transform _groundChecker;

    [SerializeField]
    float _moveSpeed = 5f;
    [SerializeField]
    float _runRatio = 1.5f;
    [SerializeField]
    float _jumpPower = 9.8f;
    //Ground Check
    [SerializeField]
    float _groundDistance = 0.2f;           // ground 체크 Ray 길이
    [SerializeField]
    LayerMask _ground;

    PlayerAnimation _anim;
    Rigidbody _rigid;


    [SerializeField] Vector3 _moveVec;
    [SerializeField] Vector3 _dodgeVec;
    float hAxis;
    float vAxis;

    bool _isDodge = false;


    void Awake()
    {
        _anim = _characterBody.GetComponent<PlayerAnimation>();
        _rigid = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        GetInput();
        LookAround();
        Move();
        Jump();
        ComboAttack();
    }

    [SerializeField] Vector2 testVec2;
    #region Camera
    void LookAround()
    {
        // 마우스 이동 값
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;

        testVec2 = mouseDelta;

        // y축 카메라 각도 제한
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 60f);   // 최소를 0도로 잡으면 수평면 아래로 내려가지 않는 버그
        else
            x = Mathf.Clamp(x, 330f, 361f);

        _cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
    #endregion

    #region Body
    void GetInput()
    {
        hAxis = Input.GetAxis("Horizontal");
        vAxis = Input.GetAxis("Vertical");
    }

    public Vector3 dir;
    void Move()
    {
        // 이동 입력
        Vector2 moveInput = new Vector2(hAxis, vAxis);

        // 공격 유무 체크 ( 공격 모션 중 이동 불가 )
        if (!_anim.IsMovement() && !_anim.IsDodge())
            moveInput = Vector2.zero;
        
        // 이동 여부 체크 (moveInput의 길이가 0이 아니면 이동 입력 발생)
        bool isMove = moveInput.magnitude != 0;
        _anim.OnMovement(moveInput.x, moveInput.y);

        if (isMove)
        {
            float moveSpeed = moveInput.y > 0 ? _moveSpeed : _moveSpeed / 2;

            // 대쉬 ( 뒷걸음 질 칠때는 동작 불가 )
            if (Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0)
            {
                moveSpeed = moveSpeed * _runRatio;
            }
            // 회피
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dodge();
            }

            _moveVec = GetHeadDir(moveInput.x, moveInput.y);

            if (_isDodge)
            {
                _moveVec = _dodgeVec;
                _characterBody.forward = new Vector3(_dodgeVec.x, 0f, _dodgeVec.z);
                moveSpeed *= 2;
            }

            dir = _characterBody.forward;

            transform.position += _moveVec * Time.deltaTime * moveSpeed;
        }
    }

    Vector3 GetHeadDir(float h, float v)
    {
        Vector3 dir = Vector3.zero;

        // 정면이동
        Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
        // 측면이동
        Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
        // 바라보고 있는 방향을 기준으로 이동방향 측정
        dir = lookForward * v + lookRight * h;

        _characterBody.forward = lookForward;

        return dir;
    }

    void Jump()
    {
        bool isGround = GroundChecker();
        if(isGround)
        {
            // 점프
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                Vector3 jumpVel = Vector3.up * _jumpPower;
                //Vector3 jumpVel = Vector3.up * Mathf.Sqrt(_jumpPower * -Physics.gravity.y);
                _rigid.AddForce(jumpVel, ForceMode.Impulse);
                _anim.OnJump();
            }
        }
    }

    bool GroundChecker()
    {
        RaycastHit hit;
        Debug.DrawRay(_groundChecker.position, Vector3.down * _groundDistance, Color.red);
        // 현재 위치에서 아래 방향으로 Ray를 쏴서 ground 레이어를 찾음
        if (Physics.Raycast(_groundChecker.position, Vector3.down, out hit, _groundDistance, _ground))
        {
            return true;
        }

        return false;
    }

    void Dodge()
    {
        _dodgeVec = GetHeadDir(hAxis, vAxis);
        _isDodge = true;
        _anim.OnDadge();
        float time = _anim.GetDodgeTime();

        Invoke("DodgeOut", time);
    }

    void DodgeOut()
    {
        _isDodge = false;
    }

    void ComboAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _anim.OnComboAttack();
        }
    }
    #endregion
}
