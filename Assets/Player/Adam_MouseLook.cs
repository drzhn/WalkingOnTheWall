using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace Adam_Controller
{
    [Serializable]
    public class Adam_MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool lockCursor = true;


        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;

        public bool waschanged;
        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
            waschanged = false;

        }

        public void CameraUpDown(Transform camera)
        {
            //float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            //m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (waschanged == true)
            {
                waschanged = false;
                //m_CharacterTargetRot = character.localRotation;
                m_CameraTargetRot = camera.localRotation;
            }
            //character.localRotation = m_CharacterTargetRot;

            camera.localRotation = m_CameraTargetRot;

            UpdateCursorLock();
        }

        public void MouseRotation(Transform character, Transform camera)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
            UpdateCursorLock();
        }

        public void FollowPoint(Transform character, Transform camera, Vector3 point)
        {
            character.localRotation *= Quaternion.Euler(0f,
                Angle(character.forward,
                Vector3.ProjectOnPlane(point - character.position, character.up),
                character.up),
                0f);
            camera.localRotation *= Quaternion.Euler(
                Angle(
                    camera.forward,
                    Vector3.ProjectOnPlane(point - camera.position, camera.right),
                    camera.right),
                0f,
                0f);
            waschanged = true;
        }
        public float Angle(Vector3 from, Vector3 to, Vector3 axis)
        {
            axis = axis.normalized;
            if (Vector3.Cross(from, to).normalized == axis)
                return Vector3.Angle(from, to);
            else
                return -Vector3.Angle(from, to);
        }
        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if (!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
