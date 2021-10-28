using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private bool startOpen;
    public int Index;
    private Animator myAnimator;

    [HideInInspector] public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myAnimator.SetBool("Open", startOpen);

        isOpen = startOpen;

        //subscribe to events
        GameEvents.current.OnDoorOpen += Open;
        GameEvents.current.OnDoorClose += Close;

    }

    private void OnDestroy() 
    {
        GameEvents.current.OnDoorOpen -= Open;
        GameEvents.current.OnDoorClose -= Close;
    }

    private void Open(int i)
    {
        if(Index == i)
        {
            isOpen = true;
            myAnimator.SetBool("Open", true);
        }
    }
    private void Close(int i)
    {
        if(Index == i)
        {
            isOpen = false;
            myAnimator.SetBool("Open", false);
        }
    }
}

