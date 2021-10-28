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
    [SerializeField] private float healthRecover;

    private float health;
    public float Health
    {
        get{return health; }
        set
        {
            if(!Invisible)
            {
                health = value;
        
                healthBar.fillAmount = health/MaxHealth;

                if(health <= 0)
                {
                    health = 0;
                    Invisible = true;//if player is already dead, no damage should be taken
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
            case CharacterType.Enemy:
                StartCoroutine(GetComponent<EnemyController>().Dead());
                GameEvents.current.EnemyDead();
                break;
        }
       
    }

    private void OnTriggerStay(Collider other) 
    {
        if(other.CompareTag("Toxic"))
        {
            if(myCharacterType == CharacterType.Player)
            {
                Debug.Log(gameObject.name + " is in toxic");
                Health -= toxicDamage;
            }
        }
        else if(other.CompareTag("Health"))
        {
            if(myCharacterType == CharacterType.Player)
            {
                Debug.Log(gameObject.name + " is recovering");
                Health += healthRecover;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("LazerStop"))
        {
            if(myCharacterType == CharacterType.Player)
            {
                Health = 0;
            }
        }
        
    }
}
