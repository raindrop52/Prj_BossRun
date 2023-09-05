using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Skill_Gague : MonoBehaviour
{
    [SerializeField] float value;
    [SerializeField] float duration;
    [SerializeField] Ease ease;
    Sequence sequence;

    void Start()
    {
        sequence = DOTween.Sequence()
                .Append(transform.DOScale(0.0f, duration))
                .PrependInterval(2f)
                .OnComplete(() =>
                {
                    EventComplete();
                });
    }

    void EventComplete()
    {
        Debug.Log("동작완료");
        // 플레이어에게 value값 전달
        GameManager.i._player.GetSKGague(value);

        //종료 시 오브젝트 파괴
        Destroy(gameObject);
    }
}
