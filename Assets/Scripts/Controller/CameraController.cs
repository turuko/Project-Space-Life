using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public class CameraController : MonoBehaviour
    {

        public Transform followTransform;
        public Transform cameraTransform;
    
        public float movementSpeed;
        public float movementTime;
        public float rotationAmount;
        public float panBorderThickness = 50f;
        public Vector3 zoomAmount;
        public float maxZoom = 25f;
        public float minZoom = 5f;
        public float minZoomY = 2.5f;

        public Vector3 newPosition;
        public Quaternion newRotation;
        public Vector3 newZoom;

        public Vector3 dragStartPosition;
        public Vector3 dragCurrentPosition;
        public Vector3 rotateStartPosition;
        public Vector3 rotateCurrentPosition;

        private void Start()
        {
            newPosition = transform.position;
            newRotation = transform.rotation;
            newZoom = cameraTransform.localPosition;
        }
        
        public void OnFreeCamera(InputAction.CallbackContext context)
        {
            newPosition = transform.position;
            followTransform = null;
        }

        public void OnPanCameraUp(InputAction.CallbackContext context)
        {
            newPosition += (transform.forward * movementSpeed);
        }
        
        public void OnPanCameraDown(InputAction.CallbackContext context)
        {
            newPosition += (transform.forward * -movementSpeed);
        }
        
        public void OnPanCameraLeft(InputAction.CallbackContext context)
        {
            newPosition += (transform.right * -movementSpeed);
        }
        
        public void OnPanCameraRight(InputAction.CallbackContext context)
        {
            newPosition += (transform.right * movementSpeed);
        }
        
        public void OnCameraZoomIn(InputAction.CallbackContext context)
        {
            newZoom += zoomAmount;
        }
        
        public void OnCameraZoomOut(InputAction.CallbackContext context)
        {
            newZoom -= zoomAmount;
        }
        
        public void OnRotateCameraLeft(InputAction.CallbackContext context)
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }

        public void OnRotateCameraRight(InputAction.CallbackContext context)
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }

        // Update is called once per frame
        void Update()
        {
            if (followTransform != null)
            {
                newPosition = followTransform.position;
                HandleKeyboardRotateAndZoom();
                HandleMouseRotateAndZoom();
                UpdateTransform();
            }
            else
            {
                HandleMouseInput();
                HandleMovementInput();
                UpdateTransform();
            }

            //HandleCloseRotation();
            cameraTransform.LookAt(transform);
        }

        private void HandleMouseInput()
        {
        
            if (Mouse.current.middleButton.wasPressedThisFrame)
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    dragStartPosition = ray.GetPoint(entry);
                }
            }

            if (Mouse.current.middleButton.isPressed)
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    dragCurrentPosition = ray.GetPoint(entry);

                    newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                }
            }

            var oldMovementSpeed = movementSpeed;
            movementSpeed = 0.025f;
            if (Mouse.current.position.ReadValue().y >= Screen.height - panBorderThickness)
            {
                newPosition += (transform.forward * movementSpeed);
            }
            
            if (Mouse.current.position.ReadValue().y <= panBorderThickness)
            {
                newPosition += (transform.forward * -movementSpeed);
            }
            
            if (Mouse.current.position.ReadValue().x >= Screen.width - panBorderThickness)
            {
                newPosition += (transform.right * movementSpeed);
            }
            
            if (Mouse.current.position.ReadValue().x <= panBorderThickness)
            {
                newPosition += (transform.right * -movementSpeed);
            }

            movementSpeed = oldMovementSpeed;
            HandleMouseRotateAndZoom();
        }

        private void HandleMouseRotateAndZoom()
        {
            if (Mouse.current.scroll.ReadValue().y != 0)
            {
                newZoom += Mouse.current.scroll.ReadValue().y * 2 * zoomAmount;
            }
        
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                rotateStartPosition = Mouse.current.position.ReadValue();
            }

            if (Mouse.current.rightButton.isPressed)
            {
                rotateCurrentPosition = Mouse.current.position.ReadValue();

                var diff = rotateStartPosition - rotateCurrentPosition;

                rotateStartPosition = rotateCurrentPosition;

                newRotation *= Quaternion.Euler(Vector3.up * (-diff.x / 5f));
            }
        
            newZoom.y = Mathf.Clamp(newZoom.y, minZoomY, maxZoom);
            newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);
        }

        private void HandleMovementInput()
        {
            var oldMovementSpeed = movementSpeed;
            movementSpeed = 0.025f;

            HandleKeyboardRotateAndZoom();
            movementSpeed = oldMovementSpeed;
        }

        private void UpdateTransform()
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.unscaledDeltaTime * movementTime);
            UpdateRotationAndZoom();
        }

        private void UpdateRotationAndZoom()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.unscaledDeltaTime * movementTime);
            cameraTransform.localPosition =
                Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.unscaledDeltaTime * movementTime);
        }

        private void HandleKeyboardRotateAndZoom()
        {
            newZoom.y = Mathf.Clamp(newZoom.y, minZoomY, maxZoom);
            newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);
        }

    
    }
}
