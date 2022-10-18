//Author Affax(youtube) (naming conventions changed)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring
{
    private float m_strength, m_damper, m_target, m_velocity, m_value;

    public void Update(float deltaTime)
    {
        var direction = m_target - m_value >= 0 ? 1f : -1f;
        var force = Mathf.Abs(m_target - m_value) * m_strength;
        m_velocity += (force * direction - m_velocity * m_damper) * deltaTime;
        m_value += m_velocity * deltaTime;
    }

    public void Reset()
    {
        m_velocity = 0f;
        m_value = 0f;
    }

    public void SetValue(float value)
    {
        this.m_value = value;
    }

    public void SetTarget(float target)
    {
        this.m_target = target;
    }

    public void SetDamper(float damper)
    {
        this.m_damper = damper;
    }

    public void SetStrength(float strength)
    {
        this.m_strength = strength;
    }

    public void SetVelocity(float velocity)
    {
        this.m_velocity = velocity;
    }

    public float Value => m_value;
}
