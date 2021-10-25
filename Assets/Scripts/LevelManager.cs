using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup levelEndCG;

    // Start is called before the first frame update
    void Start()
    {
        UIExpansion.Hide(levelEndCG);
        
        //subscribe to events
        GameEvents.current.OnLevelEnd += LevelEnd;
    }

    private void OnDestroy() 
    {
        GameEvents.current.OnLevelEnd -= LevelEnd;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LevelEnd()
    {
        UIExpansion.Show(levelEndCG);
    }
}
