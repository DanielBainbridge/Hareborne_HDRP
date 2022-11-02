using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    LevelLoader m_levelLoader;
    // Start is called before the first frame update
    void Start()
    {
        m_levelLoader = FindObjectOfType<LevelLoader>();
    }

    public void LoadLevel(int sceneIndex)
    {
        m_levelLoader.LoadScene(sceneIndex);
    }
    public void ExitGame()
    {
        Debug.Log("Exits Application");
        Application.Quit();
    }
}
