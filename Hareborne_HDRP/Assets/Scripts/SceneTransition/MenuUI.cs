//Authoured By Daniel Bainbridge and Fynn Burgess
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MenuUI : MonoBehaviour
{
    public LevelLoader m_levelLoader;

    [SerializeField] int m_deselectedFontSize = 22;

    public void TextGreyColour([SerializeField] TMP_Text _btn_Text)
    {
        _btn_Text.color = Color.grey;
        _btn_Text.fontSize = m_deselectedFontSize;
    }

    public void TextWhiteColour([SerializeField] TMP_Text _btn_Text)
    {
        _btn_Text.color = Color.white;
        _btn_Text.fontSize = m_deselectedFontSize + 4;
    }
    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        Time.timeScale = 1;
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length != 0)
        {
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i].gameObject != gameObject)
                {
                    eventSystems[i].enabled = false;
                }
            }
        }
    }

    public void SelectLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelNumber);
    }

    public void LoadLevel(int sceneIndex)
    {
        Destroy(FindObjectOfType<PauseMenu>());
        m_levelLoader.LoadScene(sceneIndex);
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        LoadLevel(2);
        Time.timeScale = 1;
    }
    public void ExitGame()
    {
        Debug.Log("Exits Application");
        Application.Quit();
    }
}
