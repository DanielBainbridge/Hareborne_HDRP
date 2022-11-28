using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private PlayerInput m_playerInput;
    public InputAction _menu;
    public GameObject _crosshair;
    public GameObject m_wrongWay;

    [SerializeField] GameObject _pauseUI; 
    public bool _ispaused = false;

    public GameObject optionsMenu, controlsMenu;
    public bool openControlsMenu;
    

    // Start is called before the first frame update
    void Start()
    {
        m_playerInput = FindObjectOfType<MenuUI>().m_input;
    }

    void OnEnable()
    {
        if (_menu != null)
        _menu.Enable();

        _menu.performed += pause;
    }
    void OnDisable()
    {
        _menu.Disable();
    }

    private void pause(InputAction.CallbackContext ctx)
    {
        _ispaused = !_ispaused;

        if (_ispaused == true)
        {
            ActivateMenu();
        }

        else
        {
            DeActivateMenu();
        }
    }

    public void DeActivateMenu()
    {
        _crosshair.SetActive(true);
        //_countDown.SetActive(true);
        Time.timeScale = 1;
        AudioListener.pause = false;
        _pauseUI.SetActive(false);
        optionsMenu.SetActive(false);
        _ispaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_playerInput.SwitchCurrentActionMap("PlayerControls");
    }

    private void ActivateMenu()
    {
        _crosshair.SetActive(false);
        m_wrongWay.SetActive(false);
        Time.timeScale = 0;
        AudioListener.pause = true;
        _pauseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        m_playerInput.SwitchCurrentActionMap("UIControls");
    }

    public void OpenMenu()
    {
        Debug.Log("Trying to open.");
        if (openControlsMenu == false)
        {
            controlsMenu.SetActive(true);
            openControlsMenu = true;
            Debug.Log("Successfully opened.");
        }
        
        //if (openOptionsMenu == false)
        //optionsMenu.SetActive(true);
         else if (openControlsMenu == true)
        {
            controlsMenu.SetActive(false);
            openControlsMenu = false;
            Debug.Log("Successfully Closed.");
        }
        
        //if (openOptionsMenu == true)
        //optionsMenu.SetActive(false);
        else
        Debug.Log("Menu opening error. Check inspector.");
        Debug.Log("failed to open.");
    }

    public void ResetLevel()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
    public void RespawnPlayer()
    {
        FindObjectOfType<PlayerController>().RespawnCharacter();
        DeActivateMenu();
    }
}
