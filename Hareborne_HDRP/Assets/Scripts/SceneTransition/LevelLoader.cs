//Authoured By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public GameObject thing;
    public Animator m_transition;
    public float m_transitionTime = 1f;
    public bool _destory = false;

    
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadLevel(sceneIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        m_transition.SetTrigger("Start");
        yield return new WaitForSeconds(m_transitionTime);
        SceneManager.LoadScene(levelIndex);

    }

    public void LoadOffPaused(int sceneIndex)
    {
        if (sceneIndex == 0)
        {
            Destroy(thing);
            _destory = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneIndex);
        }
        
    }
}
