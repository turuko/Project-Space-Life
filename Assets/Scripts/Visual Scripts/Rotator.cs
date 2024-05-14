using UnityEngine;

namespace Visual_Scripts
{
    public class Rotator : MonoBehaviour
    {
        public float rotateSpeed = 15f;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}
