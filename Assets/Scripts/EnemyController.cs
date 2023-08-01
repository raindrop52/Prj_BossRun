using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamage
{
    EnemyAnimation      _anim;
    SkinnedMeshRenderer _meshRender;
    Color               _originColor;

    public int _hp;
    public int _hpMax;
    public float _hitTime = 0.1f;

    void Awake()
    {
        _anim = GetComponent<EnemyAnimation>();
        _meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
        if(_meshRender != null)
            _originColor = _meshRender.material.color;

        _hp = _hpMax;
    }

    IEnumerator OnHitColor()
    {
        // �޽��� ���� ���������� ����
        _meshRender.material.color = Color.red;
        // 0.1�� ���
        yield return new WaitForSeconds(_hitTime);
        // ������ ������ ����
        _meshRender.material.color = _originColor;
    }

    // IDamage Interface ����
    public void Damage(int hitPoint)
    {
        // ü�� ����
        Debug.Log(hitPoint + "���ظ� ����");
        // �ǰ� �ִϸ��̼� ���
        _anim.OnHit();
        // ���� ����
        StartCoroutine("OnHitColor");
    }
}
