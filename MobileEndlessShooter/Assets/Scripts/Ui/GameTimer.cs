using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI  timeText;
    private float time = 0f;
    private bool isTimerRunninng;
    [SerializeField] private PlayerHealth playerHealth;
    public float CurrentTime
    {
        get { return time; }
        set 
        { 
            time = value; 
            DisplayTimer(); // Instantly update the text display when loaded
        }
    }

    void Start()
    {
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if(playerHealth.IsDead) return;
        time += Time.deltaTime;
        DisplayTimer();
        
    }

    private void DisplayTimer()
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int fraction = Mathf.FloorToInt(time * 100) % 100;

        if (timeText != null)
        {
            timeText.text = $"Survived {minutes:00}:{seconds:00}:{fraction:00}";
        }
    }
}
