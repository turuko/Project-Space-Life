using UnityEngine;

namespace Visual_Scripts
{
    public class RotateAround : MonoBehaviour
    {
        public float rotateSpeed;

        public Transform target;

        public Vector3 rotateAxis;

        private float angle;

        private void Update()
        {
            if (rotateAxis == Vector3.zero)
            {
                rotateAxis = Vector3.Cross(transform.position, target.position);

                float x = rotateAxis.x;
                float y = rotateAxis.y;
                if (rotateAxis.x < 0 || rotateAxis.y < 0)
                {
                    rotateAxis *= -1;
                }
            }
            transform.RotateAround(target.position, rotateAxis, rotateSpeed * Time.deltaTime);
        }
    }
}