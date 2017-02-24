using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {

        public float Speed = 8.0f;   // Speed when walking forward
        public int JumpForce = 100;
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public uint gravity = 30;

        public Camera cam;
        public MouseLook mouseLook = new MouseLook();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;

        private Vector3 direction, oldDirection;
        private bool isMoving, isSlowing, isWalking;
        private RaycastHit hit;
        private Ray ray;
        private Vector3 movingDirection;
        private Quaternion from;

        private GameObject mesh;
        private Animator anim;

        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);
            mesh = GameObject.Find("character_rig");
            anim = mesh.GetComponent<Animator>();

            direction = Vector3.down;
            isMoving = false;
            isSlowing = false;
            isWalking = true;
        }

        //private

        private void Update()
        {
            //isSlowing -> isMoving -> isWalking

            if (Input.GetButtonDown("Fire1"))
            {
                isWalking = false;
                isMoving = false;
                m_RigidBody.velocity = Vector3.zero;
                //Time.timeScale = 0.1F;
                isSlowing = true;
            }
            if (Input.GetButtonUp("Fire1"))
            {
                Time.timeScale = 1F;
                ray = new Ray(cam.transform.position, cam.transform.forward);
                Physics.Raycast(ray, out hit);
                movingDirection = hit.point - transform.position;
                direction = RoundDirection(-hit.normal);
                transform.rotation = Quaternion.Euler(RotateCharacter(direction));
                mouseLook.FollowPoint(transform, cam.transform, hit.point);
                isMoving = true;
            }
            if (isSlowing)
            {
                RotateView();
                if (Time.timeScale >= 0.1F)
                {
                    Time.timeScale -= 0.02f;
                }

            }
            if (isMoving)
            {
                isSlowing = false;
                RotateView();
                GroundCheck();
                if (!(!m_IsGrounded || m_GroundContactNormal != hit.normal))
                {
                    isMoving = false;
                    isWalking = true;
                }
            }
            if (isWalking)
            {
                RotateView();
                if (CrossPlatformInputManager.GetButtonDown("Jump") && !m_Jump)
                {
                    m_Jump = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                //¬ектор гравитации:
                m_RigidBody.AddForce(movingDirection.normalized * gravity * 0.6F, ForceMode.Impulse);
                Debug.Log("moving" + (movingDirection.normalized * gravity).magnitude.ToString());
                return;
            }
            if (isSlowing) return;

            if (isWalking)
            {
                GroundCheck();
                Vector2 input = GetInput();

                if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (m_IsGrounded))
                {
                    Vector3 desiredMove = Vector3.zero;

                    desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                    desiredMove.x = desiredMove.x * Speed;
                    desiredMove.z = desiredMove.z * Speed;
                    desiredMove.y = desiredMove.y * Speed;
                    if (m_RigidBody.velocity.sqrMagnitude < (Speed * Speed))
                    {
                        m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
                    }
                }

                if (m_IsGrounded)
                {
                    m_RigidBody.drag = 5f;
                    //ѕозвол€ет нам перемещатьс€ и прыгать одновременно
                    if (m_Jump)
                    {
                        m_RigidBody.drag = 0f;
                        m_RigidBody.velocity = RotateControl(direction);
                        m_RigidBody.AddForce(-1f * JumpForce * direction, ForceMode.Impulse);
                        m_Jumping = true;
                    }
                }
                //позвол€ет при сходе с уступа не падать вниз, а начать движение со скоростью, полученной ранее
                if (!m_IsGrounded && !m_Jump && !m_Jumping)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = RotateControl(direction);
                    m_Jumping = true;
                }
                m_Jump = false;

                //¬ектор гравитации:
                m_RigidBody.AddForce(direction * gravity, ForceMode.Acceleration);
                Debug.Log("gravity" + (direction * gravity).magnitude.ToString());
            }
        }



        private Vector3 RotateControl(Vector3 dir)
        {
            if (dir == Vector3.forward)
                return new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, 0f);
            if (dir == Vector3.down)
                return new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
            if (dir == Vector3.up)
                return new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
            if (dir == Vector3.back)
                return new Vector3(m_RigidBody.velocity.x, m_RigidBody.velocity.y, 0f);
            if (dir == Vector3.right)
                return new Vector3(0f, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
            if (dir == Vector3.left)
                return new Vector3(0f, m_RigidBody.velocity.y, m_RigidBody.velocity.z);
            else
                return new Vector3(0, 0, 0);
        }
        private Vector3 RotateCharacter(Vector3 dir)
        {
            if (dir == Vector3.forward)
                return new Vector3(-90, 0, 0);
            if (dir == Vector3.down)
                return new Vector3(0, 0, 0);
            if (dir == Vector3.up)
                return new Vector3(180, 0, 0);
            if (dir == Vector3.back)
                return new Vector3(90, 0, 0);
            if (dir == Vector3.right)
                return new Vector3(0, 0, 90);
            if (dir == Vector3.left)
                return new Vector3(0, 0, -90);
            else
                return new Vector3(0, 0, 0);
        }

        private Vector3 RoundDirection(Vector3 dir)
        {
            Vector3[] dirs = { Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
            float minAngle = 180;
            Vector3 minDir = Vector3.zero;
            for (int i = 0; i < dirs.Length; i++)
            {
                float angle = Vector3.Angle(dir, dirs[i]);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    minDir = dirs[i];
                }
            }
            return minDir;
        }

        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal"),
                    y = CrossPlatformInputManager.GetAxis("Vertical")
                };
            if (input.magnitude > 0)
                anim.SetBool("IsWalking", true);
            else
                anim.SetBool("IsWalking", false);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            //get the rotation before it's changed
            //float oldYRotation = 0f;
            //if (direction == Vector3.down)
            //{
            //    oldYRotation = transform.eulerAngles.y;
            //}
            //if (direction == Vector3.forward)
            //{
            //    oldYRotation = -1 * transform.eulerAngles.z;
            //}

            mouseLook.LookRotation(transform, cam.transform);

            //if (m_IsGrounded)
            //{
            //    // Rotate the rigidbody velocity to match the new direction that the character is looking
            //    if (direction == Vector3.down)
            //    {
            //        Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, -1 * direction);
            //        m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            //    }
            //    if (direction == Vector3.forward)
            //    {
            //        Quaternion velRotation = Quaternion.AngleAxis(-1 * transform.eulerAngles.z - oldYRotation, -1 * direction);
            //        m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            //    }
            //}
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius, direction,
                out hitInfo,
                ((m_Capsule.height / 2f) - m_Capsule.radius) + groundCheckDistance,
                ~0,
                QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
                //Debug.Log("grounded" + m_GroundContactNormal.ToString());
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = -1 * direction;
                //Debug.Log("not grounded");
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
