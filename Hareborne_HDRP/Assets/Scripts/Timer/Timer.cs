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
    private CheckpointSystem m_checkpointSystem;
    [Header("Line Between Time Splits")]
    public Image m_linePrefab;
    public float m_lineSpace;
    [Header("Text Prefab To Add for Splits")]
    public Text m_textPrefab;
    private Image m_containerImage;


    // Start is called before the first frame update
    void Start()
    {
        m_containerImage = GetComponentInParent<Image>();
        m_checkpointSystem = FindObjectOfType<CheckpointSystem>();
        m_countDown = m_checkpointSystem.m_countDown;
        m_timerText = this.GetComponent<Text>();
        WaitSeconds(m_countDown);
        StartTimer();
        m_timerActive = true;

        for (int i = 0; i < m_checkpointSystem.m_checkpoints.Count - 1; i++)
        {
            float yOffset = m_lineSpace * i -60;
            Vector3 positionOffset = new Vector3(-3, -yOffset, 0);
            Image lineToAdd = Instantiate(m_linePrefab, transform.parent);
            lineToAdd.rectTransform.Translate(positionOffset);
            if(i == 0)
            {
                transform.position = lineToAdd.transform.position;
                transform.Translate(new Vector3(0, 4, 0));
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (m_timerActive)
        {
            float currentTime = GetCurrentTime();
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
    public void AddCheckpointTimeToUI()
    {
        Text checkpointTime = Instantiate(m_textPrefab, transform.parent);
        checkpointTime.transform.position = transform.position;
        float currentTime = GetCurrentTime();
        string minutes = ((int)currentTime / 60).ToString();
        string seconds = (currentTime % 60).ToString("f2");
        checkpointTime.text = minutes + " : " + seconds;
        transform.Translate(new Vector3(0, -m_lineSpace, 0));
    }
    private IEnumerator WaitSeconds(int secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
    }

}
