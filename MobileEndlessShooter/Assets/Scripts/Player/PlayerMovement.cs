using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    
    [SerializeField] private Joystick joystick;
    [SerializeField] private float joyStickDeadzone = 0.15f;
    [SerializeField] private float jumpThreshold = 0.65f;
    [SerializeField] private float slowThreshold = -0.4f;
    
    
    [SerializeField] Camera mainCamera;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] Score score;
    
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float slowSpeed = 8.0f;
    [SerializeField] private float sideSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 8.0f;
    //[SerializeField] private float screenMargin = 1.5f;
    
    private bool isGrounded = true;
    private bool requestJump;
    private bool joyStickUpPressed;
    
    private float sideInput;
    private float moveRotation = 25f;
    private float moveRotationSpeed = 40f;
    
    
    
    Quaternion startRotation;
    Quaternion targetRotation;
    
    private const string coinsHush = "Coins";
    
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        mainCamera =  GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        if (playerHealth.IsDead)
        {
            sideInput = 0f;
            return;
        }
        
        if (Mathf.Abs(sideInput) < joyStickDeadzone)
        {
            sideInput = 0f;
        }

        targetRotation = Quaternion.Euler(0f, 0f, -sideInput * moveRotation);

        // joystick up = jump
        if (joystick.Vertical > jumpThreshold)
        {
            if (!joyStickUpPressed)
            {
                requestJump = true;
                joyStickUpPressed = true;
            }
        }
        else
        {
            joyStickUpPressed = false;
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveRotationSpeed );

    }

    private void FixedUpdate()
    {
        if (playerHealth.IsDead) return;

        Movement();
        
    }
    
    
/* Shoots a raycast from the camera and plane to the left and right side,
 then makes clamp variables to use in the player movement */
    private void MovementClamp(out float minX, out float maxX)
    {
        // Set the left and right screen margins
        float leftPlayerMargin = 0.1f;
        float rightPlayerMargin = 0.9f;

        // Get the player's distance from the camera
        float distanceFromCamera =
            mainCamera.WorldToViewportPoint(rb.position).z;

        // Convert the left and right viewport limit into a world position
        Vector3 leftPoint = mainCamera.ViewportToWorldPoint(
            new Vector3(leftPlayerMargin, 0.5f, distanceFromCamera)
        );

        Vector3 rightPoint = mainCamera.ViewportToWorldPoint(
            new Vector3(rightPlayerMargin, 0.5f, distanceFromCamera)
        );

        // Store the minimum and maximum allowed X positions
        minX = leftPoint.x;
        maxX = rightPoint.x;
        
    }

    public void Movement()
    {
        // Get the minimum and maximum X positions of the player, this is where he can go
        MovementClamp(out float minX, out float maxX);

        // Get horizontal joystick input
        sideInput = joystick.Horizontal;

        // Ignore very small joystick movement
        if (Mathf.Abs(sideInput) < joyStickDeadzone)
        {
            sideInput = 0f;
        }

        // Calculate horizontal movement speed
        float xVelocity = sideInput * sideSpeed;

        // Calculate the player's next X position
        float nextX =
            rb.position.x + xVelocity * Time.fixedDeltaTime;

        // Keep the next X position inside the screen limits
        float clampedX = Mathf.Clamp(nextX, minX, maxX);

        // Convert the clamped position back into velocity
        xVelocity =
            (clampedX - rb.position.x) / Time.fixedDeltaTime;

        // Get the current Rigidbody velocity
        Vector3 velocity = rb.linearVelocity;

        // Slow the player when pulling the joystick down
        if (joystick.Vertical < slowThreshold && isGrounded)
        {
            velocity.z = slowSpeed;
        }
        else
        {
            // Move forward at normal speed
            velocity.z = speed;
        }

        // Apply horizontal movement
        velocity.x = xVelocity;
        
        //Apply final movement
        rb.linearVelocity = velocity;

        if (requestJump)
        {
            Jump();
            requestJump = false;
        }
    }
    
    public void Jump()
    {
        if (playerHealth.IsDead) return;
        if (!isGrounded) return;

        //sideInput = 0f;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);

        isGrounded = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == coinsHush)
        {
            Coins coins = other.gameObject.GetComponent<Coins>();
            score.UpdateScore(coins.coinValue);
            coins.RecycleCoin();
        }
        if (other.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }
}
