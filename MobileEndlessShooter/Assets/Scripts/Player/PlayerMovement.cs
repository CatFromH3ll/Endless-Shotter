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
    [SerializeField] private float screenMargin = 1.5f;
    
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
    

    private void MovementClamp(out float minX, out float maxX)
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        float playerScreenY = mainCamera.WorldToViewportPoint(transform.position).y;
        
        Ray leftRay = mainCamera.ViewportPointToRay(new Vector3(0f, playerScreenY, 0));
        Ray rightRay = mainCamera.ViewportPointToRay(new Vector3(1f, playerScreenY, 0));
        
        groundPlane.Raycast(leftRay, out float leftDistance);
        groundPlane.Raycast(rightRay, out float rightDistance);
        
        //Vector3 leftEdge = new Vector3(screenWidth, transform.position.y, transform.position.z);
        //Vector3 rightEdge = new Vector3(screenWidth, transform.position.y, transform.position.z);
        
        Vector3 leftEdge = leftRay.GetPoint(leftDistance);
        Vector3 rightEdge = rightRay.GetPoint(rightDistance);
        
        minX = leftEdge.x + screenMargin;
        maxX = rightEdge.x - screenMargin;
        
    }

    public void Movement()
    {
        //calculate how far the player can go on X axis
        MovementClamp(out float minX, out float maxX);
        sideInput = joystick.Horizontal;
        
        if (Mathf.Abs(sideInput) < joyStickDeadzone)
        {
            sideInput = 0f;
        }
        float xVelocity = sideInput * sideSpeed;

        float nextX = rb.position.x + xVelocity * Time.fixedDeltaTime;

        // If next frame would go outside edge, stop X movement
        if (nextX < minX)
        {
            xVelocity = (minX - rb.position.x) / Time.fixedDeltaTime;
        }
        else if (nextX > maxX)
        {
            xVelocity = (maxX - rb.position.x) / Time.fixedDeltaTime;
        }
        
        //calculate speed
        Vector3 velocity = rb.linearVelocity;
            
        if (joystick.Vertical < slowThreshold && isGrounded)
        {
            velocity.z = slowSpeed;
        }
        else
        {
            velocity.z = speed;
        }
        velocity.x = xVelocity;
        
        rb.linearVelocity = velocity;
        
        //Condition to jump calculated from Update input
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
