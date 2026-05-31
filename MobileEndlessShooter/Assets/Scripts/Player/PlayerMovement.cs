using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float sideSpeed = 4.0f;
    [SerializeField] private float jumpheight = 8f;
    private bool isGrounded = true;
    private float sideInput;
    [SerializeField] private float minX = -17f;
    [SerializeField] private float maxX = 12f;
    Vector3 moveDirection = Vector3.zero;
    
    private float moveRotation = 25f;
    private float moveRotationSpeed = 40f;
    Quaternion startRotation;
    Quaternion targetRotation;
    private const string coinsHush = "Coins";
    
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] Score score;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
        
        startRotation = transform.rotation;
        targetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        
        if (playerHealth.IsDead) return;

        Vector3 velocity = rb.linearVelocity;

        velocity.z = speed;
        velocity.x = sideInput * sideSpeed;

        rb.linearVelocity = velocity;

        Vector3 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        rb.position = clampedPosition;
       
        
    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * moveRotationSpeed );
    }

    public void MoveRight()
    {
        if(playerHealth.IsDead)return;
        sideInput = 1f;
        targetRotation = Quaternion.Euler(0f, 0f, -moveRotation);
    }

    public void StopMove()
    {
        sideInput = 0f;
        targetRotation = startRotation;
    }
    
    public void MoveLeft()
    {
        if(playerHealth.IsDead)return;
        sideInput = -1f;
        targetRotation = Quaternion.Euler(0f, 0f, moveRotation);
    }

    public void Jump()
    {
        if (playerHealth.IsDead) return;
        if (!isGrounded) return;

        sideInput = 0f;

        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpheight, ForceMode.Impulse);

        isGrounded = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == coinsHush)
        {
            Coins coins = other.gameObject.GetComponent<Coins>();
            score.UpdateScore(coins.coinValue);
            coins.RecycleCoin();
        }
    }
}
