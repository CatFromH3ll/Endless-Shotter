using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] public float speed = 8.0f;
    [SerializeField] public float sideSpeed = 4.0f;
    private bool canMoveLeft;
    private bool canMoveRight;
    [SerializeField] private float minX = -17f;
    [SerializeField] private float maxX = 12f;

    private void Start()
    {
        rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection = Vector3.zero;

        // Always move forward
        moveDirection += transform.forward * speed;

        // Move left only while button is held
        if (canMoveLeft)
        {
            moveDirection += -transform.right * sideSpeed;
        }

        // Move right only while button is held
        if (canMoveRight)
        {
            moveDirection += transform.right * sideSpeed;
        }

        Vector3 nextPosition = rb.position + moveDirection * Time.fixedDeltaTime;
        nextPosition.x = Mathf.Clamp(nextPosition.x, minX, maxX); // makes sure the player's x pos is between values
        rb.MovePosition(nextPosition);
    }

    public void MoveRight()
    {
        canMoveRight = true;
    }
    public void StopMoveRight()
    {
        canMoveRight = false;
    }

    
    public void StopMoveLeft()
    {
        canMoveLeft = false;
    }
    
    public void MoveLeft()
    {
        canMoveLeft = true;
    }
    
}
