//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public PlayerGrapple m_leftGrapple, m_rightGrapple;
    private float m_rotationSmooth = 0.1f, m_turnSmoothVelocity;
    public Transform m_camera;
    private CameraDolly m_cameraDolly;
    private Vector3 m_respawnLocation;
    [SerializeField]
    private InputActionAsset m_playerControls;
    private InputAction m_leftFire, m_rightFire, m_cameraMovement;

    [Header("Grapple Hook Functional Variables")]
    public LayerMask m_grappleableObjects;
    public Transform m_leftHookOrigin, m_rightHookOrigin;
    public float m_maxRopeDistance, m_minRopeDistance, m_hookSpeed, m_hookRigidness, m_hookPullSlow, m_massScale;
    [Range(0.0f, 1.0f)]
    [Tooltip("The Higher this number the stronger the initial pull")]
    public float m_initialPull;

    [Header("Grapple Hook Visual Variables")]
    public int m_ropeQuality;
    public float m_damper, m_strength, m_velocity, m_waveCount, m_waveHeight;
    public AnimationCurve m_affectCurve;
    public Material m_chainMaterial;

    // Start is called before the first frame update
    void Start()
    {
        //set reference to camera
        m_cameraDolly = m_camera.GetComponent<CameraDolly>();

        //set values from to apply to grapples here... do it...
        UpdateGrappleHookFunction(m_maxRopeDistance, m_minRopeDistance, m_hookSpeed, m_hookRigidness, m_hookPullSlow,
            m_massScale, m_grappleableObjects, m_initialPull, m_leftHookOrigin, m_rightHookOrigin);
        UpdateGrappleHookVisual(m_ropeQuality, m_damper, m_strength, m_velocity, m_waveCount, m_waveHeight, m_affectCurve, m_chainMaterial);
    }
    private void OnEnable()
    {
        //set listeners for new input system to perform functions
        m_playerControls.Enable();
        var gameplayActionMap = m_playerControls.FindActionMap("PlayerControls");
        m_leftFire = gameplayActionMap.FindAction("LeftFire");
        m_rightFire = gameplayActionMap.FindAction("RightFire");
        m_cameraMovement = gameplayActionMap.FindAction("Look");
        m_leftFire.performed += FireLeftHook;
        m_leftFire.canceled += StopLeftHook;
        m_rightFire.performed += FireRightHook;
        m_rightFire.canceled += StopRightHook;
        m_cameraMovement.performed += MoveCamera;
    }
    private void OnDisable()
    {
        m_playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        //lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        //rotation of player
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_camera.eulerAngles.y, ref m_turnSmoothVelocity, m_rotationSmooth);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        //respawn of character
        if (transform.position.y <= 2)
            RespawnCharacter();

    }

    public void SetRespawn(Vector3 location)
    {
        m_respawnLocation = location;
    }
    public void RespawnCharacter()
    {
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = m_respawnLocation;
    }
    private void FireLeftHook(InputAction.CallbackContext obj)
    {
        m_leftGrapple.StartGrapple();
    }
    private void StopLeftHook(InputAction.CallbackContext obj)
    {
        m_leftGrapple.StopGrapple();
    }
    private void FireRightHook(InputAction.CallbackContext obj)
    {
        m_rightGrapple.StartGrapple();
    }
    private void StopRightHook(InputAction.CallbackContext obj)
    {
        m_rightGrapple.StopGrapple();
    }

    private void MoveCamera(InputAction.CallbackContext obj)
    {
        Vector2 input = m_cameraMovement.ReadValue<Vector2>();
        m_cameraDolly.MoveCamera(new Vector2(-input.y, input.x));
    }

    //TODO these can be made to use a struct holding all information for grapplehooks
    public void UpdateGrappleHookFunction(float maxRopeDistance, float minRopeDistance, float hookSpeed, float hookRigidness, float hookPullSlow,
        float massScale, LayerMask grappleableObjects, float initialPull, Transform leftHookOrigin, Transform rightHookOrigin)
    {
        m_leftGrapple.m_maxRopeDistance = m_rightGrapple.m_maxRopeDistance = maxRopeDistance;
        m_leftGrapple.m_minRopeDistance = m_rightGrapple.m_minRopeDistance = minRopeDistance;
        m_leftGrapple.m_hookSpeed = m_rightGrapple.m_hookSpeed = hookSpeed;
        m_leftGrapple.m_hookRigidness = m_rightGrapple.m_hookRigidness = hookRigidness;
        m_leftGrapple.m_hookPullSlow = m_rightGrapple.m_hookPullSlow = hookPullSlow;
        m_leftGrapple.m_massScale = m_rightGrapple.m_massScale = massScale;
        m_leftGrapple.m_grappleableObjects = m_rightGrapple.m_grappleableObjects = grappleableObjects;
        m_leftGrapple.m_initialPull = m_rightGrapple.m_initialPull = initialPull;
        m_leftGrapple.m_player = m_rightGrapple.m_player = transform;
        m_leftGrapple.m_camera = m_rightGrapple.m_camera = m_camera;
        m_leftGrapple.m_hookOrigin = leftHookOrigin;
        m_rightGrapple.m_hookOrigin = rightHookOrigin;
    }
    public void UpdateGrappleHookVisual(int ropeQuality, float damper, float strength, float velocity, float waveCount, float waveHeight, AnimationCurve affectCurve, Material chainMaterial)
    {
        RopeAnimation leftGrappleAnimation = m_leftGrapple.GetComponent<RopeAnimation>();
        RopeAnimation rightGrappleAnimation = m_rightGrapple.GetComponent<RopeAnimation>();

        leftGrappleAnimation.m_ropeQuality = rightGrappleAnimation.m_ropeQuality = ropeQuality;
        leftGrappleAnimation.m_damper = rightGrappleAnimation.m_damper = damper;
        leftGrappleAnimation.m_strength = rightGrappleAnimation.m_strength = strength;
        leftGrappleAnimation.m_velocity = rightGrappleAnimation.m_velocity = velocity;
        leftGrappleAnimation.m_waveCount = rightGrappleAnimation.m_waveCount = waveCount;
        leftGrappleAnimation.m_waveHeight = rightGrappleAnimation.m_waveHeight = waveHeight;
        leftGrappleAnimation.m_affectCurve = rightGrappleAnimation.m_affectCurve = affectCurve;
        leftGrappleAnimation.m_lineRenderer.material = rightGrappleAnimation.m_lineRenderer.material = chainMaterial;

    }
}
