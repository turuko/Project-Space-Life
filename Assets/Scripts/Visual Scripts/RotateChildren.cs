using UnityEngine;

namespace Visual_Scripts
{
    public class RotateChildren : MonoBehaviour
    {
        private float rotateSpeed = .5f;

        private float radius = .03f;

        private Vector3 positionOffset;

        public Transform[] ChildrenToRotate;

        private float angle;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < ChildrenToRotate.Length; i++)
            {
                positionOffset.Set(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                ChildrenToRotate[i].position = transform.position + positionOffset * (i+1);
                angle += Time.deltaTime * rotateSpeed;
            }
        }
    }
}
