//Authoured By Daniel Bainbridge and Fynn Burgess
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class MenuUI : MonoBehaviour
{
    public LevelLoader m_levelLoader;
    private EventSystem m_eventSystem;
    [HideInInspector]
    public PlayerInput m_input;
    private string m_currentControlScheme;
    private GameObject m_currentMenu;

    [Header("Menus")]
    [SerializeField] GameObject m_mainMenu;
    [SerializeField] GameObject m_optionsMenu;
    [SerializeField] GameObject m_controlsMenu;
    [SerializeField] GameObject m_creditsMenu;
    [SerializeField] GameObject m_mainMenuDefaultButton;
    [SerializeField] GameObject m_optionsDefaultButton;
    [SerializeField] GameObject m_controlsDefaultButton;
    [SerializeField] GameObject m_creditsDefaultButton;
    void Awake()
    {
        Time.timeScale = 1;
        m_input = transform.GetComponent<PlayerInput>();
        m_currentControlScheme = m_input.currentControlScheme;
        m_currentMenu = m_mainMenu;
        EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
        if (eventSystems.Length != 0)
        {
            for (int i = 0; i < eventSystems.Length; i++)
            {
                if (eventSystems[i].gameObject != gameObject)
                {
                    eventSystems[i].enabled = false;
                }
                else
                {
                    m_eventSystem = eventSystems[i];
                }
            }
        }
    }
    private void Update()
    {
        OnControlsChanged();
    }


    public void TextGreyColour([SerializeField] TMP_Text _btn_Text)
    {
        _btn_Text.color = Color.grey;
    }

    public void TextWhiteColour([SerializeField] TMP_Text _btn_Text)
    {
        _btn_Text.color = Color.white;
    }

    public void OpenControlsMenu()
    {
        StartCoroutine(OpenControlsMenuCoroutine());
    }
    public void OpenCreditsMenu()
    {
        StartCoroutine(OpenCreditsMenuCoroutine());
    }
    public void OpenOptionsMenu()
    {
        StartCoroutine(OpenOptionsMenuCoroutine());
    }
    public void BackToMainMenu()
    {
        StartCoroutine(BackToMainMenuCoroutine());
    }

    public IEnumerator OpenControlsMenuCoroutine()
    {
        m_controlsMenu.SetActive(true);
        m_currentMenu.SetActive(false);
        m_currentMenu = m_controlsMenu;
        yield return null;
        if (m_currentControlScheme == "Gamepad")
            GetComponent<EventSystem>().SetSelectedGameObject(m_controlsDefaultButton);
    }
    public IEnumerator OpenCreditsMenuCoroutine()
    {
        m_creditsMenu.SetActive(true);
        m_currentMenu.SetActive(false);
        m_currentMenu = m_creditsMenu;
        yield return null;
        if (m_currentControlScheme == "Gamepad")
            GetComponent<EventSystem>().SetSelectedGameObject(m_creditsDefaultButton);
    }
    public IEnumerator OpenOptionsMenuCoroutine()
    {
        m_optionsMenu.SetActive(true);
        m_currentMenu.SetActive(false);
        m_currentMenu = m_optionsMenu;
        yield return null;
        if (m_currentControlScheme == "Gamepad")
            GetComponent<EventSystem>().SetSelectedGameObject(m_optionsDefaultButton);
    }
    public IEnumerator BackToMainMenuCoroutine()
    {
        m_mainMenu.SetActive(true);
        m_currentMenu.SetActive(false);
        m_currentMenu = m_mainMenu;
        yield return null;
        if (m_currentControlScheme == "Gamepad")
            GetComponent<EventSystem>().SetSelectedGameObject(m_mainMenuDefaultButton);
    }

    public void SelectLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("CurrentLevel", levelNumber);
    }

    public void LoadLevel(int sceneIndex)
    {
        Destroy(this.gameObject);
        m_levelLoader.LoadScene(sceneIndex);
        Time.timeScale = 1;
    }
    public void LoadMainMenu()
    {
        LoadLevel(0);
    }
    public void RestartLevel()
    {
        LoadLevel(2);
    }
    public void ExitGame()
    {
        Debug.Log("Exits Application");
        Application.Quit();
    }
    public void OnControlsChanged()
    {
        if (m_input)
        {
            if (m_input.currentControlScheme != m_currentControlScheme)
            {
                m_currentControlScheme = m_input.currentControlScheme;
                if (m_currentControlScheme == "Gamepad")
                {
                    if (m_currentMenu == m_mainMenu)
                        m_eventSystem.SetSelectedGameObject(m_mainMenuDefaultButton);
                    else if (m_currentMenu == m_optionsMenu)
                        m_eventSystem.SetSelectedGameObject(m_optionsDefaultButton);
                    else if (m_currentMenu == m_controlsMenu)
                        m_eventSystem.SetSelectedGameObject(m_controlsDefaultButton);
                    else if (m_currentMenu == m_creditsMenu)
                        m_eventSystem.SetSelectedGameObject(m_creditsDefaultButton);
                }
                else
                {
                    m_eventSystem.SetSelectedGameObject(null);
                }
            }
        }
    }
}
