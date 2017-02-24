using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Adam_Controller
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Adam_Controller : MonoBehaviour
    {

        public float Speed = 8.0f;   // Speed when walking forward
        public int JumpForce = 100;
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public uint gravity = 30;
        public bool lockCursor = true;
        public Camera cam;


        private Rigidbody rigidbody;
        private CapsuleCollider capsule;
        private Vector3 groundContactNormal, groundContactPoint, currentNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

        private Vector3 direction;
        private bool isMoving, isSlowing, isWalking, isPaused, justPaused;

        private RaycastHit hit;
        private Ray ray;
        private Vector3 movingDirection, walkingDirection;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.freezeRotation = true;
            capsule = GetComponent<CapsuleCollider>();

            currentNormal = Vector3.up;
            direction = Vector3.down;
            walkingDirection = Vector3.zero;
            isMoving = false;
            isSlowing = false;
            isWalking = true;
            isPaused = false;
            justPaused = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            //isSlowing -> isMoving -> isWalking

            if (Input.GetMouseButtonDown(0))
            {
                isWalking = false;
                isMoving = false;
                rigidbody.velocity = Vector3.zero;
                Time.timeScale = 0.01F;
                rigidbody.drag = 0f;
                isSlowing = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Time.timeScale = 1F;
                isSlowing = false;
                movingDirection = cam.transform.forward;
                direction = movingDirection.normalized;
                m_IsGrounded = false;
                RotateCharacterToNormalCameraKeeping(-direction);
                isMoving = true;
            }
            if (isSlowing)
            {
                RotateCharacterFree();
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 0.5f);
            }
            if (isMoving)
            {
                RotateCharacterToNormal();
                GroundCheck();
                if (m_IsGrounded)
                {
                    isMoving = false;
                    isWalking = true;
                    direction = -groundContactNormal;
                }
            }
            if (isWalking)
            {
                GroundCheck();
                RotateCharacterToNormal();

                if (Input.GetButtonDown("Jump") && !m_Jump)
                {
                    m_Jump = true;
                }
            }
            //justPaused = false;
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                //Вектор гравитации:
                rigidbody.AddForce(direction * gravity, ForceMode.Acceleration);
                return;
            }
            if (isSlowing) return;

            if (isWalking)
            {
                //Вектор гравитации:
                rigidbody.AddForce(direction * gravity, ForceMode.Acceleration);
                Vector2 input = GetInput();
                if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (m_IsGrounded))
                {
                    walkingDirection = transform.forward * input.y + transform.right * input.x;
                    walkingDirection = Vector3.ProjectOnPlane(walkingDirection, groundContactNormal).normalized;

                    walkingDirection = walkingDirection * Speed;
                    if (rigidbody.velocity.sqrMagnitude < (Speed * Speed))
                    {
                        rigidbody.AddForce(walkingDirection, ForceMode.Impulse);
                    }

                }
                else
                {
                    walkingDirection = Vector3.zero;
                }

                if (m_IsGrounded)
                {
                    //Позволяет нам перемещаться и прыгать одновременно
                    if (m_Jump)
                    {
                        rigidbody.drag = 0f;
                        rigidbody.AddForce(-1f * JumpForce * direction - walkingDirection.normalized, ForceMode.Impulse);
                        m_Jumping = true;
                        m_Jump = false;
                    }
                }
                WallCheck(walkingDirection);
            }
        }

        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            return input;
        }

        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, capsule.radius + 0.1f, direction,
                out hit,
                ((capsule.height / 2f) - capsule.radius) + groundCheckDistance,
                ~0,
                QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.isTrigger && hit.collider.tag != "Movable")
                {
                    m_IsGrounded = true;
                    rigidbody.drag = 5f;
                    groundContactNormal = hit.normal;
                    groundContactPoint = hit.point;
                    direction = -groundContactNormal;
                }
            }
            else
            {
                m_IsGrounded = false;
                groundContactNormal = -1 * direction;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
        private void WallCheck(Vector3 move)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, move);
            if (Physics.Raycast(ray, out hit, capsule.height))
            {
                float alpha = Vector3.Angle(hit.normal, groundContactNormal) * Mathf.PI / 180;
                if (hit.distance <= capsule.height / 2 * Mathf.Tan(Mathf.PI / 2 - alpha) +
                    capsule.radius / (Mathf.Tan((Mathf.PI - alpha) / 2)) +
                    0.01f)
                {
                    if (!hit.collider.isTrigger && hit.collider.tag != "Movable")
                    {
                        //m_IsGrounded = true;
                        groundContactNormal = hit.normal;
                        groundContactPoint = hit.point;
                        direction = -groundContactNormal;
                        RotateCharacterToNormal();
                    }
                }
            }
            else
            {
                //Debug.Log("none");
            }
        }
        private void RotateCharacterToNormal()
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * 10;
            currentNormal = Vector3.Lerp(currentNormal, groundContactNormal, 10 * Time.deltaTime);
            Vector3 myForward = Vector3.Cross(transform.right, currentNormal);
            // align character to the new myNormal while keeping the forward direction:
            Quaternion targetRot = Quaternion.LookRotation(myForward, currentNormal);
            targetRot *= Quaternion.Euler(0f, yRot, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 10 * Time.deltaTime);

            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * 2;
            cam.transform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);
        }

        private void RotateCharacterToNormalCameraKeeping(Vector3 normal)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * 10;

            Vector3 oldMyNormal = currentNormal;
            currentNormal = normal;
            Vector3 myForward = Vector3.Cross(transform.right, currentNormal);
            Quaternion targetRot = Quaternion.LookRotation(myForward, currentNormal);
            targetRot *= Quaternion.Euler(0f, yRot, 0f);
            transform.rotation = targetRot;

            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * 2;
            cam.transform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);

            cam.transform.localRotation *= Quaternion.Euler(-Angle(
                Vector3.ProjectOnPlane(oldMyNormal, cam.transform.right),
                Vector3.ProjectOnPlane(currentNormal, cam.transform.right),
                cam.transform.right), 0f, 0f);
        }
        private void RotateCharacterFree()
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * 2;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * 2;

            transform.localRotation *= Quaternion.Euler(0f, yRot, 0f);
            cam.transform.localRotation *= Quaternion.Euler(-xRot, 0f, 0f);
        }
        private float Angle(Vector3 from, Vector3 to, Vector3 axis)
        {
            axis = axis.normalized;
            if (Vector3.Cross(from, to).normalized == axis)
                return Vector3.Angle(from, to);
            else
                return -Vector3.Angle(from, to);
        }

        public void ChangeDirection(Vector3 newDirection)
        {
            rigidbody.drag = 0f;
            rigidbody.velocity = Vector3.zero;
            movingDirection = newDirection;
            direction = newDirection;
            m_IsGrounded = false;
            //RotateCharacterToNormalCameraKeeping(-direction);
            isMoving = true;
        }
    }
}
