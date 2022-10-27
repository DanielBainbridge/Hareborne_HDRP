//Authored By Daniel Bainbridge, Kai Van Der Staay
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Text m_timerText;
    private float m_startTime;
    private bool m_timerActive;
    private int m_countDown;

    // Start is called before the first frame update
    void Start()
    {
        m_countDown = FindObjectOfType<CheckpointSystem>().m_countDown;
        m_timerText = this.GetComponent<Text>();
        WaitSeconds(m_countDown);
        StartTimer();
        m_timerActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timerActive)
        {
            float currentTime = Time.time - m_startTime;
            string minutes = ((int)currentTime / 60).ToString();
            string seconds = (currentTime % 60).ToString("f2");

            m_timerText.text = minutes + " : " + seconds;
        }
    }
    // get current time
    public float GetCurrentTime()
    {
        return Time.time - m_startTime;
    }
    public void StartTimer() 
    {
        m_startTime = Time.time;
    }
    public void StopTimer()
    {
        m_timerActive = false;
    }
    private IEnumerator WaitSeconds(int secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
    }
}
