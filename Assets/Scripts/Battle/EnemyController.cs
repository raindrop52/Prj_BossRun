using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    SkinnedMeshRenderer _meshRender;
    Color               _originColor;

    void Awake()
    {
        _meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
        if(_meshRender != null)
            _originColor = _meshRender.material.color;
    }

    IEnumerator OnHitColor(float time)
    {
        // �޽��� ���� ���������� ����
        _meshRender.material.color = Color.red;
        // 0.1�� ���
        yield return new WaitForSeconds(time);
        // ������ ������ ����
        _meshRender.material.color = _originColor;
    }

    public void HitEvent(float time)
    {
        StartCoroutine(OnHitColor(time));
    }
}
