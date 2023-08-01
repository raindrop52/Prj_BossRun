using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager i;

    public Player _player;
    
    void Awake()
    {
        i = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
