using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementChecker : MonoBehaviour
{
    private List<Achievement> tripAchievements = new List<Achievement>();
    private List<Achievement> driftAchievements = new List<Achievement>();
    private List<Achievement> highSpeedAchievements = new List<Achievement>();

    private Achievements achievements;

    private void Start() => GetAchievements();

    private void GetAchievements()
    {
        achievements = new Achievements().Deserialize();

        tripAchievements = achievements.achievements.FindAll(c => c.type == 0);
        driftAchievements = achievements.achievements.FindAll(c => c.type == 1);
        highSpeedAchievements = achievements.achievements.FindAll(c => c.type == 2);

        if (checkCor == null) checkCor = StartCoroutine(CheckCoroutine());
    }

    private Coroutine checkCor;
    private IEnumerator CheckCoroutine()
    {
        while (true)
        {
            Check();
            yield return new WaitForSeconds(1f);
        }
    }

    private void Check()
    {
        if (!tripCompleted) CheckTrip();
        if (!driftCompleted) CheckDrift();
        if (!highSpeedCompleted) CheckHighSpeed();
        if (tripCompleted && driftCompleted && highSpeedCompleted && checkCor != null) StopCoroutine(checkCor);
    }

    private bool tripCompleted;
    private void CheckTrip()
    {
        if (!tripAchievements.Any(c => !c.completed))
        {
            tripCompleted = true;
            return;
        }

        for (int i = 0; i < tripAchievements.Count; i++)
        {
            Achievement a = tripAchievements[i];
            if (!a.completed && GameManager.Instance.dataManager.user.gameData.statistics.trip >= a.target)
            {
                a.completed = true;
                achievements.achievements.Find(c => c.id == a.id).completed = true;
                achievements.Save();
                GameManager.Instance?.sdkManager?.gpgManager?.ReportAchiecement(a.id);
            }
        }
    }

    private bool driftCompleted;
    private void CheckDrift()
    {
        if (!driftAchievements.Any(c => !c.completed))
        {
            driftCompleted = true;
            return;
        }

        for (int i = 0; i < driftAchievements.Count; i++)
        {
            Achievement a = driftAchievements[i];
            if (!a.completed && GameManager.Instance.dataManager.user.gameData.statistics.drift >= a.target)
            {
                a.completed = true;
                achievements.achievements.Find(c => c.id == a.id).completed = true;
                achievements.Save();
                GameManager.Instance?.sdkManager?.gpgManager?.ReportAchiecement(a.id);
            }
        }
    }

    private bool highSpeedCompleted;
    private void CheckHighSpeed()
    {
        if (!highSpeedAchievements.Any(c => !c.completed))
        {
            highSpeedCompleted = true;
            return;
        }

        for (int i = 0; i < highSpeedAchievements.Count; i++)
        {
            Achievement a = driftAchievements[i];
            if (!a.completed && GameManager.Instance.dataManager.user.gameData.statistics.highSpeedTrip >= a.target)
            {
                a.completed = true;
                achievements.achievements.Find(c => c.id == a.id).completed = true;
                achievements.Save();
                GameManager.Instance?.sdkManager?.gpgManager?.ReportAchiecement(a.id);
            }
        }
    }

    private void OnDestroy()
    {
        if (checkCor != null) StopCoroutine(checkCor);
    }
}