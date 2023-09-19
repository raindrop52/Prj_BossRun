using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSky : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speed);
    }
}
