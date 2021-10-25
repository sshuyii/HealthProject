using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public enum CharacterType
    {
        Player,
        Enemy,
        Boss
    }

    public bool Invisible = false;
    public CharacterType myCharacterType;
    [SerializeField] private Image healthBar;
    public float MaxHealth;

    [SerializeField] private float toxicDamage;

    private float health;
    public float Health
    {
        get{return health; }
        set
        {
            if(!Invisible)
            {
                health = value;
            
                if(myCharacterType != CharacterType.Enemy)
                {
                    healthBar.fillAmount = health/MaxHealth;
                }

                if(health <= 0)
                {
                    health = 0;
                    Dead();
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        health = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Dead()
    {
        switch (myCharacterType)
        {
            case CharacterType.Boss:
                GameEvents.current.BossDead();
                break;
            case CharacterType.Player:
                GameEvents.current.PlayerDead();
                break;

        }
       
    }

    private void OnTriggerStay(Collider other) 
    {
        if(other.CompareTag("Toxic"))
        {
            Health -= toxicDamage;
        }
    }
}
