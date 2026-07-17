using UnityEngine;

namespace OptimaWorks.StylizedCloudParticle
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 _offset;

        void Start()
        {
            _offset = transform.position - target.position;
        }

        void Update()
        {
            transform.position = target.position + _offset;
        }
    }
}