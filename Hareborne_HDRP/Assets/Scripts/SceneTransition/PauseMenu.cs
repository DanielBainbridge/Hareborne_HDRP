using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public InputActionMap _playerInput;
    public InputAction _menu;
    public GameObject _crosshair;
    public GameObject _countDown;

    [SerializeField] GameObject _pauseUI;
    public bool _ispaused = false;

    public Canvas[] _canvases;

    public GameObject optionsMenu, controlsMenu;
    public bool openControlsMenu, openOptionsMenu;
    

    // Start is called before the first frame update
    void start()
    {
        _canvases = FindObjectsOfType<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            _crosshair.SetActive(false);
            //_countDown.SetActive(false);
            ActivateMenu();
        }

        else
        {
            _crosshair.SetActive(true);
            //_countDown.SetActive(true);
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
        _ispaused = false;
    }

    private void ActivateMenu()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        _pauseUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Debug.Log(Cursor.lockState);
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
}
