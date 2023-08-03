using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    Image hpBar;

    private void Awake()
    {
        hpBar = GetComponent<Image>();
    }

    public void Init()
    {
        SetHpBar(1.0f);
    }

    public void SetHpBar(float ratio)
    {
        if(hpBar != null)
        {
            hpBar.fillAmount = ratio;
        }
    }
}
