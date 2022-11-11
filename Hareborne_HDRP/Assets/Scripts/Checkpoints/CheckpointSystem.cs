//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [Header("Checkpoint Prefabs")]
    public Checkpoint m_checkpointPrefab;
    public Checkpoint m_levelStartPrefab;
    public Checkpoint m_levelEndPrefab;
    [HideInInspector]
    public List<Checkpoint> m_checkpoints;
    [HideInInspector]
    public PlayerController m_player;
    [Header("Timer Prefab")]
    public Timer m_timer;
    public int m_countDown = 3;
    [HideInInspector]
    public int m_currentTriggeredCheckpoint = 0;
    private LevelLoader m_levelLoader;
    /// <summary>
    /// Set a reference to the player from within the scene
    /// </summary>

    // Custom start call to be used by the Game manager, ****REQUIRED TO RUN TO PLAY THE GAME****
    public void Initialise()
    {
        m_player = FindObjectOfType<PlayerController>();
        m_levelLoader = FindObjectOfType<LevelLoader>();
        if (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Checkpoint checkpointToAdd = transform.GetChild(i).GetComponent<Checkpoint>();
                m_checkpoints.Add(checkpointToAdd);
                checkpointToAdd.m_triggered = false;
                checkpointToAdd.GetComponent<Collider>().enabled = false;
            }
            //checkpoint game objects set to false except the first one
            transform.GetChild(0).GetComponent<Collider>().enabled = true;

            m_player.transform.position = transform.GetChild(0).transform.position;
            m_player.transform.rotation = transform.GetChild(0).transform.rotation;
        }
    }

    public void LevelFinished()
    {
        //loads end win screen
        m_levelLoader.LoadScene(1);
        FindObjectOfType<PlayerCrosshair>().gameObject.SetActive(false);
        StartCoroutine(MoveTimer());
        
    }

    /// <summary>
    /// These are Unity Editor tools to assist designers with editing the checkpoints inside of levels easily
    /// </summary>
    public void CreateStart()
    {
        if (transform.childCount == 0 || transform.GetChild(0).gameObject.name != "Map Start")
        {
            GameObject start = Instantiate(m_levelStartPrefab.gameObject, transform);
            start.transform.Translate(new Vector3(0, 0, -20));
            start.name = "Map Start";
            start.transform.SetSiblingIndex(0);
            return;
        }
        Debug.Log("The start already exists");
    }
    public void CreateEnd()
    {
        if (transform.childCount == 0 || transform.GetChild(transform.childCount - 1).gameObject.name != "Map End")
        {
            GameObject end = Instantiate(m_levelEndPrefab.gameObject, transform);
            end.transform.Translate(new Vector3(0, 0, 20));
            end.name = "Map End";
            end.transform.SetSiblingIndex(transform.childCount - 1);
            return;
        }
        Debug.Log("The end already exists");
    }
    public void CreateNewCheckpoint()
    {
        if (transform.childCount < 2)
        {
            if (transform.childCount == 0 || transform.GetChild(0).gameObject.name != "Map Start")
            {
                //runs check twice but this is in the editor it is fine
                CreateStart();
                return;
            }
            else if (transform.GetChild(transform.childCount - 1).gameObject.name != "Map End")
            {
                //runs check twice but this is in the editor it is fine
                CreateEnd();
                return;
            }
        }
        GameObject nextCheckpoint = Instantiate(m_checkpointPrefab.gameObject, transform);
        nextCheckpoint.name = "Checkpoint " + (transform.childCount - 2);
        nextCheckpoint.transform.SetSiblingIndex(transform.childCount - 2);
    }
    public void RemoveStart()
    {
        if (transform.childCount > 0 && transform.GetChild(0).gameObject.name == "Map Start")
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            return;
        }
        Debug.Log("Could not find a start to remove");
    }
    public void RemoveEnd()
    {
        if (transform.childCount > 0 && transform.GetChild(transform.childCount - 1).gameObject.name == "Map End")
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
            return;
        }
        Debug.Log("Could not find an end to remove");
    }
    public void RemoveCheckpointFromStart()
    {
        if (transform.childCount > 0)
        {
            if (transform.childCount == 2)
            {
                RemoveStart();
            }
            else if (transform.childCount == 1 && transform.GetChild(0).name == "Map End")
            {
                RemoveEnd();
            }
            else
            {
                DestroyImmediate(transform.GetChild(1).gameObject);
                for (int i = 1; i < transform.childCount - 1; i++)
                {
                    transform.GetChild(i).name = "Checkpoint " + i;
                }
            }
        }

    }
    public void RemoveCheckpointFromEnd()
    {
        if (transform.childCount > 0)
        {
            if (transform.childCount == 2)
            {
                RemoveEnd();
            }
            else if (transform.childCount == 1 && transform.GetChild(0).name == "Map Start")
            {
                RemoveStart();
            }
            else
            {
                DestroyImmediate(transform.GetChild(transform.childCount - 2).gameObject);
                for (int i = 1; i < transform.childCount - 1; i++)
                {
                    transform.GetChild(i).name = "Checkpoint " + i;
                }
            }
        }
    }
    public void ClearCheckpoints()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    private IEnumerator WaitSeconds(int secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
    }
    private IEnumerator MoveTimer()
    {
        yield return new WaitForSeconds(1);        
        m_timer.transform.parent.transform.Translate(new Vector2(200, -100));
        Cursor.lockState = CursorLockMode.None;
    }
}
