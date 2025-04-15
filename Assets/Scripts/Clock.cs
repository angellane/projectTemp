using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public TextMeshProUGUI clockText;
    public float secondsPerInGameMinute = 1f;

    private float timer = 0f;
    private int hours = 9;
    private int minutes = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= secondsPerInGameMinute)
        {
            timer = 0f;
            minutes++;

            if (minutes >= 60)
            {
                minutes = 0;
                hours++;

                if (hours >= 24)
                {
                    hours = 0;
                    FindAnyObjectByType<DayManager>()?.NextDay();
                }
            }

            clockText.text = $"{hours:00}:{minutes:00}";
        }
    }
}
