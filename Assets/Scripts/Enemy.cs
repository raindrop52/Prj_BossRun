using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    EnemyAnimation _anim;
    EnemyController _enemyController;

    void Awake()
    {
        _anim = GetComponent<EnemyAnimation>();
        _enemyController = GetComponent<EnemyController>();
    }

}
