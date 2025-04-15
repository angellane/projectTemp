using TMPro;
using UnityEngine;
using System.Collections;

public class DayManager : MonoBehaviour
{
    public GameObject dayPanel;
    public TextMeshProUGUI dayText;
    private int currentDay = 1;

    public void NextDay()
    {
        currentDay++;
        StartCoroutine(ShowNextDayScreen());
    }

    private IEnumerator ShowNextDayScreen()
    {
        dayPanel.SetActive(true);
        dayText.text = $"Day {currentDay}";
        yield return new WaitForSeconds(2f);
        dayPanel.SetActive(false);
    }

    public int GetCurrentDay()
    {
        return currentDay;
    }
}
