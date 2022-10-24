using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Utils.UI;

public class CustomizePanel : MonoBehaviour
{
    public RectTransform panel;
    public RectTransform closeButtonRect;

    public RectTransform bodyButtonRect;
    public RectTransform windowButtonRect;
    public RectTransform suspensionButtonRect;
    public RectTransform rimButtonRect;
    public RectTransform plateButtonRect;

    private Button closeButton;
    private Button bodyButton;
    private Button windowButton;
    private Button suspensionButton;
    private Button rimButton;
    private Button plateButton;

    private UnityAction<bool> onComplete;

    public CustomizePanel Construct(UnityAction<bool> onComplete)
    {
        this.onComplete = onComplete;
        closeButton = closeButtonRect.GetButton(OnPressedCloseButton);
        bodyButton = bodyButtonRect.GetButton(OnPressedBodyButton);
        windowButton = windowButtonRect.GetButton(OnPressedWindowButton);
        suspensionButton = suspensionButtonRect.GetButton(OnPressedSuspensionButton);
        rimButton = rimButtonRect.GetButton(OnPressedRimButton);
        plateButton = plateButtonRect.GetButton(OnPressedPlateButton);

        panel.anchoredPosition += new Vector2(0f, -1000f);

        onComplete?.Invoke(true);

        GameManager.Instance.analyticManager.Showed("customize-panel");

        return this;
    }

    private void Start()
    {
        Appear();
    }

    private void OnPressedCloseButton()
    {
        closeButton?.onClick.RemoveAllListeners();
        Disappear();
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("customize-panel", "back-button");
    }

    private void OnPressedBodyButton()
    {
        HidePanel(true);
        UIManager.Instance.Push(BodyPanel.Get(() =>
        {
            HidePanel(false);
        }));
        GameManager.Instance.audioManager.Play("tap");

        GameManager.Instance.analyticManager.Clicked("customize-panel", "body-button");
    }

    private void OnPressedWindowButton()
    {
        HidePanel(true);
        UIManager.Instance.Push(WindowPanel.Get(() =>
        {
            HidePanel(false);
        }));
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-panel", "window-button");
    }

    private void OnPressedSuspensionButton()
    {
        HidePanel(true);
        UIManager.Instance.Push(SuspensionPanel.Get(() =>
        {
            HidePanel(false);
        }));
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-panel", "suspension-button");
    }

    private void OnPressedRimButton()
    {
        HidePanel(true);
        UIManager.Instance.Push(RimPanel.Get(() =>
        {
            HidePanel(false);
        }));
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-panel", "rim-button");
    }

    private void OnPressedPlateButton()
    {
        HidePanel(true);
        UIManager.Instance.Push(PlatePanel.Get(() =>
        {
            HidePanel(false);
        }));
        GameManager.Instance.audioManager.Play("tap");
        GameManager.Instance.analyticManager.Clicked("customize-panel", "plate-button");
    }

    private void HidePanel(bool hide, float duration = 0.25f)
    {
        panel.DOComplete();
        panel.DOAnchorPosY(panel.anchoredPosition.y - (panel.rect.size.y + 50f) * (hide ? 1f : -1f), duration);
        closeButton.gameObject.SetActive(!hide);
        ShowroomManager.Instance.current.SetOpenableParts(0.5f);
    }

    private void Appear(float duration = 0.25f)
    {
        panel.DOAnchorPosY(panel.anchoredPosition.y + 1000f, duration);
        ShowroomManager.Instance.current.SetOpenableParts(0.5f);
    }

    private void Disappear(float duration = 0.25f)
    {
        ShowroomManager.Instance.current.SetOpenableParts(0.5f);
        onComplete?.Invoke(false);
        panel
        .DOAnchorPosY(panel.anchoredPosition.y - 1000f, duration)
        .OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        closeButton?.onClick.RemoveAllListeners();
        bodyButton?.onClick.RemoveAllListeners();
        windowButton?.onClick.RemoveAllListeners();
        suspensionButton?.onClick.RemoveAllListeners();
        rimButton?.onClick.RemoveAllListeners();
        onComplete = null;
    }

    public static EKCanvas Get(UnityAction<bool> onComplete)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_showroom/_customize-panel/main-canvas");
        CustomizePanel cp = Instantiate(prefab).GetComponent<CustomizePanel>().Construct(onComplete);
        return cp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}
