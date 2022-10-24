using UnityEngine;
using UnityEngine.UI;

public class UserStatisticsPanel : MonoBehaviour
{
    public RectTransform experienceTextRect;
    public RectTransform totalTripTextRect;
    public RectTransform driftTextRect;
    public RectTransform bestDriftTextRect;

    private Text experienceText;
    private Text totalTripText;
    private Text driftText;
    private Text bestDriftText;

    public UserStatisticsPanel Construct()
    {
        experienceText = experienceTextRect.GetComponent<Text>();
        totalTripText = totalTripTextRect.GetComponent<Text>();
        driftText = driftTextRect.GetComponent<Text>();
        bestDriftText = bestDriftTextRect.GetComponent<Text>();

        experienceText.text = GameManager.Instance.dataManager.user.gameData.statistics.experience.ToString() + " POINT";
        totalTripText.text = GameManager.Instance.dataManager.user.gameData.statistics.trip.ToString("F1") + " KM";
        driftText.text = GameManager.Instance.dataManager.user.gameData.statistics.drift.ToString() + " POINT";
        bestDriftText.text = GameManager.Instance.dataManager.user.gameData.statistics.bestDrift.ToString() + " POINT";

        return this;
    }
}