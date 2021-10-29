using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private int life;
    [SerializeField] private float restTime;
    [SerializeField] private float cdLength;
    private float timer;
    private float cdTimer;

    
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject toxicPrefab;
    private Animator myAnimator;
    private HealthManager myHealthManager;
    [SerializeField] private CanvasGroup uiCG;


    [Header("Attack Variables")]
    [SerializeField] private float escapeRange;
    [SerializeField] private float magicAttackRange;
    [SerializeField] private float alertRange;
    [SerializeField] private float physicsDamage;
    [SerializeField] private float attackAngle;
    [SerializeField] private float attackRadius;


    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;


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
        if(!myHealthManager.Invisible) 
        {
            LookAtTarget(player.position);
            CalPlayerDist();
        }
        else
        {
            myAnimator.SetBool("Move Backwards", false);
            myAnimator.SetBool("Move Forward", false);
        }
    }

    private void LookAtTarget(Vector3 v)
    {
        Vector3 temp = v - transform.position;
        temp.y = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.LookRotation(temp), rotateSpeed * Time.deltaTime);
    }

    private void CalPlayerDist()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        // Debug.Log("the distance between boss and player is " + dist);

        if(dist <= magicAttackRange)
        {
            myAnimator.SetBool("Move Forward", false);

            if(dist <= escapeRange)
            {
                //boss escape from player
                Vector3 moveDir = Vector3.Normalize(new Vector3(transform.position.x, 0, transform.position.z) 
                    - new Vector3(player.position.x, 0, player.position.z));
                transform.position += moveDir * Time.deltaTime * moveSpeed;

                myAnimator.SetBool("Move Backwards", true);
            }
            else
            {
                myAnimator.SetBool("Move Backwards", false);
                //follow player
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            }
           
            //boss attack physics
            if(cdTimer < cdLength)
            {
                cdTimer += Time.deltaTime;
            }
            else
            {
                cdTimer = 0;
                myAnimator.SetTrigger("Attack_Physics");
            }
        }
        else if(dist <= alertRange)
        {

            UIExpansion.Show(uiCG);
            myAnimator.SetBool("Move Backwards", false);
            myAnimator.SetBool("Move Forward", false);

            //boss attack magic
            if(cdTimer < cdLength)
            {
                cdTimer += Time.deltaTime;
            }
            else
            {
                cdTimer = 0;
                myAnimator.SetTrigger("Attack_Magic");
            }
        }
        else
        {
            UIExpansion.Hide(uiCG);
            myAnimator.SetBool("Move Forward", false);
            myAnimator.SetBool("Move Backwards", false);
        }
    }

    private void AttackMagic()
    {
        //toxic appear at the position where player stands
        StartCoroutine(ToxicAppear());
    }

    IEnumerator ToxicAppear()
    {
        yield return new WaitForSeconds(0.6f);
        Vector3 v = player.position;

        yield return new WaitForSeconds(0.4f);

        Instantiate(toxicPrefab, v, Quaternion.identity);
    }

    private void AttackPhysics()
    {
        if(ValidAttack(transform, player, attackRadius))
        {
            player.GetComponent<HealthManager>().Health -= physicsDamage;
        }
    }

    private void Rest()
    {
        life --;
        
        myAnimator.SetTrigger("Dead");

        GameEvents.current.ToxicRisePre();

        //move to initial position and play dead animation
        // StartCoroutine("MoveInitial");
    }

    // IEnumerator MoveInitial()
    // {
    //     while(Vector3.Distance(transform.position ,initialPosition) > 0.05f)
    //     {
    //         myAnimator.SetBool("Move Forward", true);

    //         transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * 1.8f * Time.deltaTime);

    //         LookAtTarget(initialPosition);

    //         yield return null;
    //     }

    //     while(Quaternion.Angle(transform.rotation, initialRotation) > 1f)
    //     {
    //         myAnimator.SetBool("Move Forward", false);

    //         transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, rotateSpeed * 1.8f * Time.deltaTime);
    //         yield return null;
    //     }

    //     myAnimator.SetTrigger("Dead");

    // }


    IEnumerator Resting()
    {         
        //time for player to run away from toxic
        yield return new WaitForSeconds(3f);

        GameEvents.current.ToxicRise();

        while(timer < restTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        if(timer >= restTime)
        {
            timer = 0;
            
            //set position and rotation
            // transform.position = initialPosition;
            // transform.localRotation = initialRotation;

            myAnimator.SetTrigger("Born");
            GameEvents.current.ToxicFall();
        }
    }
    private void BossDead()
    {
        if(life <= 0) 
        {
            GameEvents.current.LevelEnd();
        }
        else 
        {
            StartCoroutine(Resting());
        }
    }
    
    private void Reborn()
    {
        myHealthManager.Invisible = false;
        myHealthManager.Health = myHealthManager.MaxHealth;
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
