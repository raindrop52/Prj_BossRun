using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Bar : MonoBehaviour
{
    [SerializeField] bool isBounce = false;
    Image bar;

    private void Awake()
    {
        bar = GetComponent<Image>();
        StartCoroutine(MaxBounce());
    }

    public void Init()
    {
        SetBar(1.0f);
    }

    public void SetBar(float ratio)
    {
        if(bar != null)
        {
            bar.fillAmount = ratio;
        }
    }

    IEnumerator MaxBounce()
    {
        if (isBounce)
        {
            yield return new WaitUntil(() => bar.fillAmount == 1);

            while(bar.fillAmount >= 1)
            {
                yield return null;

                bar.transform.DOScale(1.2f, 1f);

                yield return new WaitForSeconds(1f);

                bar.transform.DOScale(1f, 1f);

                yield return new WaitForSeconds(2f);
            }

            bar.transform.DOScale(1f, 0.1f);
        }

        yield return null;
    }
}
