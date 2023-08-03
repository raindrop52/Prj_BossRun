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

        // ü�¹� �ִ�ġ�� ����
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
