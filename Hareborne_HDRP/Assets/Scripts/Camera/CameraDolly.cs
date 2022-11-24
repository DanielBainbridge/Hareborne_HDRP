//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CameraDolly : MonoBehaviour
{
    [Header("What The Camera Looks At:")]
    public Transform m_cameraTarget;
    private Vector3 m_targetPosition, m_previousTargetPosition;
    private Vector2 m_orbitAngles = new Vector2(45f, 0f);
    private Camera m_camera;

    [Header("Variables To Change:")]
    public float m_cameraSensitivity = 90f;
    public float m_cameraPOV = 50f;
    [SerializeField, Min(0f)]
    public float m_focusRadius = 1f;
    [SerializeField, Range(0f, 1f)]
    public float m_focusCentering = 0.5f;
    [SerializeField, Range(-89f, 89f)]
    public float m_minVerticalAngle = -75f, m_maxVerticalAngle = 75f;
    [SerializeField, Min(0f)]
    public bool m_autoAlign = true;
    public float m_autoAlignDelay = 5f;
    [SerializeField, Range(0f, 90f)]
    public float alignSmoothRange = 45f;

    private Quaternion m_lookRotation;
    private float m_lastManualRotationTime;
    public float m_cameraDistance = 12;
    [SerializeField]
    private Slider sensitivitySlider;


    private void OnValidate()
    {
        if (m_maxVerticalAngle < m_minVerticalAngle)
        {
            m_maxVerticalAngle = m_minVerticalAngle;
        }
    }
    // Custom start call to be used by the Game manager, ****REQUIRED TO RUN TO PLAY THE GAME****
    public void Initialise()
    {

        m_camera = transform.GetComponent<Camera>();
        transform.position = m_cameraTarget.position;

        transform.localRotation = Quaternion.Euler(m_orbitAngles);
        m_camera.fieldOfView = m_cameraPOV;

        SetCameraSensitivity();
    }

    private void LateUpdate()
    {
        UpdateCameraTarget();
        
        Vector3 lookDirection = m_lookRotation * Vector3.forward;
        transform.localPosition = m_targetPosition;
        Vector3 lookPosition = (m_targetPosition - lookDirection * m_cameraDistance);

        Vector3 lookRight = m_lookRotation * Vector3.right;



        //calculations for boxcast to work with focus radius
        Vector3 rectOffset = lookDirection * m_camera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = m_targetPosition - new Vector3(m_cameraTarget.localPosition.x,0,0);
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        //check for 
        if (Physics.Raycast(castFrom, lookRight, out RaycastHit hitRight, m_cameraTarget.localPosition.x, ~2))
        {
            castFrom = castFrom - lookRight * hitRight.distance;
        }

        //check for collision behind camera
        if (Physics.BoxCast(castFrom, CameraHalfExtents, castDirection, out RaycastHit hit, m_lookRotation, castDistance))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }
        transform.SetPositionAndRotation(lookPosition, m_lookRotation);
    }

    private void UpdateCameraTarget()
    {
        m_previousTargetPosition = m_targetPosition;
        Vector3 targetPosition = m_cameraTarget.position;
        if (m_focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPosition, m_targetPosition);
            float t = 1f;
            if (distance > m_focusRadius)
            {
                t = Mathf.Min(t, m_focusRadius / distance);
            }
            m_targetPosition = Vector3.Lerp(targetPosition, m_targetPosition, t);
        }
        else
            m_targetPosition = m_cameraTarget.position;
    }

    private bool ManualRotation(Vector2 input)
    {
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e | input.y > e)
        {
            m_orbitAngles += (m_cameraSensitivity * Time.unscaledDeltaTime * input);
            m_lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }
    private bool AutomaticRotation()
    {
        if (Time.unscaledTime - m_lastManualRotationTime < m_autoAlignDelay)
            return false;
        Vector2 movement = new Vector2(m_targetPosition.x - m_previousTargetPosition.x, m_targetPosition.y - m_previousTargetPosition.y);
        float movementDeltaSqr = movement.SqrMagnitude();
        if (movementDeltaSqr < 0.00001f)
            return false;
        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(m_orbitAngles.y, headingAngle));
        float rotationChange = m_cameraSensitivity * Time.unscaledDeltaTime;
        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        m_orbitAngles.y = Mathf.MoveTowardsAngle(m_orbitAngles.y, headingAngle, rotationChange);
        return true;
    }
    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
    private void ClampAngles()
    {
        m_orbitAngles.x = Mathf.Clamp(m_orbitAngles.x, m_minVerticalAngle, m_maxVerticalAngle);
        if (m_orbitAngles.y < 0f)
            m_orbitAngles.y += 360f;
        else if (m_orbitAngles.y >= 360f)
            m_orbitAngles.y -= 360f;
    }
    private Vector3 CameraHalfExtents
    {
        get
        {
            Vector3 halfextents;
            halfextents.y = m_camera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * m_camera.fieldOfView);
            halfextents.x = halfextents.y * m_camera.aspect;
            halfextents.z = 0f;
            return halfextents;
        }
    }
    public void MoveCamera(Vector2 moveCoordinates)
    {
        bool autoRotate = false;
        if (m_autoAlign)
        {
            autoRotate = AutomaticRotation();
        }
        if (ManualRotation(moveCoordinates) || autoRotate)
        {
            ClampAngles();
            m_lookRotation = Quaternion.Euler(m_orbitAngles);
        }
        else
            m_lookRotation = transform.localRotation;
    }
    public void SetLookRotation(Quaternion lookRotation)
    {
        m_lookRotation = lookRotation;
        Vector3 lookDirection = m_lookRotation * Vector3.forward;
        transform.localPosition = m_targetPosition;
        Vector3 lookPosition = (m_targetPosition - lookDirection * m_cameraDistance);



        //calculations for boxcast to work with focus radius
        Vector3 rectOffset = lookDirection * m_camera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = m_targetPosition;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;

        //check for collision behind camera
        if (Physics.BoxCast(castFrom, CameraHalfExtents, castDirection, out RaycastHit hit, m_lookRotation, castDistance))
        {
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, m_lookRotation);
    }
    public void SetCameraSensitivity()
    {
        if (PlayerPrefs.GetFloat("CamSpeed") == 0)
            PlayerPrefs.SetFloat("CamSpeed", sensitivitySlider.value);
        m_cameraSensitivity = PlayerPrefs.GetFloat("CamSpeed");
    }
}
