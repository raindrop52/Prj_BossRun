using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public  enum Bar_Type
{
    HP_Player,
    HP_Boss
}

public class UIManager : MonoBehaviour
{
    public static UIManager i;
    [SerializeField] // 0 : Player, 1 : Boss
    List<Bar> _hpBars;
    [SerializeField]
    Bar _stBar;
    [SerializeField]
    Bar _skBar;
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

        // 체력바 최대치로 충전 (아군, 적)
        foreach(Bar bar in _hpBars)
        {
            bar.SetBar(1.0f);
        }

        _stBar.SetBar(1.0f);
        _skBar.SetBar(0f);
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
        _hpBars[index].SetBar(ratio);
    }

    public void ChangeST(float ratio)
    {
        _stBar.SetBar(ratio);
    }

    public void ChangeSK(float ratio)
    {
        _skBar.SetBar(ratio);
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
