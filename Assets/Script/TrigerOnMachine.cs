using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigerOnMachine : MonoBehaviour
{
    [HideInInspector]public bool isON = false;
    public TrigerOffMachine trigerOffMachine;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Kenop") && !isON)
        {
            isON = true;
            Debug.Log(isON);
            Debug.Log("Mesin ON");
        }
       
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Kenop") && !isON)
    //    {
    //        isON = false;
    //        Debug.Log(isON);
    //        Debug.Log("Mesin Off");
    //    }

    //}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
    }
}
