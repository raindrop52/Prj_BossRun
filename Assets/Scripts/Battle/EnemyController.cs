using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    SkinnedMeshRenderer _meshRender;
    Color               _originColor;
    [SerializeField] float speed = 0f;

    void Awake()
    {
        _meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
        if(_meshRender != null)
            _originColor = _meshRender.material.color;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, 7f);
    //}

    public void FindTargetRot(Transform target)
    {
        // ���� ���� = ��ǥ ���� - ���� ����
        Vector3 dir = target.position - transform.position;
        dir = new Vector3(dir.x, 0f, dir.z);
        Quaternion rot = Quaternion.LookRotation(dir.normalized);
        transform.rotation = rot;
    }

    public void FollowTarget(Transform target)
    {
        FindTargetRot(target);
        
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void HitEvent(float time)
    {
        StartCoroutine(OnHitColor(time));
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
}
