using UnityEngine;

namespace Utility
{
    public static class TransformExtensions
    {
        public static T GetComponentInObjectOrAncestor<T>(this Transform transform)
        {
            T component = transform.GetComponent<T>();

            if (component != null)
            {
                return component;
            }

            Transform parent = transform.parent;

            while (parent != null)
            {
                component = parent.GetComponent<T>();

                if (component != null)
                {
                    return component;
                }

                parent = parent.parent;
            }

            return default; // Component not found in the object or its ancestors.
        }
    }
}