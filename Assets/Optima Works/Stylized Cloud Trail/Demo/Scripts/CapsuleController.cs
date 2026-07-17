using UnityEngine;

namespace OptimaWorks.StylizedCloudParticle
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class CapsuleController : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Jump Settings")] [SerializeField]
        private float jumpForce = 8f;

        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckDistance = 0.1f;

        [Header("Particle")] public ParticleSystem landingParticle;
        public ParticleSystem runningParticle;

        private Rigidbody rb;
        private CapsuleCollider col;

        private Vector3 moveDirection;
        private bool isRunning;
        private bool isGrounded;
        private bool previousGrounded;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<CapsuleCollider>();

            rb.freezeRotation = true;
            rb.useGravity = true;
        }

        void Update()
        {
            HandleInput();
            CheckGround();
            HandleLanding();
            HandleRunningParticles();
        }

        void FixedUpdate()
        {
            HandleMovement();
            ApplyJumpPhysics();
        }

        void HandleInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
            isRunning = Input.GetKey(KeyCode.LeftShift);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                Jump();
            }
        }

        void HandleMovement()
        {
            if (moveDirection.magnitude >= 0.1f)
            {
                float currentSpeed = isRunning ? runSpeed : moveSpeed;
                Vector3 moveVelocity = moveDirection * currentSpeed;
                rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }

        void Jump()
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        void ApplyJumpPhysics()
        {
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            else if (rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        void CheckGround()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            RaycastHit hit;
            isGrounded = Physics.Raycast(origin, Vector3.down, out hit, col.height / 2f + groundCheckDistance,
                groundLayer);
        }

        private bool _landingTriggered = false;

        void HandleLanding()
        {
            if (!_landingTriggered && isGrounded)
            {
                landingParticle.Play();
                _landingTriggered = true;
            }

            if (!isGrounded) _landingTriggered = false;
        }

        void HandleRunningParticles()
        {
            bool shouldRun = isGrounded && moveDirection.magnitude > 0.1f;

            if (shouldRun)
            {
                runningParticle.Play();
            }
            else
            {
                runningParticle.Stop();
            }
        }
    }
}