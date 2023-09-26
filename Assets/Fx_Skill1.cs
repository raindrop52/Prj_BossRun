using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fx_Skill1 : MonoBehaviour
{
    public List<ParticleSystem> _pslMain;
    public List<ParticleSystem> _pslEffect;
    public bool test = false;

    void Update()
    {
        if (test)
        {
            test = false;
            Play();
        }
    }

    public void Play()
    {
        Play_Main();
        Play_Effect();        
    }

    void Play_Main()
    {
        foreach (ParticleSystem ps in _pslMain)
        {
            ps.Play();
        }
    }

    void Play_Effect()
    {
        foreach (ParticleSystem ps in _pslEffect)
        {
            ps.Play();
        }
    }
}
