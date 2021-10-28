using System.Collections;
using UnityEngine;

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
    [SerializeField] private float alertRange;


    [SerializeField] private float physicsDamage;
    [SerializeField] private float magicDamage;

    [SerializeField] private float cdLength;
    private float cdTimer;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    [SerializeField] private float attackAngle;
    [SerializeField] private float attackRadius;

    [SerializeField] private GameObject toxicPrefab;

    private bool rotatable;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    [SerializeField] private CanvasGroup uiCG;


    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myHealthManager = GetComponent<HealthManager>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

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
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            Quaternion.LookRotation(player.position - transform.position), rotateSpeed * Time.deltaTime);
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
            myAnimator.SetBool("Move Forward", true);

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
        myHealthManager.Invisible = true;

        //move to initial position and play dead animation
        // StartCoroutine("MoveInitial");

        if(life > 0)
        {
            myAnimator.SetTrigger("Dead");

            StartCoroutine(TideRising());
            StartCoroutine("Resting");
        }
        else
        {
            Dead();
        }    
    }

    IEnumerator MoveInitial()
    {
        while(Vector3.Distance(transform.position ,initialPosition) > 0.05f)
        {
            myAnimator.SetBool("Move Forward", true);

            transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * 1.8f * Time.deltaTime);
            transform.LookAt(initialPosition);
            yield return null;
        }

        while(Quaternion.Angle(transform.rotation, initialRotation) > 1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, rotateSpeed * 1.8f * Time.deltaTime);
            yield return null;
        }

        myAnimator.SetBool("Move Forward", false);
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
            timer += Time.deltaTime;

            yield return null;
        }

        if(timer >= restTime)
        {
            timer = 0;
            
            //set position and rotation
            transform.position = initialPosition;
            transform.localRotation = initialRotation;

            myAnimator.SetTrigger("Born");
            GameEvents.current.ToxicFall();
        }
    }
    private void Dead()
    {
        GameEvents.current.LevelEnd();
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
