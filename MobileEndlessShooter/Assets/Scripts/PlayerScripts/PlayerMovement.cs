using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 8.0f;
    [SerializeField] private float sideSpeed = 4.0f;
    private float sideInput;
    [SerializeField] private float minX = -17f;
    [SerializeField] private float maxX = 12f;
    Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        

        // Always move forward
         moveDirection = transform.forward * speed;
        
       

        // Move left only while button is held
        moveDirection += transform.right * (sideInput * sideSpeed);
        Vector3 nextPosition = rb.position + moveDirection * Time.fixedDeltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX); // makes sure the player's x pos is between values
        rb.MovePosition(nextPosition);
        
    }

    public void MoveRight()
    {
        sideInput = 1f;
    }

    public void StopMove()
    {
        sideInput = 0f;
    }
    
    public void MoveLeft()
    {
        sideInput = -1f;
    }
    
}
