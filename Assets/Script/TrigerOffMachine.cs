using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigerOffMachine : MonoBehaviour
{
    public TrigerOnMachine trigerOnMachine;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Kenop") && trigerOnMachine.isON)
        {
            trigerOnMachine.isON = false;
            Debug.Log(trigerOnMachine.isON);
            Debug.Log("Mesin Off");
        }

    }
}
