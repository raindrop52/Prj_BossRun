using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    SkinnedMeshRenderer _meshRender;
    Color               _originColor;
    NavMeshAgent agent;
    [SerializeField] float speed = 0f;

    void Awake()
    {
        _meshRender = GetComponentInChildren<SkinnedMeshRenderer>();
        if(_meshRender != null)
            _originColor = _meshRender.material.color;

        agent = GetComponent<NavMeshAgent>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 7f);
    }

    public void OnSpeed(bool on)
    {
        if (on)
            agent.speed = speed;
        else
            agent.speed = 0f;
    }

    public void FollowTarget(Transform target)
    {
        agent.SetDestination(target.position);
    }

    public void HitEvent(float time)
    {
        StartCoroutine(OnHitColor(time));
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
}
