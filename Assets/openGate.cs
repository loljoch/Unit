using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openGate : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Animator>().SetTrigger("Opened");
    }
}
