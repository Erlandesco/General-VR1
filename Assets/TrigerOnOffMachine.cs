using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrigerOnOffMachine : MonoBehaviour
{
    [SerializeField]private bool isON = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Kenop") && isON)
        {
            isON = true;
        }
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Kenop") && !isON)
        {
            isON = false;
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
