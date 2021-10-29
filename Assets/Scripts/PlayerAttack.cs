using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackAngle;
    [SerializeField] private float attackRadius;
    [SerializeField] private float playerDamage;
    public List<Transform> EnemyList;


    void Start()
    {
        // GameEvents.current.OnPlayerAttack += Attack;
        GameEvents.current.OnStageThree += ChangeEnemy;
    }


    private void OnDestroy() 
    {
        // GameEvents.current.OnPlayerAttack -= Attack;
        GameEvents.current.OnStageThree -= ChangeEnemy;
    }

    public void Attack()
    {
        Debug.Log("On Attack");
        
            for(int i = 0; i < EnemyList.Count; i++)
            {
                if(EnemyList[i] !=null)
                {
                    if(ValidAttack(transform, EnemyList[i]))
                    {
                        EnemyList[i].GetComponent<HealthManager>().Health -= playerDamage; 
                        if(EnemyList[i].GetComponent<EnemyController>()) EnemyList[i].GetComponent<EnemyController>().GetHit();
                        Debug.Log(EnemyList[i].name + " is damaged");
                    }
                }
            }
        
    }

    public bool ValidAttack(Transform player, Transform target)
    {
        Vector3 attackDir = target.position - player.position;

        float realAngle = Mathf.Acos(Vector3.Dot(attackDir.normalized, player.forward)) * Mathf.Rad2Deg;

        if(realAngle < attackAngle * 0.5f && attackDir.sqrMagnitude < attackRadius * attackRadius)
        {
            Debug.Log("player attack is valid");
            return true;
        }

        return false;
    }

    private void ChangeEnemy()
    {

    }
}
