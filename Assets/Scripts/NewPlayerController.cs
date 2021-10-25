using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private CharacterController myCC;
    private Animator myAnimator;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float runSpeed;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isJumpPressed;
    bool isAttackPressed;

    private bool isAttacking;
    private float gravity;

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

    }

    private void Start() 
    {
        GameEvents.current.OnPlayerDead += Dead;
    }

    private void OnDestroy() 
    {
        GameEvents.current.OnPlayerDead -= Dead;
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

        if(isAttacking) currentMovement = Vector3.zero;

        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        HandleAnimation();

        myCC.Move(new Vector3(currentMovement.x, gravity, currentMovement.z) * Time.deltaTime);

        HandleGravity();
    }

    private void HandleGravity()
    {
        if(myCC.isGrounded)
        {
            gravity = -0.5f;
        }
        else
        {
            gravity = -9.8f;
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

    private void Dead()
    {
        myAnimator.SetTrigger("Die");
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

        if(isMovementPressed && !isAttacking)
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
        Quaternion toRotation = Quaternion.LookRotation(currentMovement, Vector3.up);

        if(isMovementPressed)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);    
        }
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        Debug.Log("Run is " + context.ReadValueAsButton());
        isRunPressed = context.ReadValueAsButton();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }
 
    private void OnAttack(InputAction.CallbackContext context)
    {
        isAttackPressed = context.ReadValueAsButton();
        if(isAttackPressed) GameEvents.current.PlayerAttack();
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
