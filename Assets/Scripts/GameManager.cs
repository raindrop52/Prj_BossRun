using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;
    [Header("Ʃ�丮��")]
    public bool _isTutorial = false;

    [Header("�÷��̾�")]
    public Transform _respawnPos;
    public GameObject _goPlayer;
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

    void CreatePlayer()
    {
        if (_player == null)
        {
            GameObject go = Instantiate(_goPlayer);
            go.transform.position = _respawnPos.position;
        }
    }
}
