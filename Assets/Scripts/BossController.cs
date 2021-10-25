using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BossController : MonoBehaviour
{
    [SerializeField] private int life;
    [SerializeField] private float restTime;
    private float timer;

    private Animator myAnimator;
    private HealthManager myHealthManager;

    [SerializeField] private Transform player;

    [SerializeField] private float escapeRange;
    [SerializeField] private float magicAttackRange;

    [SerializeField] private float physicsDamage;
    [SerializeField] private float magicDamage;

    [SerializeField] private float cdLength;
    private float cdTimer;

    [SerializeField] private float speed;
    [SerializeField] private float attackAngle;
    [SerializeField] private float attackRadius;

    [SerializeField] private GameObject toxicPrefab;


    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myHealthManager = GetComponent<HealthManager>();

        //subscribe to events
        GameEvents.current.OnBossDead += Rest;
    }

    private void OnDestroy() 
    {
        GameEvents.current.OnBossDead -= Rest;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();
        if(!myHealthManager.Invisible) CalPlayerDist();
        else
        {
            myAnimator.SetBool("Move Backwards", false);
            myAnimator.SetBool("Move Forward", false);
        }

    }

    private void LookAtPlayer()
    {
        transform.LookAt(player);
    }

    private void CalPlayerDist()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        // Debug.Log("the distance between boss and player is " + dist);

        if(dist <= magicAttackRange)
        {
            if(dist <= escapeRange)
            {
                //boss escape from player
                Vector3 moveDir = Vector3.Normalize(new Vector3(transform.position.x, 0, transform.position.z) 
                    - new Vector3(player.position.x, 0, player.position.z));
                transform.position += moveDir * Time.deltaTime * speed;

                myAnimator.SetBool("Move Backwards", true);
            }
            else
            {
                myAnimator.SetBool("Move Backwards", false);
                //follow player
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
           
            //boss attack physics
            if(cdTimer < cdLength)
            {
                cdTimer += Time.deltaTime;
            }
            else
            {
                cdTimer = 0;
                AttackPhysics();
            }
        }
        else
        {
            myAnimator.SetBool("Move Backwards", false);
            myAnimator.SetBool("Move Forward", true);

            //boss attack magic
            if(cdTimer < cdLength)
            {
                cdTimer += Time.deltaTime;
            }
            else
            {
                cdTimer = 0;
                AttackMagic();
            }
        }
    }

    private void AttackMagic()
    {
        //toxic appear at the position where player stands
        StartCoroutine(ToxicAppear());

        myAnimator.SetTrigger("Attack_Magic");
    }

    IEnumerator ToxicAppear()
    {
        yield return new WaitForSeconds(0.6f);
        Vector3 v = player.position;

        yield return new WaitForSeconds(0.4f);

        Instantiate(toxicPrefab, new Vector3(v.x, 0, v.z), Quaternion.identity);
    }

    private void AttackPhysics()
    {
        myAnimator.SetTrigger("Attack_Physics");
        if(ValidAttack(transform, player))
        {
            player.GetComponent<HealthManager>().Health -= physicsDamage;
        }
    }

    private void Rest()
    {
        life --;
        myAnimator.SetTrigger("Dead");

        if(life > 0)
        {
            StartCoroutine(TideRising());
            StartCoroutine("Resting");
        }
        else
        {
            Dead();
        }
    }

    IEnumerator TideRising()
    {
        //todo: time according to animation clip length
        yield return new WaitForSeconds(3f);

        //trigger tide event
        GameEvents.current.ToxicRise();
    }
    IEnumerator Resting()
    {
        while(timer < restTime)
        {
            myHealthManager.Invisible = true;
            timer += Time.deltaTime;

            yield return null;
        }

        if(timer >= restTime)
        {
            myHealthManager.Invisible = false;
            timer = 0;
            myAnimator.SetBool("Dead", false);
            myHealthManager.Health = myHealthManager.MaxHealth;

            GameEvents.current.ToxicFall();
        }
    }

    private void Dead()
    {
        
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
}
