using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    [Header("튜토리얼")]
    public bool _isTutorial = false;

    [Header("플레이어")]
    public Player _player;
    public bool _alive = true;
        
    void Awake()
    {
        i = this;
    }

    private void Start()
    {
        UIManager.i.ShowTutorial(true);
    }

    void Update()
    {
        
    }
}
