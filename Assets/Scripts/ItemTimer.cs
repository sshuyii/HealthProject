using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTimer : MonoBehaviour
{
    [SerializeField] private float timeLength;
    private float timer;
    public float Timer
    {
        get{ return timer;}
        set 
        {   
            timer = value;
            StopCoroutine("CalTime");
            StartCoroutine("CalTime");
           
        }
    }

    IEnumerator CalTime()
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        
        if(timer <= 0)
        {
            timer = 0;
            Destroy(this.gameObject);
        }
    
    }

    // Start is called before the first frame update
    void Start()
    {
        Timer = timeLength;
    }

    public void Reset()
    {
        Timer = timeLength;
    }
}
