using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public  enum HPBar_Type
{
    HP_Plyaer,
    HP_Boss
}

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    [SerializeField] // 0 : Player, 1 : Boss
    List<HpBar> _hpBars;
    [Header("튜토리얼")]
    [SerializeField]
    GameObject _goTutorial;    // 전반적인 튜토리얼 게임 오브젝트 (표시, 비표시 용)
    [SerializeField]
    GameObject _goInfoPanel;    // 튜토리얼 설명부 (표시, 비표시 용)
    [SerializeField]
    Text _textTuto;             // 튜토리얼의 설명을 담당하는 텍스트

    void Awake()
    {
        i = this;

        // 체력바 최대치로 충전
        foreach(HpBar bar in _hpBars)
        {
            bar.SetHpBar(1.0f);
        }
    }

    void Update()
    {
        
    }

    public void ShowTutorial(bool show)
    {
        _goTutorial.SetActive(show);
    }

    public void ChangeHP(int index, float ratio)
    {
        _hpBars[index].SetHpBar(ratio);
    }

    public void ChangeText_Tutorial(string text)
    {
        _textTuto.text = "";
        _textTuto.DOText(text, text.Length * 0.25f);
        //_textTuto.text = text;
    }

    #region Test
    [SerializeField] GameObject goTest;
    [SerializeField] InputField testInput;

    public void TestA()
    {
        Debug.Log("테스트A 동작");
        goTest.SetActive(true);
    }

    public void TestB()
    {
        Debug.Log("테스트B 동작");
        string text = testInput.text;
        ChangeText_Tutorial(text);
        goTest.SetActive(false);
    }
    #endregion
}
