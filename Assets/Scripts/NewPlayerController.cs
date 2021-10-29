using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    //references
    private PlayerInputActions playerInputActions;
    private CharacterController myCC;
    private Animator myAnimator;

    [Header("Movement Variables")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpVelocity;
    private float gravity;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private bool isMovePressed;
    private bool isRunPressed;
    private bool isJumpPressed;
    private bool isAttackPressed;


    private bool isAttacking;
    private bool isJumping;

    [SerializeField] private string[] attackAnimNames;

    private void Awake() 
    {
        myCC = GetComponent<CharacterController>();
        myAnimator = GetComponent<Animator>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Move.started += OnMovementInput;
        playerInputActions.Player.Move.performed += OnMovementInput;
        playerInputActions.Player.Move.canceled += OnMovementInput;

        playerInputActions.Player.Run.started += OnRun;
        playerInputActions.Player.Run.canceled += OnRun;

        playerInputActions.Player.Jump.started += OnJump;
        playerInputActions.Player.Jump.canceled += OnJump;

        playerInputActions.Player.Attack.started += OnAttack;
        playerInputActions.Player.Attack.canceled += OnAttack;

        playerInputActions.Player.Pick.started += OnPick;
        playerInputActions.Player.Pick.canceled += OnPick;

    }

    private void Start() 
    {
        GameEvents.current.OnPlayerDead += HandleDead;
    }

    private void OnDestroy() 
    {
        GameEvents.current.OnPlayerDead -= HandleDead;
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {

        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        Vector3 camDir = Camera.main.transform.forward;
        camDir = new Vector3(camDir.x, 0, camDir.z);

        float angle = Vector3.SignedAngle(Vector3.forward, camDir, Vector3.up);
        currentMovement = Quaternion.AngleAxis(angle, Vector3.up) * currentMovement;

        if(isRunPressed) currentMovement = new Vector3(currentMovement.x * runSpeed, 0, currentMovement.z * runSpeed);

        //This will cause player rotate to unexpected angle
        // if(isAttacking) currentMovement = Vector3.zero;

        isMovePressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMovePressed && !isAttacking && !isJumpPressed) Rotate();

        HandleAnimation();

        if(!isAttacking) myCC.Move(currentMovement * Time.deltaTime);

        HandleGravity();
        HandleJump();

        // Debug.Log("currentMovement Y = " + currentMovement.y);
    }

    private void HandleGravity()
    {
        if(myCC.isGrounded)
        {
            gravity = -0.5f;
        }
        else
        {
            gravity = -20f;
            currentMovement.y += gravity * Time.deltaTime;
        }
    }

    private void HandleJump()
    {
        if(!isJumping && myCC.isGrounded && isJumpPressed)
        {
            isJumping= true;
            
            myAnimator.SetTrigger("Jump");
            currentMovement.y = jumpVelocity;

            //temporarily change x and z movement
            //not the best solution
            currentMovement.x *= 2;
            currentMovement.z *= 2;
        }
        else if(!isJumpPressed && isJumping && myCC.isGrounded)
        {
            isJumping = false;
        }
    }


    private int attackIdx;//two kinds of attack
    public int AttackIdx
    {
        get{ return attackIdx;}
        set
        {
            if(value < attackAnimNames.Length)
            {
                attackIdx = value;
            }
            else
            {
                attackIdx = 0;
            }
        }
    }

    private void HandleDead()
    {
        myAnimator.SetTrigger("Die");
    }

    private void Dead()
    {
        GameEvents.current.LevelFail();
    }

    private void HandleAnimation()
    {         
        if(isAttackPressed && !isAttacking)
        {
            AttackIdx ++;
            isAttacking = true;
            Debug.Log("isAttacking = " + isAttacking);

            myAnimator.SetTrigger(attackAnimNames[attackIdx]);
        }


        //player should not move if it is attacking
        AnimatorStateInfo animatorInfo = myAnimator.GetCurrentAnimatorStateInfo(0);
        // Debug.Log(animatorInfo.IsTag("Attack") + "+" + animatorInfo.normalizedTime);

        if (isAttacking && animatorInfo.normalizedTime > (1.0f - Time.deltaTime)
            && animatorInfo.IsTag("Attack"))
        {
            //attack anim has ended
            isAttacking = false;
        }

        if(isMovePressed && !isAttacking)
        {
            myAnimator.SetBool("Walk", true);

            if(isRunPressed && !isAttacking)
            {
                myAnimator.SetBool("Run", true);
            }
            else
            {
                myAnimator.SetBool("Run", false);
            }
        }
        else
        {
            myAnimator.SetBool("Walk", false);
            myAnimator.SetBool("Run", false);
        }
     
    }

    private void Rotate()
    {
        Quaternion toRotation = Quaternion.LookRotation(new Vector3(currentMovement.x, 0, currentMovement.z), Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);    
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        Debug.Log("Run is " + context.ReadValueAsButton());
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump is " + context.ReadValueAsButton());
        isJumpPressed = context.ReadValueAsButton();
    }
 
    private void OnAttack(InputAction.CallbackContext context)
    {
        isAttackPressed = context.ReadValueAsButton();
        if(isAttackPressed) GameEvents.current.PlayerAttack();
    }

    private void OnPick(InputAction.CallbackContext context)
    {
       GameEvents.current.PlayerPick();
    }

    private void OnEnable() 
    {
        //enable the action map
        playerInputActions.Player.Enable();
    }

    private void OnDisable() 
    {
        //disable the action map
        playerInputActions.Player.Disable();
    }
}
