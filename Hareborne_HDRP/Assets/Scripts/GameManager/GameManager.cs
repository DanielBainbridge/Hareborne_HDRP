using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // What do I need access to?

    // I need a list of checkpoints, I need to reference the player, audio reference

    // Checkpoint related variables
    private List<CheckpointSystem> m_checkpointSystems = new List<CheckpointSystem>();
    private CheckpointSystem m_usedCheckpointSystem;
    private Timer m_sceneTimer;

    // Player related variables
    private PlayerController m_playerInScene;
    private CameraDolly m_mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // Find and assign variables
        var checkpoints = FindObjectsOfType<CheckpointSystem>();

        for (int i = 0; i < checkpoints.Length; i++)
        {
            m_checkpointSystems.Add(checkpoints[i]);
        }
        m_playerInScene = FindObjectOfType<PlayerController>();
        m_mainCamera = FindObjectOfType<CameraDolly>();
        m_sceneTimer = FindObjectOfType<Timer>();


        //use selected level
        m_usedCheckpointSystem = m_checkpointSystems[PlayerPrefs.GetInt("CurrentLevel")];
        m_usedCheckpointSystem.gameObject.SetActive(true);


        // Checkpoint System Start
        m_usedCheckpointSystem.Initialise();
        // Player Start
        m_playerInScene.Initialise();
        // Camera Start
        m_mainCamera.Initialise();
        // Timer Start
        m_sceneTimer.Initialise();
    }

    private static uint BitRotate(uint x)
    {
        int bits = 16;
        return (x << bits) | (x >> bits);
    }
    private int GetRandomLevel()
    {
        uint number = (uint)Random.Range(0, 520);

        int x = Random.Range(0, 300);
        int y = Random.Range(0, 300);

        for (uint i = 0; i < 16; i++)
        {
            number = number * 541 + (uint)x;
            number = BitRotate(number);
            number = number * 809 + (uint)y;
            number = BitRotate(number);
            number = number * 673 + (uint)i;
            number = BitRotate(number);
        }
        return (int)(number % m_checkpointSystems.Count);
    }
}
