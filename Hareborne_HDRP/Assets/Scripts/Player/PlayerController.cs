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
    private Vector3 m_respawnLocation;
    public InputAction m_playerControls;

    // Start is called before the first frame update
    void Start()    {
        
    }
    private void OnEnable()
    {
        m_playerControls.Enable();
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

        // Left grapple input controls
        if (Input.GetMouseButtonDown(0))
        {
            m_leftGrapple.StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_leftGrapple.StopGrapple();
        }

        // Right grapple input controls
        if (Input.GetMouseButtonDown(1))
        {
            m_rightGrapple.StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            m_rightGrapple.StopGrapple();
        }

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

}
