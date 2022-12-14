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
    private float m_maxSpeed = 60f;
    public LevelLoader m_respawnAnimation;

    //respawn
    private Vector3 m_respawnLocation;
    private Quaternion m_respawnRotation;
    [SerializeField]
    private InputActionAsset m_playerControls;
    private InputAction m_leftFire, m_rightFire, m_cameraMovement;
    private PlayerInput m_playerInput;
    private bool m_inputActive = true;
    public PauseMenu m_pauseMenu;

    [Header("Grapple Hook Functional Variables")]
    public LayerMask m_grappleableObjects;
    public Transform m_leftHookOrigin, m_rightHookOrigin;
    public float m_maxRopeDistance, m_minRopeDistance, m_hookSpeed, m_grappleCooldown, m_hookRigidness, m_hookPullSlow, m_massScale;
    [Range(0.0f, 1000.0f)]
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

    [HideInInspector]
    public Transform m_nextCheckpoint = null;
    [Header("Wrong Way")]
    [SerializeField]
    private GameObject m_wrongWayObject;
    private bool m_wrongWayToggle = false;

    [Header("Speed Lines")]
    [SerializeField] private GameObject m_speedLines;
    private Renderer m_speedLineRender;

    //Lists of sounds will choose random sound from list when played. Individual will play one sound.
    [Header("Sounds:")]
    [Header("Death")]
    public List<VFX> m_deathSounds = new List<VFX>();
    [Header("Grunts")]
    public List<VFX> m_gruntSounds = new List<VFX>();
    [Header("Voice Lines")]
    public List<VFX> m_voiceLineSounds = new List<VFX>();
    [Header("Chain Sound")]
    public VFX m_grappleLaunch;
    public VFX m_grappleRetract;
    public VFX m_grappleHit;
    [Header("Wind Sound")]
    public VFX m_wind;

    //Animator Controls
    private Animator m_animator;
    private Rigidbody m_rigidBody;
    private float m_lastSoundPlayed = 2.0f;

    //private variables
    private bool m_isRespawning = false;
    private enum GroundedState
    {
        grounded,
        inAir
    }
    GroundedState m_currentState;

    public void Initialise()
    {
        //set reference to camera
        m_cameraDolly = m_camera.GetComponent<CameraDolly>();
        m_animator = GetComponent<Animator>();
        m_playerInput = FindObjectOfType<MenuUI>().m_input;

        m_speedLineRender = m_speedLines.GetComponent<Renderer>();

        //set values from to apply to grapples here... do it...
        UpdateGrappleHookFunction(m_maxRopeDistance, m_minRopeDistance, m_hookSpeed, m_grappleCooldown, m_hookRigidness, m_hookPullSlow,
            m_massScale, m_grappleableObjects, m_initialPull, m_leftHookOrigin, m_rightHookOrigin, m_grappleHit);
        UpdateGrappleHookVisual(m_ropeQuality, m_damper, m_strength, m_velocity, m_waveCount, m_waveHeight, m_affectCurve, m_chainMaterial);
        StartCoroutine(DisableForSeconds(3));
        m_leftArmTargetOriginalPos = m_leftArmTarget.localPosition;
        m_rightArmTargetOriginalPos = m_rightArmTarget.localPosition;
        m_currentState = GroundedState.grounded;
        m_rigidBody = GetComponent<Rigidbody>();

        // m_playerControls is a reference to the InputActionAsset
        // m_leftFire, m_rightFire and m_cameraMovement are all inidividual actions

        // first we have to enable the new system inside of code only one action map can be enabled at a time so it is important to disable it again if you switch controls
        m_playerControls.Enable();

        // here we get get our ActionMap
        var gameplayActionMap = m_playerControls.FindActionMap("PlayerControls");
        m_playerInput.currentActionMap = gameplayActionMap;

        // here we find actions on this action map and assign them to our individual actions
        m_playerInput.actions["LeftFire"].performed += FireLeftHook;
        m_playerInput.actions["LeftFire"].canceled += StopLeftHook;
        m_playerInput.actions["RightFire"].performed += FireRightHook;
        m_playerInput.actions["RightFire"].canceled += StopRightHook;
        m_currentState = GroundedState.grounded;
        Cursor.lockState = CursorLockMode.Locked;


    }

    // Update is called once per frame
    void Update()
    {
        if (m_pauseMenu._ispaused || !m_inputActive)
        {
            return;
        }
        else
        {
            //lock cursor to the center of the screen
            Cursor.lockState = CursorLockMode.Locked;

            //rotation of player
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_camera.eulerAngles.y, ref m_turnSmoothVelocity, m_rotationSmooth);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            //respawn of character
            if (transform.position.y <= 2)
            {
                RespawnCharacter();
            }

            float rayLength = 2.0f;
            //check if the player is touching the ground
            if (Physics.Raycast(transform.position + (transform.up * 0.5f), -transform.up, rayLength))
                m_currentState = GroundedState.grounded;
            else
                m_currentState = GroundedState.inAir;


            // Updates blend tree + IK restraints on arms

            //Updates based to grounded animation position
            if (m_currentState == GroundedState.grounded)
            {
                //Set Values for blend tree to animate
                m_animator.SetFloat("Still", Mathf.Clamp(Mathf.Lerp(m_animator.GetFloat("Still"), 0, 0.08f), 0, 1));
                m_animator.SetFloat("Grappling", Mathf.Clamp(Mathf.Lerp(m_animator.GetFloat("Grappling"), 0, 0.08f), 0, 1));
            }
            else if (!m_rightGrapple.IsGrappling() && !m_leftGrapple.IsGrappling())
            {
                //Set Values for blend tree to animate
                m_animator.SetFloat("Still", Mathf.Clamp(Mathf.Lerp(m_animator.GetFloat("Still"), 1, 0.08f), 0, 1));
                m_animator.SetFloat("Grappling", Mathf.Clamp(Mathf.Lerp(m_animator.GetFloat("Grappling"), 0, 0.08f), 0, 1));
            }

            if (m_rightGrapple.IsGrappling())
            {
                //Set Values for blend tree to animate
                m_animator.SetFloat("Grappling", Mathf.Clamp(Mathf.Lerp(m_animator.GetFloat("Grappling"), 1, 0.08f), 0, 1));
                m_animator.SetFloat("Still", Mathf.Clamp(Mathf.Lerp(m_animator.GetFloat("Still"), Mathf.Clamp((m_rigidBody.velocity.magnitude / m_maxSpeed), 0, 1), 0.08f), 0, 1));
                //Hands IK point to position
                m_rightArmTarget.position = Vector3.Lerp(m_rightArmTarget.position, m_rightGrapple.m_currentGrapplePosition, 0.08f);
                m_rightArmTarget.localPosition = new Vector3(Mathf.Clamp(m_rightArmTarget.position.x, 0, 50), m_rightArmTarget.localPosition.y, m_rightArmTarget.localPosition.z);
            }
            else
                //TODO Change this to 0 when you get the new rig
                m_rightArmTarget.localPosition = Vector3.Lerp(m_rightArmTarget.localPosition, m_rightArmTargetOriginalPos, 0.08f);

            if (m_leftGrapple.IsGrappling())
            {
                //Set Values for blend tree to animate
                m_animator.SetFloat("Grappling", Mathf.Lerp(m_animator.GetFloat("Grappling"), 1, 0.08f));
                m_animator.SetFloat("Still", Mathf.Lerp(m_animator.GetFloat("Still"), Mathf.Clamp((m_rigidBody.velocity.magnitude / m_maxSpeed), 0, 1), 0.08f));
                //Hands IK point to position
                m_leftArmTarget.position = Vector3.Lerp(m_leftArmTarget.position, m_leftGrapple.m_currentGrapplePosition, 0.08f);
                m_leftArmTarget.localPosition = new Vector3(Mathf.Clamp(m_leftArmTarget.position.x, -50, 0), m_leftArmTarget.localPosition.y, m_leftArmTarget.localPosition.z);
            }
            else
                //TODO Change this to 0 when you get the new rig
                m_leftArmTarget.localPosition = Vector3.Lerp(m_leftArmTarget.localPosition, m_leftArmTargetOriginalPos, 0.08f);
        }
        if (m_lastSoundPlayed <= 1.4f)
        {
            m_lastSoundPlayed += Time.deltaTime;
        }

        // check if facing correct way to next checkpoint
        Vector3 posdif = transform.position - m_nextCheckpoint.position;
        float angleForLakitu = -Mathf.Atan2(posdif.z, posdif.x) * Mathf.Rad2Deg;
        //using the direction cause player to rotate to that direction
        //calculate the the angle between facing forward and the way we want to be facing and convert to a value between 0 and 1
        float angleBetweenForwardAndDesired = Vector3.Angle(transform.forward, Quaternion.Euler(0, angleForLakitu, 0) * Vector3.forward) / 180;
        if (Vector3.Dot(transform.forward, Quaternion.Euler(0, angleForLakitu, 0) * Vector3.right) > 0)
        {
            angleBetweenForwardAndDesired = -angleBetweenForwardAndDesired;
        }
        angleBetweenForwardAndDesired = Mathf.Clamp(angleBetweenForwardAndDesired, -1.0f, 1.0f);
        if (angleBetweenForwardAndDesired < 0)
        {
            if (!m_wrongWayToggle)
            {
                m_wrongWayObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(m_wrongWayObject.GetComponent<CanvasGroup>().alpha, 1, 8 * Time.deltaTime);
                if (m_wrongWayObject.GetComponent<CanvasGroup>().alpha > 0.99f)
                    m_wrongWayToggle = true;
            }
            else
            {
                m_wrongWayObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(m_wrongWayObject.GetComponent<CanvasGroup>().alpha, 0, 6 * Time.deltaTime);
                if (m_wrongWayObject.GetComponent<CanvasGroup>().alpha < 0.01f)
                    m_wrongWayToggle = false;
            }
        }
        else
        {
            if (m_wrongWayObject.GetComponent<CanvasGroup>().alpha > 0)
            {
                m_wrongWayObject.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(m_wrongWayObject.GetComponent<CanvasGroup>().alpha, 0, 6 * Time.deltaTime);
            }
        }
    }
    private void FixedUpdate()
    {
        Color newColor = m_speedLineRender.material.color;
        newColor.a = Mathf.Clamp((m_rigidBody.velocity.magnitude / 2 - 10) / m_maxSpeed, 0, 1);
        m_speedLineRender.material.color = newColor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // respawn character when hitting an obstacle
        if (collision.gameObject.CompareTag("Obstacle"))
        {
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
        if (!m_isRespawning)
            StartCoroutine(RespawnDelay());
    }
    // Taking an InputAction.CallbackContext allows us to add these functions to our actions with a '+='
    private void FireLeftHook(InputAction.CallbackContext obj)
    {
        m_leftGrapple.StartGrapple();
        //Checks if grapple was successful before playing sound
        if (m_leftGrapple.IsGrappling())
            StartCoroutine(FireHookSound());
    }
    private void StopLeftHook(InputAction.CallbackContext obj)
    {
        //Checks if grapple was successful before playing sound
        //if (m_leftGrapple.IsGrappling())
        m_leftGrapple.StopGrapple();
    }
    private void FireRightHook(InputAction.CallbackContext obj)
    {
        m_rightGrapple.StartGrapple();

        //Checks if grapple was successful before playing sound
        if (m_rightGrapple.IsGrappling())
            StartCoroutine(FireHookSound());

    }
    private void StopRightHook(InputAction.CallbackContext obj)
    {
        //Checks if grapple was successful before playing sound
        //if (m_rightGrapple.IsGrappling())
        m_rightGrapple.StopGrapple();
    }

    private void LateUpdate()
    {
        if (m_pauseMenu._ispaused || !m_inputActive)
        {
            return;
        }
        else
        {
            Vector2 input = m_playerInput.actions["Look"].ReadValue<Vector2>();
            m_cameraDolly.MoveCamera(new Vector2(-input.y, input.x));
            m_cameraDolly.MoveCamera(new Vector2(-input.y, input.x));
        }
    }

    //TODO these can be made to use a struct holding all information for grapplehooks
    public void UpdateGrappleHookFunction(float maxRopeDistance, float minRopeDistance, float hookSpeed, float grappleCooldown, float hookRigidness, float hookPullSlow,
        float massScale, LayerMask grappleableObjects, float initialPull, Transform leftHookOrigin, Transform rightHookOrigin, VFX hookHitFX)
    {
        m_leftGrapple.m_maxRopeDistance = m_rightGrapple.m_maxRopeDistance = maxRopeDistance;
        m_leftGrapple.m_minRopeDistance = m_rightGrapple.m_minRopeDistance = minRopeDistance;
        m_leftGrapple.m_hookSpeed = m_rightGrapple.m_hookSpeed = hookSpeed;
        m_leftGrapple.m_grappleCooldown = m_rightGrapple.m_grappleCooldown = grappleCooldown;
        m_leftGrapple.m_hookRigidness = m_rightGrapple.m_hookRigidness = hookRigidness;
        m_leftGrapple.m_hookPullSlow = m_rightGrapple.m_hookPullSlow = hookPullSlow;
        m_leftGrapple.m_massScale = m_rightGrapple.m_massScale = massScale;
        m_leftGrapple.m_grappleableObjects = m_rightGrapple.m_grappleableObjects = grappleableObjects;
        m_leftGrapple.m_initialPull = m_rightGrapple.m_initialPull = initialPull;
        m_leftGrapple.m_player = m_rightGrapple.m_player = transform;
        m_leftGrapple.m_camera = m_rightGrapple.m_camera = m_camera;
        m_rightGrapple.m_hookHitFX = m_leftGrapple.m_hookHitFX = hookHitFX;
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
        DeactivatePlayer();

        // wait for the amount of seconds at the beginning of the game
        yield return new WaitForSeconds(secondsToWait);

        m_inputActive = true;
        m_playerInput.ActivateInput();
        m_playerInput.actions["LeftFire"].performed += FireLeftHook;
        m_playerInput.actions["LeftFire"].canceled += StopLeftHook;
        m_playerInput.actions["RightFire"].performed += FireRightHook;
        m_playerInput.actions["RightFire"].canceled += StopRightHook;
    }
    public void DeactivatePlayer()
    {
        m_playerInput.DeactivateInput();
        m_playerInput.actions["LeftFire"].performed -= FireLeftHook;
        m_playerInput.actions["LeftFire"].canceled -= StopLeftHook;
        m_playerInput.actions["RightFire"].performed -= FireRightHook;
        m_playerInput.actions["RightFire"].canceled -= StopRightHook;
        m_inputActive = false;
    }
    private IEnumerator RespawnDelay()
    {
        SelectRandomSound(m_deathSounds).Spawn(transform);
        m_isRespawning = true;
        m_respawnAnimation.m_transition.Play("Crossfade_Start");
        yield return new WaitForSeconds(1);
        transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.SetPositionAndRotation(m_respawnLocation, m_respawnRotation);
        m_leftGrapple.StopGrapple();
        m_rightGrapple.StopGrapple();
        m_cameraDolly.SetLookRotation(m_respawnRotation);

        yield return new WaitForSeconds(1);
        m_respawnAnimation.m_transition.Play("Crossfade_End");
        m_isRespawning = false;
    }

    private VFX SelectRandomSound(List<VFX> VFXList)
    {
        return VFXList[(int)Random.Range(0, VFXList.Count)];
    }
    IEnumerator FireHookSound()
    {
        if (m_lastSoundPlayed >= 1.4f)
        {
            m_grappleLaunch.Spawn(transform);
            m_lastSoundPlayed = 0f;
            yield return new WaitForSeconds(0.1f);
            SelectRandomSound(m_gruntSounds).Spawn(transform);
            m_wind.Spawn(transform, (m_rigidBody.velocity.magnitude / 15f), Mathf.Clamp(m_rigidBody.velocity.magnitude / 50f, 0, 1));
        }
    }
}