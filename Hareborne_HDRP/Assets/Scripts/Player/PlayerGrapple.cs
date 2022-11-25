//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer)), RequireComponent(typeof(RopeAnimation))]
public class PlayerGrapple : MonoBehaviour
{
    //make all hidden in inspector, edit inside of player

    private Vector3 m_grapplePoint;
    [HideInInspector]
    public Vector3 m_currentGrapplePosition;
    private SpringJoint m_springJoint;
    [HideInInspector]
    public LayerMask m_grappleableObjects;
    [HideInInspector]
    public Transform m_hookOrigin, m_player;
    [HideInInspector]
    public Transform m_camera;
    [HideInInspector]
    public float m_maxRopeDistance, m_minRopeDistance, m_hookSpeed, m_grappleCooldown, m_hookRigidness, m_hookPullSlow, m_massScale;
    [HideInInspector]
    public float m_initialPull;
    private float m_cooldownCounter;
    public VFX m_hookHitFX;

    void Start()
    {
        m_cooldownCounter = m_grappleCooldown;
    }
    void Update()
    {
        if (m_cooldownCounter < m_grappleCooldown && !IsGrappling())
            m_cooldownCounter += Time.deltaTime;
    }
    public void StartGrapple()
    {
        if (m_cooldownCounter >= m_grappleCooldown)
        {
            m_cooldownCounter = 0;
            //create RaycastHit
            RaycastHit hit;
            // if raycast hits something that you can grapple onto
            if (Physics.Raycast(m_camera.position, m_camera.forward, out hit, m_maxRopeDistance, m_grappleableObjects))
            {
                // Spring creation
                m_grapplePoint = hit.point;
                StartCoroutine(PlayFXTime());
                m_springJoint = m_player.gameObject.AddComponent<SpringJoint>();
                m_springJoint.autoConfigureConnectedAnchor = false;
                m_springJoint.connectedAnchor = m_grapplePoint;

                //set spring to pull you all the way towards the point and add a force onto that to get an initial tug on the grapple
                m_springJoint.maxDistance = 0.1f;
                m_springJoint.minDistance = 0f;
                m_player.GetComponent<Rigidbody>().AddForce((m_grapplePoint - m_player.position).normalized * m_initialPull, ForceMode.Acceleration);


                m_springJoint.spring = m_hookRigidness;
                m_springJoint.damper = m_hookPullSlow;
                m_springJoint.massScale = m_massScale;
                m_springJoint.enableCollision = true;
            }
        }
    }
    private IEnumerator PlayFXTime()
    {
        yield return new WaitForSeconds(m_hookSpeed * Time.deltaTime);
        GameObject neweffect = m_hookHitFX.Spawn(transform);
        neweffect.transform.position = m_grapplePoint;
    }
    public void StopGrapple()
    {
        Destroy(m_springJoint);
    }

    public bool IsGrappling()
    {
        return m_springJoint != null;
    }
    public Vector3 GetGrapplePoint()
    {
        return m_grapplePoint;
    }
}
