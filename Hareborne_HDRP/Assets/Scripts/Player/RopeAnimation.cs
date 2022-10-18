//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnimation : MonoBehaviour
{
    private LineRenderer m_lineRenderer;
    private Spring m_spring;
    public PlayerGrapple m_grappleHook;
    public int m_ropeQuality;
    public float m_damper, m_strength, m_velocity, m_waveCount, m_waveHeight;
    public AnimationCurve m_affectCurve;

    // Start is called before the first frame update
    private void Awake()
    {
        // Defines Line Renderer and spring
        m_lineRenderer = GetComponent<LineRenderer>();
        m_spring = new Spring();
        m_spring.SetTarget(0);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        DrawRope();
    }
    public void DrawRope()
    {
        if (!m_grappleHook.IsGrappling())
        {
            m_grappleHook.m_currentGrapplePosition = m_grappleHook.m_hookOrigin.position;
            m_spring.Reset();
            if (m_lineRenderer.positionCount > 0)
                m_lineRenderer.positionCount = 0;
            return;
        }
        //creates a line with as many segments as quality suggests
        if (m_lineRenderer.positionCount == 0)
        {
            m_spring.SetVelocity(m_velocity);
            m_lineRenderer.positionCount = m_ropeQuality + 1;
        }
        //sets spring values
        m_spring.SetDamper(m_damper);
        m_spring.SetStrength(m_strength);
        m_spring.Update(Time.deltaTime);

        //calculates the ropes upwards and right direction
        Vector3 hookUp = Quaternion.LookRotation(m_grappleHook.GetGrapplePoint() - m_grappleHook.m_hookOrigin.position.normalized) * Vector3.up;
        Vector3 hookRight = Quaternion.LookRotation(m_grappleHook.GetGrapplePoint() - m_grappleHook.m_hookOrigin.position.normalized) * Vector3.right;

        m_grappleHook.m_currentGrapplePosition = Vector3.Lerp(m_grappleHook.m_currentGrapplePosition, m_grappleHook.GetGrapplePoint(), Time.deltaTime * m_grappleHook.m_hookSpeed);

        //assign the position up and to the right of 
        for (int i = 0; i < m_ropeQuality + 1; i++)
        {
            float delta = i / (float)m_ropeQuality;
            Vector3 offset = hookUp * m_waveHeight * Mathf.Sin(delta * m_waveCount * Mathf.PI) * m_spring.Value * m_affectCurve.Evaluate(delta)
                + hookRight * m_waveHeight * Mathf.Cos(delta * m_waveCount * Mathf.PI) * m_spring.Value * m_affectCurve.Evaluate(delta);

            m_lineRenderer.SetPosition(i, Vector3.Lerp(m_grappleHook.m_hookOrigin.position, m_grappleHook.m_currentGrapplePosition, delta) + offset);
        }
    }
}
