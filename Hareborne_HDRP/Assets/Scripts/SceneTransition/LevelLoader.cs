using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator m_transition;
    public float m_transitionTime = 1f;
    
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
}
