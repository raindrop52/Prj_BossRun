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
        // 메쉬의 색을 빨간색으로 변경
        _meshRender.material.color = Color.red;
        // 0.1초 대기
        yield return new WaitForSeconds(_hitTime);
        // 원래의 색으로 변경
        _meshRender.material.color = _originColor;
    }

    // IDamage Interface 구현
    public void Damage(int hitPoint)
    {
        // 체력 감소
        Debug.Log(hitPoint + "피해를 입음");
        // 피격 애니메이션 재생
        _anim.OnHit();
        // 색상 변경
        StartCoroutine("OnHitColor");
    }
}
