using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToxicState
{
    Rise,
    Fall
}

public class ToxicController : MonoBehaviour
{
    private ToxicState myToxicState;
    public ToxicState MyToxicState
    {
        get{ return myToxicState; }
        set
        {
            myToxicState = value;
            if(myToxicState == ToxicState.Fall) 
            {
                GameEvents.current.ToxicFall();
                Debug.Log("Current state is fall");

                StopCoroutine("Fall");
                StartCoroutine("Fall");
            }
            else if(myToxicState == ToxicState.Rise)
            {
                GameEvents.current.ToxicRise();
                Debug.Log("Current state is rise");

                StopCoroutine("Rise");
                StartCoroutine("Rise");
            }
        }
    }

    [SerializeField] private float risingTime;
    [SerializeField] private float fallingTime;

    [SerializeField] private float timer;

    IEnumerator Rise()
    {
        while(timer < risingTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if(timer >= risingTime)
        {
            timer = 0;
        }
    }

    IEnumerator Fall()
    {
        while(timer < fallingTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if(timer >= fallingTime)
        {
            timer = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MyToxicState = ToxicState.Fall;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Current state = " + MyToxicState);
    }
}
