//Authored By Daniel Bainbridge
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public PlayerGrapple m_leftGrapple, m_rightGrapple;
    private float m_rotationSmooth = 0.1f, m_turnSmoothVelocity;
    public Transform m_camera;
    private CameraDolly m_cameraDolly;
    public float m_maxSpeed = 500f;
    public LevelLoader m_respawnAnimation;

    //respawn
    private Vector3 m_respawnLocation;
    private Quaternion m_respawnRotation;
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

    [Header("IK Targets")]
    public Transform m_leftArmTarget;
    public Transform m_rightArmTarget;
    private Vector3 m_leftArmTargetOriginalPos, m_rightArmTargetOriginalPos;

    [Header("Sounds")]
    public AudioSource m_grappleShot;
    public AudioSource m_grappleWithdraw, m_playerDeath;

    //Animator Controls
    private Animator m_animator;
    private enum GroundedState
    {
        grounded,
        inAir
    }
    GroundedState m_currentState;
    void Awake()
    {
        //set reference to camera
        m_cameraDolly = m_camera.GetComponent<CameraDolly>();
        m_animator = GetComponent<Animator>();

        //set values from to apply to grapples here... do it...
        UpdateGrappleHookFunction(m_maxRopeDistance, m_minRopeDistance, m_hookSpeed, m_hookRigidness, m_hookPullSlow,
            m_massScale, m_grappleableObjects, m_initialPull, m_leftHookOrigin, m_rightHookOrigin);
        UpdateGrappleHookVisual(m_ropeQuality, m_damper, m_strength, m_velocity, m_waveCount, m_waveHeight, m_affectCurve, m_chainMaterial);
        //DisableForSeconds(3);
        m_leftArmTargetOriginalPos = m_leftArmTarget.localPosition;
        m_rightArmTargetOriginalPos = m_rightArmTarget.localPosition;
        m_currentState = GroundedState.grounded;
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
        m_currentState = GroundedState.grounded;
    }
    private void OnDisable()
    {
        m_playerControls.Disable();
        m_leftFire.performed -= FireLeftHook;
        m_leftFire.canceled -= StopLeftHook;
        m_rightFire.performed -= FireRightHook;
        m_rightFire.canceled -= StopRightHook;
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
        {
            m_playerDeath.Play();
            RespawnCharacter();
        }

        //check if the player is touching the ground
        if (Physics.Raycast(transform.position, Vector3.down, 0.15f))
            m_currentState = GroundedState.grounded;
        else
            m_currentState = GroundedState.inAir;


        // Updates blend tree + IK restraints on arms

        //Updates based to grounded animation position
        if (m_currentState == GroundedState.grounded)
        {
            m_animator.SetFloat("Still", Mathf.Lerp(m_animator.GetFloat("Still"), 0, 0.08f));
            m_animator.SetFloat("Grappling", Mathf.Lerp(m_animator.GetFloat("Grappling"), 0, 0.08f));
        }
        else if (!m_rightGrapple.IsGrappling() && !m_leftGrapple.IsGrappling())
        {
            m_animator.SetFloat("Still", Mathf.Lerp(m_animator.GetFloat("Still"), 1, 0.08f));
            m_animator.SetFloat("Grappling", Mathf.Lerp(m_animator.GetFloat("Grappling"), 0, 0.08f));
        }

        if (m_rightGrapple.IsGrappling())
        {
            m_animator.SetFloat("Grappling", Mathf.Lerp(m_animator.GetFloat("Grappling"), 1, 0.08f));
            m_animator.SetFloat("Still", Mathf.Lerp(m_animator.GetFloat("Still"), 1, 0.08f));
            m_rightArmTarget.position = Vector3.Lerp(m_rightArmTarget.position, m_rightGrapple.m_currentGrapplePosition, 0.08f);
            m_rightArmTarget.localPosition = new Vector3(Mathf.Clamp(m_rightArmTarget.position.x, 0, 50), m_rightArmTarget.localPosition.y, m_rightArmTarget.localPosition.z);
        }
        else
            //TODO Change this to 0 when you get the new rig
            m_rightArmTarget.localPosition = Vector3.Lerp(m_rightArmTarget.localPosition, m_rightArmTargetOriginalPos, 0.08f);

        if (m_leftGrapple.IsGrappling())
        {
            m_animator.SetFloat("Grappling", Mathf.Lerp(m_animator.GetFloat("Grappling"), 1, 0.08f));
            m_animator.SetFloat("Still", Mathf.Lerp(m_animator.GetFloat("Still"), 1, 0.08f));
            m_leftArmTarget.position = Vector3.Lerp(m_leftArmTarget.position, m_leftGrapple.m_currentGrapplePosition, 0.08f);
            m_leftArmTarget.localPosition = new Vector3(Mathf.Clamp(m_leftArmTarget.position.x, -50, 0), m_leftArmTarget.localPosition.y, m_leftArmTarget.localPosition.z);
        }
        else
            //TODO Change this to 0 when you get the new rig
            m_leftArmTarget.localPosition = Vector3.Lerp(m_leftArmTarget.localPosition, m_leftArmTargetOriginalPos, 0.08f);



    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            //Debug.Log("This hit an Obstacle");
            RespawnCharacter();
        }
    }

    public void SetRespawn(Vector3 location, Quaternion rotation)
    {
        m_respawnLocation = location;
        m_respawnRotation = rotation;
    }
    public void RespawnCharacter()
    {
        StartCoroutine(RespawnDelay());
    }
    private void FireLeftHook(InputAction.CallbackContext obj)
    {
        m_leftGrapple.StartGrapple();
        if (m_leftGrapple.IsGrappling())
            m_grappleShot.Play();
    }
    private void StopLeftHook(InputAction.CallbackContext obj)
    {
        if (m_leftGrapple.IsGrappling())
            m_grappleWithdraw.Play();
        m_leftGrapple.StopGrapple();
    }
    private void FireRightHook(InputAction.CallbackContext obj)
    {
        m_rightGrapple.StartGrapple();
        if (m_rightGrapple.IsGrappling())
            m_grappleShot.Play();
    }
    private void StopRightHook(InputAction.CallbackContext obj)
    {
        if (m_rightGrapple.IsGrappling())
            m_grappleWithdraw.Play();
        m_rightGrapple.StopGrapple();
    }

    private void LateUpdate()
    {
        Vector2 input = m_cameraMovement.ReadValue<Vector2>();
        input.Normalize();
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

    private IEnumerator DisableForSeconds(int secondsToWait)
    {
        OnDisable();
        yield return new WaitForSeconds(secondsToWait);
        OnEnable();
    }
    private IEnumerator RespawnDelay()
    {
        m_respawnAnimation.m_transition.Play("Crossfade_Start");
        yield return new WaitForSeconds(1);
        transform.SetPositionAndRotation(m_respawnLocation, m_respawnRotation);
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        m_leftGrapple.StopGrapple();
        m_rightGrapple.StopGrapple();
        m_cameraDolly.SetLookRotation(m_respawnRotation);

        yield return new WaitForSeconds(1);
        m_respawnAnimation.m_transition.Play("Crossfade_End");
    }
}
