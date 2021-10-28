using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Open"))
        {
            int index = other.GetComponentInParent<DoorController>().Index;
            {
                GameEvents.current.DoorOpen(index);
            }
        }
        else if(other.CompareTag("Close"))
        {
            int index = other.GetComponentInParent<DoorController>().Index;
            {
                GameEvents.current.DoorClose(index);
            }
        }
    }
}
