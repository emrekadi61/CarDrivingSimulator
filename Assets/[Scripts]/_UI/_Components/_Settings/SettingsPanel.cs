using UnityEngine;
using UnityEngine.UI;
using Utils.UI;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    [Header("PANEL"), Space(5)]
    public RectTransform panel;
    public RectTransform closeButtonRect;

    [Header("OTHER"), Space(5)]
    public RectTransform privacyButtonRect;
    public RectTransform deleteDataButtonRect;
    public RectTransform versionTextRect;

    private GraphicQualitySettings graphic;
    private ControllerSettings controller;
    private SFXSettings sfx;

    #region Components
    private Button closeButton;
    private Button privacyButton;
    private Button deleteDataButton;
    #endregion

    private void Awake()
    {
        closeButton = closeButtonRect.GetButton(OnPressedCloseButton);
        panel.anchoredPosition += new Vector2(1000f, 0f);
        deleteDataButton = deleteDataButtonRect.GetButton(OnPressedDeleteDataButton);

        graphic = GetComponentInChildren<GraphicQualitySettings>().Construct();
        controller = GetComponentInChildren<ControllerSettings>().Construct();
        sfx = GetComponentInChildren<SFXSettings>().Construct();

        privacyButtonRect.GetButton(OnPressedPrivacyButton);
        versionTextRect.GetComponent<Text>().text = "Developed by <b>ZEK Games</b> || verion: " + Application.version;

        GameManager.Instance.analyticManager.Showed("settings-panel");
    }

    private void Start()
    {
        Appear();
    }

    private void OnPressedCloseButton()
    {
        Disappear();
    }

    private void Appear(float duration = 0.25f)
    {
        panel.DOComplete();
        panel.DOAnchorPosX(panel.anchoredPosition.x - 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        panel.DOComplete();
        panel
        .DOAnchorPosX(panel.anchoredPosition.x + 1000f, duration)
        .OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnPressedPrivacyButton()
    {
        Application.OpenURL(GameManager.Instance.statics.privacyURL);

        GameManager.Instance.analyticManager.Clicked("settings-panel", "privacy-button");
    }

    private void OnPressedDeleteDataButton()
    {
        DataDeletionDialog.Get(b =>
        {
            if (b) GameManager.Instance.dataManager.DeleteAllData();
        }, transform);
    }

    public static EKCanvas Get()
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_settings/settings-canvas");
        return Instantiate(prefab).AddComponent<EKCanvas>().Construct(null);
    }

    private void OnDestroy()
    {
        closeButton?.onClick.RemoveAllListeners();
        privacyButton?.onClick.RemoveAllListeners();
    }
}