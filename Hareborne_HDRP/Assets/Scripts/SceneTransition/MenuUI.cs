//Authoured By Daniel Bainbridge and Fynn Burgess
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuUI : MonoBehaviour
{
    LevelLoader m_levelLoader;

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
