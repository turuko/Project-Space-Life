using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Battle;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnVelocity;

    private Rigidbody rb;
    public Transform cam;

    private bool isGrounded = false;
    [SerializeField] private float groundCheckDistance;

    private Vector2 movementInput = Vector2.zero;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;
        cam = FindObjectOfType<CinemachineBrain>().transform;

    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        NormalMovement();
    }

    private void NormalMovement()
    {
        Vector3 movement = new Vector3(movementInput.x, 0f, movementInput.y).normalized;
        
        float targetAngle = cam.eulerAngles.y;
        
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        float targetSpeed = speed;
        float currentSpeed = isGrounded ? targetSpeed : Mathf.Lerp(rb.velocity.magnitude, targetSpeed, Time.deltaTime * 4f);

        currentSpeed = movement.z >= 0 ? currentSpeed : currentSpeed * 0.4f;

        if (movement.magnitude > 0.1f)
        {
            Vector3 movementDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * movement.z +
                                        Quaternion.Euler(0f, targetAngle + 90f, 0f) * Vector3.forward * movement.x;
            rb.MovePosition(rb.position + movementDirection.normalized * (currentSpeed * Time.fixedDeltaTime));
        }
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
    }

    private void CheckGrounded()
    {
        // Perform a raycast downwards to check if the character is grounded
        Vector3 rayOrigin = transform.position;
        if (Physics.Raycast(rayOrigin, Vector3.down, out _, groundCheckDistance, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(rayOrigin, Vector3.down, Color.green,groundCheckDistance);
            isGrounded = true;
        }
        else
        {
            Debug.DrawRay(rayOrigin, Vector3.down, Color.red,groundCheckDistance);
            isGrounded = false;
        }
    }
}
