using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake() 
    {
        current = this;
    }
   
    public event Action OnPlayerAttack;
    public void PlayerAttack()
    {
        //called by other scripts
        OnPlayerAttack?.Invoke();
    }

    public event Action OnToxicRise;
    public void ToxicRise()
    {
        OnToxicRise?.Invoke();
    }

    public event Action OnToxicFall;
    public void ToxicFall()
    {
        OnToxicFall?.Invoke();
    }

    public event Action OnBossDead;
    public void BossDead()
    {
        OnBossDead?.Invoke();
    }

    public event Action OnPlayerDead;
    public void PlayerDead()
    {
        OnPlayerDead?.Invoke();
    }

    public event Action OnLevelEnd;
    public void LevelEnd()
    {
        OnLevelEnd?.Invoke();
    }

    public event Action OnEnemyDead;
    public void EnemyDead()
    {
        OnEnemyDead?.Invoke();
    }

    public event Action OnPlayerPick;
    public void PlayerPick()
    {
        OnPlayerPick?.Invoke();
    }

    public event Action OnStageTwo;
    public void StageTwo()
    {
        OnStageTwo?.Invoke();
    }

    public event Action OnStageThree;
    public void StageThree()
    {
        OnStageThree?.Invoke();
    }

    public event Action<int> OnDoorOpen;
    public void DoorOpen(int i)
    {
        OnDoorOpen?.Invoke(i);
    }

    public event Action<int> OnDoorClose;
    public void DoorClose(int i)
    {
        OnDoorClose?.Invoke(i);
    }



}
