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
        // 메쉬의 색을 빨간색으로 변경
        _meshRender.material.color = Color.red;
        // 0.1초 대기
        yield return new WaitForSeconds(time);
        // 원래의 색으로 변경
        _meshRender.material.color = _originColor;
    }

    public void HitEvent(float time)
    {
        StartCoroutine(OnHitColor(time));
    }
}
