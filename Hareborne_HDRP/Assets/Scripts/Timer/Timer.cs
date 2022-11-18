//Authored By Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI m_timerText;
    //private Text m_timerText;
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
    //total time float
    [Header("Total Time Taken")]
    public float m_totalTime;

    public void Initialise()
    {
        Timer[] timers = FindObjectsOfType<Timer>();
        for (int i = 0; i < timers.Length; i++)
        {
            if (timers[i].gameObject != gameObject)
                Destroy(timers[i].transform.parent.transform.parent.gameObject);
        }

        m_checkpointSystem = FindObjectOfType<CheckpointSystem>();
        m_containerImage = GetComponentInParent<Image>();
        m_countDown = m_checkpointSystem.m_countDown;
        m_timerText = this.GetComponent<TextMeshProUGUI>();
        m_lineSpace = m_containerImage.rectTransform.rect.height / m_checkpointSystem.m_checkpoints.Count;
        StartCoroutine(WaitSeconds(m_countDown));


        for (int i = 0; i < m_checkpointSystem.m_checkpoints.Count - 1; i++)
        {
            float yOffset = m_lineSpace * i - 60;
            Vector3 positionOffset = new Vector3(-3, -yOffset, 0);
            Image lineToAdd = Instantiate(m_linePrefab, transform.parent);
            lineToAdd.rectTransform.Translate(positionOffset);
            if (i == 0)
            {
                transform.position = lineToAdd.transform.position;
            }
            Text checkpointNumber = Instantiate(m_textPrefab, transform.parent);
            checkpointNumber.alignment = TextAnchor.MiddleLeft;
            checkpointNumber.rectTransform.position = lineToAdd.transform.position + new Vector3(-m_containerImage.rectTransform.rect.width / 8.0f, 0, 0);
            if (i + 1 != m_checkpointSystem.m_checkpoints.Count - 1)
            {
                checkpointNumber.text = "Checkpoint " + (i + 1);
            }
            else
                checkpointNumber.text = "Finish";
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
        m_timerActive = true;

    }
    public void StopTimer()
    {
        m_timerActive = false;
    }
    public void AddCheckpointTimeToUI()
    {
        Text checkpointTime = Instantiate(m_textPrefab, transform.parent);
        checkpointTime.transform.position = transform.position + new Vector3(m_containerImage.rectTransform.rect.width / 8.0f, 0, 0);
        float currentTime = GetCurrentTime();
        string minutes = ((int)currentTime / 60).ToString();
        string seconds = (currentTime % 60).ToString("f2");
        checkpointTime.text = minutes + " : " + seconds;
        if (m_checkpointSystem.m_currentTriggeredCheckpoint != m_checkpointSystem.m_checkpoints.Count - 1)
            transform.Translate(new Vector3(0, -m_lineSpace, 0));
        else
            transform.gameObject.SetActive(false);
    }
    private IEnumerator WaitSeconds(int secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        StartTimer();
    }

}
