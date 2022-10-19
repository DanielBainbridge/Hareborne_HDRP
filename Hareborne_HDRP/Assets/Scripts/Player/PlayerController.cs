//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    // Start is called before the first frame update
    void Start() 
    {
        //set reference to camera
        m_cameraDolly = m_camera.GetComponent<CameraDolly>();

        //set values from to apply to grapples here... do it...
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
        m_cameraMovement.performed += 
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
        //needs a vector 2
        m_cameraDolly.MoveCamera();
    }
}
