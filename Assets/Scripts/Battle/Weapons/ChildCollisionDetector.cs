using System;
using UnityEngine;

namespace Battle.Weapons
{
    public class ChildCollisionDetector : MonoBehaviour
    {
        private ICollisionParent parent;

        private void Start()
        {
            // Start from the current object and traverse up the hierarchy
            Transform currentTransform = transform;
            while (currentTransform != null)
            {
                // Check if the current object has a component that implements ICollisionParent
                parent = currentTransform.GetComponent<ICollisionParent>();
            
                // If we found a component that implements ICollisionParent, break out of the loop
                if (parent != null)
                {
                    break;
                }

                // Move up to the parent in the hierarchy
                currentTransform = currentTransform.parent;
            }

            if (parent == null)
            {
                // Handle the case where no parent with ICollisionParent was found.
                // You can log an error, disable this component, or take other appropriate action.
                Debug.LogError("ICollisionParent not found in the ancestor objects.");
                // Optionally, you might want to disable this component in case the parent is not found.
                // gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            parent.OnCollision(other);
        }
    }
}