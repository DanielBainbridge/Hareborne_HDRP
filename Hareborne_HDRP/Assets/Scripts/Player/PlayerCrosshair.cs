using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCrosshair : MonoBehaviour
{
    private PlayerController m_player;
    private PlayerGrapple m_grappleInfo;
    private Image m_crosshair;
    public Color m_grapplePossibleColour, grappleNotPossibleColour;
    private void Start()
    {
        m_player = FindObjectOfType<PlayerController>();
        m_grappleInfo = m_player.m_leftGrapple;
        m_crosshair = GetComponent<Image>();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        //gets information from raycast
        RaycastHit hit;
        //checks if the raycast actually hits, Raycast(Casts ray from camera, forward from camera, OUTPUT, Length of the ray, the layer I am checking for collision on)
        if (Physics.Raycast(m_grappleInfo.m_camera.position, m_grappleInfo.m_camera.forward, out hit, m_grappleInfo.m_maxRopeDistance, m_grappleInfo.m_grappleableObjects))
        {
            //change crosshair colour
            m_crosshair.color = m_grapplePossibleColour;
        }
        else m_crosshair.color = grappleNotPossibleColour;
    }
}
