using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_CharacterController : MonoBehaviour, IDamage
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
    float _groundDistance = 0.2f;           // ground üũ Ray ����
    [SerializeField]
    LayerMask _ground;

    PlayerAnimation _anim;
    Rigidbody _rigid;


    Vector3 _moveVec;
    Vector3 _dodgeVec;
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

    #region Camera
    void LookAround()
    {
        // ���콺 �̵� ��
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = _cameraArm.rotation.eulerAngles;

        // y�� ī�޶� ���� ����
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 60f);   // �ּҸ� 0���� ������ ����� �Ʒ��� �������� �ʴ� ����
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
        // �̵� �Է�
        Vector2 moveInput = new Vector2(hAxis, vAxis);

        // ���� ���� üũ ( ���� ��� �� �̵� �Ұ� )
        if (!_anim.IsMovement() && !_anim.IsDodge())
            moveInput = Vector2.zero;
        
        // �̵� ���� üũ (moveInput�� ���̰� 0�� �ƴϸ� �̵� �Է� �߻�)
        bool isMove = moveInput.magnitude != 0;
        _anim.OnMovement(moveInput.x, moveInput.y);

        if (isMove)
        {
            float moveSpeed = moveInput.y > 0 ? _moveSpeed : _moveSpeed / 2;

            // �뽬 ( �ް��� �� ĥ���� ���� �Ұ� )
            if (Input.GetKey(KeyCode.LeftShift) && moveInput.y > 0)
            {
                moveSpeed = moveSpeed * _runRatio;
            }
            // ȸ��
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dodge();
            }

            Vector3 lookForward = new Vector3(_cameraArm.forward.x, 0f, _cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(_cameraArm.right.x, 0f, _cameraArm.right.z).normalized;
            // �ٶ󺸰� �ִ� ������ �������� �̵����� ����
            _moveVec = lookForward * moveInput.y + lookRight * moveInput.x;

            if (_isDodge)
            {
                _moveVec = _dodgeVec;
                _characterBody.forward = new Vector3(_dodgeVec.x, 0f, _dodgeVec.z);
                moveSpeed *= 2;
            }
            else
                _characterBody.forward = lookForward;

            dir = _characterBody.forward;

            transform.position += _moveVec * Time.deltaTime * moveSpeed;
        }
    }

    void Jump()
    {
        bool isGround = GroundChecker();
        if(isGround)
        {
            // ����
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
        // ���� ��ġ���� �Ʒ� �������� Ray�� ���� ground ���̾ ã��
        if (Physics.Raycast(_groundChecker.position, Vector3.down, out hit, _groundDistance, _ground))
        {
            return true;
        }

        return false;
    }

    void Dodge()
    {
        _dodgeVec = new Vector3(hAxis, 0, vAxis).normalized;
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

    public void Damage(int hitPoint)
    {
        // ü�� ����
        Debug.Log(hitPoint + "���ظ� ����");
    }
    #endregion
}
