//Authored By Daniel Bainbridge, Kai Van Der Staay
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Checkpoint : MonoBehaviour
{
    float m_RecordedTime;
    //[HideInInspector]
    public bool m_triggered;
    private CheckpointSystem m_parentSystem;
    //[HideInInspector]
    public Timer m_timer;
    [Header("Particle Prefab")]
    private ParticleSystem[] m_checkpointReachedParticle;

    /// <summary>
    /// Get the parent system for references to the player in the scene
    /// </summary>
    private void Start()
    {
        m_parentSystem = GetComponentInParent<CheckpointSystem>();
        m_timer = m_parentSystem.m_timer;
        m_checkpointReachedParticle = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem pS in m_checkpointReachedParticle)
        {
            pS.Pause();
        }
    }
    /// <summary>
    /// If the player collides with the checkpoints trigger, set the players respawn location to the checkpoints transform,
    /// Take a record of the time
    /// set the next checkpoint to active
    /// set current checkpoiint to false
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //set respawn and collects current time
            m_parentSystem.m_player.SetRespawn(transform.position);
            m_RecordedTime = m_timer.GetCurrentTime();
            if(m_parentSystem.m_currentTriggeredCheckpoint != 0)
                m_timer.AddCheckpointTimeToUI();
            foreach (ParticleSystem pS in m_checkpointReachedParticle)
            {
                pS.Play();
            }

            int siblingIndex = transform.GetSiblingIndex();

            //change this to be the collision boxes/triggers

            gameObject.GetComponent<Collider>().enabled = false;
            m_triggered = true;

            //check if all checkpoints in parent Checkpoint system are hit
            //note for later, change to system not storing bools for each checkpoint, (triggered checkpoint count in checkpoint system, better for memory, faster, also harder to break)

            m_parentSystem.m_currentTriggeredCheckpoint += 1;
            if (m_parentSystem.m_currentTriggeredCheckpoint == m_parentSystem.m_checkpoints.Count)
            {
                //load win screen scene
                m_timer.StopTimer();
                m_parentSystem.LevelFinished();
                return;
            }
            m_parentSystem.m_checkpoints[siblingIndex + 1].gameObject.GetComponent<Collider>().enabled = true;

            //TODO Retain the time between Scene Transitions
        }
    }
}
