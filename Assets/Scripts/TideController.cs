using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class TideController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float height;
    [SerializeField] private GameObject healthRecoverArea;

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

        //subscribe to events
        GameEvents.current.OnToxicFall += Fall;
        GameEvents.current.OnToxicRise += Rise;
    }

    private void OnDestroy() 
    {
        GameEvents.current.OnToxicFall -= Fall;
        GameEvents.current.OnToxicRise -= Rise;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Rise()
    {
        StopCoroutine("Rising");
        StartCoroutine("Rising");
    }

    IEnumerator Rising()
    {
        healthRecoverArea.SetActive(true);

        while(transform.position.y < height + initialPosition.y)
        {
            // Debug.Log("Stone is rising");

            transform.position += new Vector3(0, speed * Time.deltaTime, 0);

            yield return null;
        }
        if(transform.position.y <= height + initialPosition.y)
        {
            transform.position = initialPosition + new Vector3(0, height, 0);
        }
    }

    private void Fall()
    {
        StopCoroutine("Falling");
        StartCoroutine("Falling");
    }

    IEnumerator Falling()
    {
        healthRecoverArea.SetActive(false);

        while(transform.position.y > initialPosition.y)
        {
            // Debug.Log("Stone is falling");

            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);

            yield return null;
        }
        if(transform.position.y <= initialPosition.y)
        {
            transform.position = initialPosition;
        }
    }

}
