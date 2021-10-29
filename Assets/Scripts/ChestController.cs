using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public int index;
    [SerializeField] private int itemNum;
    public int ItemNum
    {
        get{ return itemNum;}
        set
        { 
            itemNum = value;
            if(itemNum <= 0)
            {
                itemNum = 0;
                empty = true;
            }
        }
    }
    [SerializeField] private CanvasGroup pickUI;
    [HideInInspector] public MeshRenderer myMR;


    private bool empty;
    public bool Empty
    {
        get{ return empty;}
        set
        {
            empty = value;
            if(empty)
            {
                myMR.enabled = false;
                UIExpansion.Hide(pickUI);
            }
        }
    }

    private bool pickable;


    // Start is called before the first frame update
    void Start()
    {
        myMR = GetComponent<MeshRenderer>();
        myMR.enabled = false;

        Empty = false;
        UIExpansion.Hide(pickUI);

        //subscribe to event
        GameEvents.current.OnPlayerPick += Pick;
    }

    private void OnDestroy() 
    {
        GameEvents.current.OnPlayerPick -= Pick;
    }


    private void Pick()
    {
        if(pickable)
        {
            Empty = true;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !Empty)
        {
            UIExpansion.Show(pickUI);
            pickable = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player") && !Empty)
        {
            UIExpansion.Hide(pickUI);
            pickable = false;
        }
    }
}
