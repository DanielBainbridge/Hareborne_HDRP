//Authored By Daniel Bainbridge, Kai Van Der Staay
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public GameObject m_checkpointPrefab;
    //[HideInInspector]
    public List<Checkpoint> m_checkpoints;
    [HideInInspector]
    public PlayerController m_player;
    public Timer m_timer;

    /// <summary>
    /// Set a reference to the player from within the scene
    /// </summary>
    void Start()
    {
        m_player = GetComponentInParent<PlayerController>();
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Checkpoint checkpointToAdd = transform.GetChild(i).GetComponent<Checkpoint>();
                m_checkpoints.Add(checkpointToAdd);
                checkpointToAdd.m_triggered = false;
            }
            //checkpoint game objects set to false except the first one
            transform.GetChild(0).GetComponent<Checkpoint>().m_triggered = true;
            m_player.transform.position = transform.GetChild(0).transform.position;
            m_player.transform.rotation = transform.GetChild(0).transform.rotation;
        }
    }

    public void CreateStartEnd()
    {
        GameObject start = Instantiate(m_checkpointPrefab, transform);
        start.name = "Map Start";

        GameObject end = Instantiate(m_checkpointPrefab, transform);
        end.name = "Map End";
    }
    public void CreateNewCheckpoint()
    {
        GameObject nextCheckpoint = Instantiate(m_checkpointPrefab, transform);
        nextCheckpoint.name = "Checkpoint " + (transform.childCount - 2);
        nextCheckpoint.transform.SetSiblingIndex(transform.childCount - 2);
    }
    public void RemoveCheckpointFromStart()
    {
        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0));
            transform.GetChild(0).gameObject.name = "Map Start";
        }
    }
    public void RemoveCheckpointFromEnd()
    {
        if (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            transform.GetChild(transform.childCount - 1).gameObject.name = "Map End";
        }
    }
    public void ClearCheckpoints()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
    }
}
