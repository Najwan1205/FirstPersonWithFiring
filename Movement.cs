using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{ 
    [Header("CharacterMovementSetting")]
    public CharacterController controller;
    public float gravity = -9.81f;
    public float maxWalkSpeed = 3f;
    public float maxCrouchSpeed = 1f;

    [Header("Animation")]
    public Animator animator;
    

    [Header("Walking")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 7f; 
    bool isSprint = false;

    [Header("Jump")]
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = .4f;
    public LayerMask groundMask;
    Vector3 velocity;
    bool isGrounded;

    [Header("Crouching")] 
    public AnimationCurve crouchCurve;
    public float transitionDuration = 0.1f;
    private bool isCrouch = false;
    private bool crouchState = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MovementInput();
        JumpInput();
        CrouchInput();
    }

    public void MovementInput()
    {
        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * maxWalkSpeed * Time.deltaTime);

        float currentSpeed = animator.GetFloat("Speed");
        float targetSpeed = 0f;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(isCrouch)
            {
                isSprint = false;
            }
            else
            {
                isSprint = true;
                maxWalkSpeed = sprintSpeed;
            }        
        }
        else
        {
            isSprint = false;
            maxWalkSpeed = walkSpeed;
        }

        //Animation
        if(Mathf.Abs(x)>.1f || Mathf.Abs(z)>.1f)
        {
            if(isSprint)
            {
                targetSpeed = 1f;
            }
            else
            {
                targetSpeed = 0.5f;
            }
        }
        else
        {
            targetSpeed = 0.0f;
        }

        float speed = Mathf.Lerp(currentSpeed, targetSpeed, 5f * Time.deltaTime);
        animator.SetFloat("Speed", speed);
        
    }

    public void JumpInput()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void CrouchInput()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            crouchState = !crouchState;
            StartCoroutine(CrouchLerp(crouchState));
        }
    }

     IEnumerator CrouchLerp(bool crouch)
    {
        float elapsedTime = 0f;
        float startHeight = controller.height;
        float targetHeight = crouch ? 1f : 2f;

        while (elapsedTime < transitionDuration)
        {
            controller.height = Mathf.Lerp(startHeight, targetHeight, crouchCurve.Evaluate(elapsedTime / transitionDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        isCrouch = crouch;
    }
}