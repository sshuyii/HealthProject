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


}
