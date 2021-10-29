using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator myAnimator;
    private HealthManager myHealthManager;

    [SerializeField] private Transform player;

    [Header("Attack Variables")]
    [SerializeField] private float shortAttackRange;
    [SerializeField] private float longAttackRange;
    [SerializeField] private float alertRange;
    [SerializeField] private float attackAngle;
    [SerializeField] private float cdLength;
    private float cdTimer;


    [Header("Damage Variables")]
    [SerializeField] private float shortDamage;
    [SerializeField] private float longDamage;

    
    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;


    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myHealthManager = GetComponent<HealthManager>();
    }

    private void OnDestroy() 
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!myHealthManager.Invisible) 
        {
            LookAtPlayer();
            CalPlayerDist();
        }
        else
        {
            myAnimator.SetBool("Move Backwards", false);
            myAnimator.SetBool("Move Forward", false);
        }
    }

    private void LookAtPlayer()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, 
        Quaternion.LookRotation(player.position - transform.position), rotateSpeed * Time.deltaTime);
    }

    private void CalPlayerDist()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        // Debug.Log("the distance between boss and player is " + dist);

        if(dist <= shortAttackRange)
        {           
            //boss attack physics
            if(cdTimer < cdLength)
            {
                cdTimer += Time.deltaTime;
                myAnimator.SetBool("Move Forward", false);
            }
            else
            {
                cdTimer = 0;
                myAnimator.SetTrigger("Attack_Short");
            }
        }
        else if(dist <= longAttackRange)
        {
            //boss attack magic
            if(cdTimer < cdLength)
            {
                cdTimer += Time.deltaTime;
                myAnimator.SetBool("Move Forward", false);
            }
            else
            {
                cdTimer = 0;
                myAnimator.SetTrigger("Attack_Long");
            }
        }
        else if(dist <= alertRange)
        {
            myAnimator.SetBool("Alert", true);

            //follow player
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            myAnimator.SetBool("Move Forward", true);
        }
        else
        {
            myAnimator.SetBool("Alert", false);
        }
    }

    private void LongAttack()
    {
        if(ValidAttack(transform, player, longAttackRange))
        {
            player.GetComponent<HealthManager>().Health -= longDamage;
        }
    }

    private void ShortAttack()
    {
        if(ValidAttack(transform, player, shortAttackRange))
        {
            player.GetComponent<HealthManager>().Health -= shortDamage;
        }
    }

    public void GetHit()
    {
        if(myHealthManager.Health > 20f) myAnimator.SetTrigger("Hit");
    }

    public IEnumerator Dead()
    {
        myAnimator.SetTrigger("Dead");

        yield return new WaitForSeconds(5f);

        player.GetComponent<PlayerAttack>().EnemyList.Remove(this.transform);
        Destroy(gameObject);
    }

    public bool ValidAttack(Transform player, Transform target, float radius)
    {
        Vector3 attackDir = target.position - player.position;

        float realAngle = Mathf.Acos(Vector3.Dot(attackDir.normalized, player.forward)) * Mathf.Rad2Deg;

        if(realAngle < attackAngle * 0.5f && attackDir.sqrMagnitude < radius * radius)
        {
            Debug.Log("player attack is valid");
            return true;
        }

        return false;
    }


}
