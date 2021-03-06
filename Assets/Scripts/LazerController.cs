using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LazerMode
{
    Static,
    Move,
}


public class LazerController : MonoBehaviour
{
    public LazerMode myLM;
    [HideInInspector] public int DoorIdx;
    [HideInInspector] public Transform LazerStop;
    [HideInInspector] public Vector3 Speed;
    private bool move;
    private MeshRenderer[] myMRList;
    private Vector3 stopPoint;
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        if(myLM != LazerMode.Static)
        {
            initialPosition = transform.position;
            stopPoint = LazerStop.position;
        }

        myMRList = GetComponentsInChildren<MeshRenderer>();
    
        //subscribe to events
        GameEvents.current.OnDoorOpen += Move;
    
    }

    // Update is called once per frame
    void Update()
    {
        if(myLM == LazerMode.Move && move)
        {
            if(Vector3.Distance(transform.position, stopPoint) > 0.05f)
            {
                transform.position += Speed * Time.deltaTime;
            }
            else
            {
                Disappear();
                Speed = Vector3.zero;
            }
        }
    }

    private void Disappear()
    {
        foreach(MeshRenderer mr in myMRList)
        {
            mr.enabled = false;
        }
    }

    private void Move(int i)
    {
        if(DoorIdx == i) move = true;
    }
    
}
