using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private int currentStage = 1;
    private int defeatedEnemyNum;
    [SerializeField] private ChestController chest;
    [SerializeField] private CanvasGroup levelEndCG;
    [SerializeField] private CanvasGroup levelFailCG;
    [SerializeField] private DoorController finalDoor;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        UIExpansion.Hide(levelEndCG);
        UIExpansion.Hide(levelFailCG);

        
        //subscribe to events
        GameEvents.current.OnLevelEnd += LevelEnd;
        GameEvents.current.OnEnemyDead += EnemyDead;
        GameEvents.current.OnLevelFail += LevelFail;

    }

    private void OnDestroy() 
    {
        GameEvents.current.OnLevelEnd -= LevelEnd;
        GameEvents.current.OnEnemyDead -= EnemyDead;
        GameEvents.current.OnLevelFail -= LevelFail;

    }

    // Update is called once per frame
    void Update()
    {
        if(currentStage == 1 && defeatedEnemyNum == 4)
        {
            chest.myMR.enabled = true;
        }

        if(chest.Empty && currentStage == 1)
        {
            currentStage = 2;
            GameEvents.current.StageTwo();
            GameEvents.current.DoorOpen(1);
        }

        if(finalDoor.isOpen)
        {
            currentStage = 3;
            GameEvents.current.StageThree();
        }
    }

    private void StageThree()
    {
        currentStage = 3;
    }


    private void LevelEnd()
    {
        Time.timeScale = 0;
        //succeed
        UIExpansion.Show(levelEndCG);
    }

    public void LevelFail()
    {
        Time.timeScale = 0;
        //fail
        UIExpansion.Show(levelFailCG);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Demo", LoadSceneMode.Single);
    }

    public void EnemyDead()
    {
        defeatedEnemyNum ++;
    }
}
