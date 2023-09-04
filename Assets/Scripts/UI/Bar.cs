using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    Image bar;

    private void Awake()
    {
        bar = GetComponent<Image>();
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
}
