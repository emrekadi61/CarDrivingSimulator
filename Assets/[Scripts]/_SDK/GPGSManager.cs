using UnityEngine;
using UnityEngine.Events;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSManager : ManagerBase
{
    private string userId;

    public bool isAuthenticated { get { return PlayGamesPlatform.Instance.IsAuthenticated(); } }

    [HideInInspector] public string username;

    private void Awake() => base.onConstructed.AddListener(this.OnConstructed);

    private void OnConstructed()
    {
        base.onConstructed.RemoveListener(this.OnConstructed);
        SignIn();
    }

    public void SignIn(UnityAction<SignInStatus> onComplete = null)
    {
        PlayGamesPlatform.Activate();
        PlayGamesPlatform.Instance.Authenticate(status =>
        {
            userId = PlayGamesPlatform.Instance.GetUserId();
            onComplete?.Invoke(status);
            username = PlayGamesPlatform.Instance.localUser.userName;
            if (GameManager.Instance.dataManager.user.username != username)
            {
                GameManager.Instance.dataManager.user.id = userId;
                GameManager.Instance.dataManager.user.username = username;
                GameManager.Instance.dataManager.SaveUser();
            }
        });
    }

    public void ReportProgress(string achievementID, float progress)
    {
        if (!isAuthenticated) return;
        PlayGamesPlatform.Instance.ReportProgress(achievementID, (long)progress, null);
    }

    public void ReportAchiecement(string achievementName)
    {
        if (!isAuthenticated) return;
        PlayGamesPlatform.Instance.UnlockAchievement(achievementName, null);
    }

    public void ReportScore(string scoreboardName, float score)
    {
        if (!isAuthenticated) return;
        PlayGamesPlatform.Instance.ReportScore((long)score, scoreboardName, null);
    }

    public void ShowLeaderboard(string leaderBoardID)
    {
        if (!isAuthenticated) return;
        PlayGamesPlatform.Instance.ShowLeaderboardUI(leaderBoardID);
    }

    public void ShowAchievements()
    {
        if (!isAuthenticated) return;
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    // public void LoadFriends()
    // {

    // }

    // public void ViewProfile()
    // {
    //     if (!isAuthenticated) return;
    //     PlayGamesPlatform.Instance
    //     .ShowCompareProfileWithAlternativeNameHintsUI(userId, null, null, res => { });
    // }
}