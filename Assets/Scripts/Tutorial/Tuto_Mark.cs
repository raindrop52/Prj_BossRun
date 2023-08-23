using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto_Mark : MonoBehaviour
{
    [SerializeField] string text;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            UIManager.i.ChangeText_Tutorial(text);
        }
    }
}
