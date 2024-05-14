using System;
using Battle.Entity_Components;
using UnityEngine;
using UnityEngine.InputSystem;
using Utility;

namespace Battle
{
    public class BattlePlayer : MonoBehaviour
    {
        public ThirdPersonMovement movementController;
        [SerializeField] private Transform RayOrigin;
        [SerializeField] private float interactDistance = 2f;

        private void Awake()
        {
            movementController = GetComponent<ThirdPersonMovement>();
            Debug.Log(GetComponent<PlayerInput>().currentActionMap.name);
            //HotkeyInput.Instance.SubscribeToHotkeyEvent("Interact", Interact, this);
        }
        
        public void Interact(InputAction.CallbackContext context)
        {
            RaycastHit hit;
            if (Physics.Raycast(RayOrigin.position, movementController.cam.forward, out hit, interactDistance))
            {
                Debug.Log("Hit: " + hit.transform.name);
                var interactable = hit.transform.GetComponentInObjectOrAncestor<IInteractable>();
                Debug.Log("interactable: " + interactable.name);
                if (interactable != default(IInteractable))
                {
                    //Object is interactable.
                    interactable.Interact(this);
                }
                Debug.DrawRay(RayOrigin.position, movementController.cam.forward, Color.green,interactDistance);
            }
            else
                Debug.DrawRay(RayOrigin.position, movementController.cam.forward, Color.red,interactDistance);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            GetComponent<Attack>().PerformAttack();
        }
    }
}