//Authored By Daniel Bainbridge, Kai Van Der Staay
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    float m_RecordedTime;
    [HideInInspector]
    public bool m_triggered = false;
    private CheckpointSystem m_parentSystem;
    [HideInInspector]
    public Timer m_timer;

    /// <summary>
    /// Get the parent system for references to the player in the scene
    /// </summary>
    private void Start()
    {
        m_parentSystem = GetComponentInParent<CheckpointSystem>();
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
            m_parentSystem.m_player.SetRespawn(transform.position);
            m_RecordedTime = m_timer.GetCurrentTime();

            int siblingIndex = transform.GetSiblingIndex();
            m_parentSystem.m_checkpoints[siblingIndex + 1].gameObject.SetActive(true);
            gameObject.SetActive(false);

            //check if all checkpoints in parent Checkpoint system are hit
            //note for later, change to system not storing bools for each checkpoint, (triggered checkpoint count in checkpoint system, better for memory, faster, also harder to break)
            foreach(Checkpoint c in m_parentSystem.m_checkpoints)
            {
                if(!c.m_triggered)
                {
                    break;
                }
                //do win stuffs
                m_timer.StopTimer();
            }
        }
    }
}
