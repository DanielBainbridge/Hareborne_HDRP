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
    private bool isSetup = false;
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
        isSetup = false;
        Timer[] timers = FindObjectsOfType<Timer>();
        for (int i = 0; i < timers.Length; i++)
        {
            if (timers[i].gameObject != gameObject)
                Destroy(timers[i].transform.parent.transform.parent.gameObject);
        }

        m_containerImage = GetComponentInParent<Image>();
        m_checkpointSystem = FindObjectOfType<CheckpointSystem>();
        m_countDown = m_checkpointSystem.m_countDown;
        m_timerText = this.GetComponent<TextMeshProUGUI>();
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
            checkpointNumber.rectTransform.position = lineToAdd.transform.position + new Vector3(-m_containerImage.rectTransform.rect.width / 30f, 0, 0);
            if (i + 1 != m_checkpointSystem.m_checkpoints.Count - 1)
            {
                checkpointNumber.text = "Checkpoint " + (i + 1);
            }
            else
                checkpointNumber.text = "Finish";
        }
        isSetup = true;

    }

    private void Awake()
    {
        
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
        if (isSetup == false)
        StartTimer();
        else
        {
            m_startTime = Time.time;
            m_timerActive = true;
        }
        
    }
    public void StopTimer()
    {
        m_timerActive = false;
    }
    public void AddCheckpointTimeToUI()
    {
        Text checkpointTime = Instantiate(m_textPrefab, transform.parent);
        checkpointTime.transform.position = transform.position + new Vector3(m_containerImage.rectTransform.rect.width / 30f, 0, 0);
        float currentTime = GetCurrentTime();
        string minutes = ((int)currentTime / 60).ToString();
        string seconds = (currentTime % 60).ToString("f2");
        checkpointTime.text = minutes + " : " + seconds;
        transform.Translate(new Vector3(0, -m_lineSpace, 0));
    }
    private IEnumerator WaitSeconds(int secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        StartTimer();
    }

}
