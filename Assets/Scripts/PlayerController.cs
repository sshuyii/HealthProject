using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpSpeed;

    private Animator myAnimator;
    private Rigidbody myRB;

    private PlayerInputActions playerInputActions;

    //variables to store player input values
    private Vector2 currentMovement;
    private bool movePressed;
    private bool runPressed;
    private bool jumpPressed;
    private bool isGrounded;
    
   private void Awake() 
    {
        myAnimator = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Player.Move.started += Move;
        playerInputActions.Player.Move.performed += Move;
        playerInputActions.Player.Move.canceled += Move;

        playerInputActions.Player.Run.started += Run;
        playerInputActions.Player.Run.canceled += Run;

        playerInputActions.Player.Jump.started += Jump;
        playerInputActions.Player.Jump.canceled += Jump;

        playerInputActions.Player.Attack.started += Attack;
        playerInputActions.Player.Attack.canceled += Attack;

    }

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //player movement
        //if jump is pressed, the player should perform jump action
        //and resume walk or run until jump is finished
        if(movePressed) 
        {
            if(!runPressed) myAnimator.SetBool("Walk", true);
            else myAnimator.SetBool("Run", true);

            Rotate();
        }
    }

    private void Rotate()
    {
        Vector3 camDir = Camera.main.transform.forward;
        Vector3 temp = new Vector3(currentMovement.x, 0, currentMovement.y);

        temp = Camera.main.transform.TransformDirection(temp);
        temp = new Vector3(temp.x, 0, temp.z);

        Quaternion toRotation = Quaternion.LookRotation(temp, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log("movePressed = true");
            currentMovement = context.ReadValue<Vector2>();
            
            movePressed = true;
        }
        else if(context.canceled)
        {
            movePressed = false;

            myAnimator.SetBool("Walk", false);

        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton())
        {
            myAnimator.SetTrigger("Jump");
            myRB.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.Impulse);

        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        runPressed = context.ReadValueAsButton();
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(context.ReadValueAsButton() && isGrounded)
        {
            myAnimator.SetTrigger("Bare Hands Projectile Right Attack 01");

        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if(other.transform.CompareTag("Ground"))
        {
            isGrounded = true;
            myAnimator.applyRootMotion = true;

        }
    }

    private void OnCollisionExit(Collision other) 
    {
        if(other.transform.CompareTag("Ground"))
        {
            isGrounded = false;
            myAnimator.applyRootMotion = false;
        }
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
