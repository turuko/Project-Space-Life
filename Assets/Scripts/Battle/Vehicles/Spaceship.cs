using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle.Vehicles
{
    public class Spaceship : Mount
    {
        
        [Header("=== Roll Settings ===")]
        public float maxRollSpeed = 30f;  // Maximum roll angle in degrees.
        public float rollAccelerationRate = 15f;  // Rate at which the spaceship rolls.
        public float rollSmoothTime = .5f;  // Smoothing time for roll interpolation.
        
        
        [Header("=== Speed Settings ===")]
        public float maxSpeed = 1000f;  // Maximum speed of the spaceship.
        public float accelerationRate = 100f; // Rate at which the spaceship accelerates/decelerates.
        public float smoothTime = 0.1f;  // Smoothing time for position interpolation.
        
        [Header("=== Pitch and Yaw Settings ===")]
        public float maxPitchYawSpeed = 30f;  // Maximum pitch and yaw speed.
        public float pitchYawAccelerationRate = 15f;  // Rate at which the spaceship pitches and yaws.
        public float pitchYawSmoothTime = 0.1f;

        private float currentSpeed = 0f;  // Current speed of the spaceship.
        private float targetSpeed = 0f;
        private float targetSpeedAccelerationRate = 150f;
        private Vector3 currentVelocity = Vector3.zero;
        
        private float currentRollSpeed = 0f;  // Current roll angle.
        private float targetRollSpeed = 0f;  // Target roll angle when A or D keys are pressed.
        private float currentRollVelocity = 0f;  // Current roll angular velocity.
        
        private Vector2 currentPitchYawSpeed = Vector2.zero;  // Current pitch and yaw speed.
        private Vector2 targetPitchYawSpeed = Vector2.zero;  // Target pitch and yaw speed based on input.
        private Vector2 currentPitchYawVelocity = Vector2.zero;  // Current pitch and yaw velocity.


        private float thrust1D;
        private float roll1D;
        private Vector2 pitchYaw;

        [Header("=== Debug UI ===")] 
        public TextMeshProUGUI currentSpeedUI;
        public TextMeshProUGUI targetSpeedUI;

        private void Start()
        {
        }

        private void Update()
        {
            if (!mountEnabled)
                return;
            HandleMovement();
            currentSpeedUI.text = "Current speed: " + currentSpeed;
            targetSpeedUI.text = "Target speed: " + targetSpeed;
        }

        private void HandleMovement()
        {
            // Handle acceleration and deceleration towards the target speed.
            if (thrust1D > 0)
            {
                targetSpeed = Mathf.Clamp(targetSpeed + (thrust1D * targetSpeedAccelerationRate * Time.deltaTime), 0f, maxSpeed);
            }
            else if (thrust1D < 0)
            {
                targetSpeed = Mathf.Clamp(targetSpeed + (thrust1D * targetSpeedAccelerationRate * Time.deltaTime), 0f, maxSpeed);
            }

            // Interpolate the current speed towards the target speed.
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelerationRate * Time.deltaTime);

            // Calculate the forward movement direction based on the spaceship's rotation.
            Vector3 forwardDirection = transform.forward;

            // Calculate the target position based on current speed.
            Vector3 targetPosition = transform.position + forwardDirection * (currentSpeed * Time.deltaTime);

            // Smoothly interpolate between the current position and target position.
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

            // Handle rolling using A and D keys.
            if (roll1D != 0f)
            {
                targetRollSpeed = Mathf.Clamp(targetRollSpeed + (-roll1D * rollAccelerationRate * Time.deltaTime), -maxRollSpeed, maxRollSpeed);
            }
            else
            {
                targetRollSpeed = Mathf.SmoothDamp(targetRollSpeed, 0f, ref currentRollVelocity, rollSmoothTime);
            }

            // Apply roll to the spaceship's rotation.
            currentRollSpeed = Mathf.MoveTowards(currentRollSpeed, targetRollSpeed, rollAccelerationRate * Time.deltaTime);
            transform.Rotate(Vector3.forward * (currentRollSpeed * Time.deltaTime));

            // Handle pitch and yaw using pitchYaw vector.
            if (pitchYaw != Vector2.zero)
            {
                targetPitchYawSpeed = Vector2.ClampMagnitude(targetPitchYawSpeed + pitchYaw * (pitchYawAccelerationRate * Time.fixedDeltaTime), maxPitchYawSpeed);
            }
            else
            {
                targetPitchYawSpeed = Vector2.SmoothDamp(targetPitchYawSpeed, Vector2.zero, ref currentPitchYawVelocity, pitchYawSmoothTime);
            }
            
            // Apply pitch and yaw speed to the spaceship's rotation.
            currentPitchYawSpeed = Vector2.MoveTowards(currentPitchYawSpeed, targetPitchYawSpeed, pitchYawAccelerationRate * Time.fixedDeltaTime);
            transform.Rotate(Vector3.up * (currentPitchYawSpeed.x * Time.deltaTime)); // Yaw
            transform.Rotate(Vector3.left * (currentPitchYawSpeed.y * Time.deltaTime)); // Pitch


        }

        public override void Interact(BattlePlayer player)
        {
            base.Interact(player);
            GetComponent<PlayerInput>().enabled = true;
        }
        
        #region Input Methods

        public void GetThrust(InputAction.CallbackContext context)
        {
            thrust1D = context.ReadValue<float>();
        }
        
        /*public void GetStrafe(InputAction.CallbackContext context)
        {
            strafe1D = context.ReadValue<float>();
        }
        
        public void GetUpDown(InputAction.CallbackContext context)
        {
            upDown1D = context.ReadValue<float>();
        }*/
        
        public void GetRoll(InputAction.CallbackContext context)
        {
            roll1D = context.ReadValue<float>();
        }
        
        public void GetPitchYaw(InputAction.CallbackContext context)
        {
            pitchYaw = context.ReadValue<Vector2>();
        }

        #endregion
    }
}