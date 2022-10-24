using UnityEngine;
using UnityEngine.Events;
using Firebase.Messaging;

public class FirebaseCloudMessaging : MonoBehaviour
{
    public readonly string DATA = "firebase-cloud-messaging-token-received";
    private UnityAction onTokenReceived;

    public FirebaseCloudMessaging Initialize(UnityAction onTokenReceived = null)
    {
        this.onTokenReceived = onTokenReceived;

        if (PlayerPrefs.GetInt(DATA, 0) == 1) onTokenReceived?.Invoke();
        else GetToken();
        return this;
    }

    private void GetToken()
    {
        FirebaseMessaging.TokenReceived += OnTokenReceived;
        FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        PlayerPrefs.SetInt(DATA, 1);
        onTokenReceived?.Invoke();
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e) { }
}