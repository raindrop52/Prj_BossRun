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
    [Header("Ʃ�丮��")]
    [SerializeField]
    GameObject _goTutorial;    // �������� Ʃ�丮�� ���� ������Ʈ (ǥ��, ��ǥ�� ��)
    [SerializeField]
    GameObject _goInfoPanel;    // Ʃ�丮�� ����� (ǥ��, ��ǥ�� ��)
    [SerializeField]
    Text _textTuto;             // Ʃ�丮���� ������ ����ϴ� �ؽ�Ʈ

    void Awake()
    {
        i = this;

        // ü�¹� �ִ�ġ�� ���� (�Ʊ�, ��)
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
        Debug.Log("�׽�ƮA ����");
        goTest.SetActive(true);
    }

    public void TestB()
    {
        Debug.Log("�׽�ƮB ����");
        string text = testInput.text;
        ChangeText_Tutorial(text);
        goTest.SetActive(false);
    }
    #endregion
}
