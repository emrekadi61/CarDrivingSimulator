using UnityEngine;
using UnityEngine.UI;
using Utils.UI;
using DG.Tweening;

public class ShopMenu : MonoBehaviour
{
    public RectTransform backgroundRect;
    public RectTransform patternRect;
    public RectTransform backButtonRect;
    public RectTransform titleTextRect;
    public RectTransform objectContainer;
    public RectTransform codeButtonRect;

    private Image background;
    private Image pattern;
    private Button backButton;
    private Text titleText;
    private Button codeButton;

    private RectTransform[] objectRects;

    private void Awake()
    {
        background = backgroundRect.GetComponent<Image>();
        pattern = patternRect.GetComponent<Image>();
        backButton = backButtonRect.GetButton(OnPressedBackButton);
        titleText = titleTextRect.GetComponent<Text>();
        codeButton = codeButtonRect.GetButton(OnPressedCodeButton);
    }

    private void Start()
    {
        Color c = background.color; c.a = 0f; background.color = c;
        c = titleText.color; c.a = 0f; titleText.color = c;
        Align();
        GameManager.Instance.analyticManager.Showed("shop-panel");
        Appear();
    }

    private void Align()
    {
        IAPObject[] objects = GameManager.Instance.dataManager.iap.objects.ToArray();
        int[] colCount = new int[2];
        colCount[1] = objects.Length / 2;
        colCount[0] = objects.Length - colCount[1];

        objectRects = new RectTransform[objects.Length];

        GameObject iapPrefab = Resources.Load<GameObject>("_ui/_shop-panel/iap-object");
        RectTransform sample = Instantiate(iapPrefab).GetComponent<RectTransform>();

        float paddingX = 50f;
        float paddingY = 25f;

        for (int i = 0; i < colCount.Length; i++)
        {
            float posY = (((float)(colCount.Length - 1)) / 2f) * (sample.rect.size.y + paddingY);
            posY -= i * (sample.rect.size.y + paddingY);
            AlignRow(posY, paddingX, colCount[i], sample, objects, iapPrefab);
        }

        Destroy(sample.gameObject);
    }

    private int objCnt = 0;
    private void AlignRow(float posY, float padding, int objectCount, RectTransform sample, IAPObject[] objects, GameObject prefab)
    {
        for (int i = 0; i < objectCount; i++)
        {
            // ShopObject so = Instantiate(prefab, objectContainer).GetComponent<ShopObject>().Construct(objects[objCnt], OnPressedBuyButton);
            ShopObject so = Instantiate(prefab, objectContainer).GetComponent<ShopObject>().Construct(objects[objCnt]);
            objectRects[objCnt] = so.rect;
            objCnt++;

            so.rect.anchorMin = so.rect.anchorMax = so.rect.pivot = new Vector2(0.5f, 0.5f);

            float posX = -(((float)(objectCount - 1)) / 2f) * (sample.rect.size.x + padding);
            posX += i * (sample.rect.size.x + padding);

            so.rect.anchoredPosition = new Vector2(posX, posY);
            so.rect.localScale = new Vector3(0f, 1f, 1f);
            so.rect.DOScaleX(1f, 0.5f).SetDelay(0.25f);
        }
    }

    // private void OnPressedBuyButton(IAPObject iapObject)
    // {
    //     GameManager.Instance.sdkManager.iapManager.Buy(iapObject.id, b =>
    //     {
    //         if (b) OnBoughted(iapObject);
    //     });
    //     GameManager.Instance.analyticManager.Clicked("shop-panel", "shop-object");
    // }

    // private void OnBoughted(IAPObject iapObject)
    // {
    //     if (iapObject.id.Contains("no_ads"))
    //     {
    //         GameManager.Instance.dataManager.user.boughtedNoAds = true;
    //         GameManager.Instance.dataManager.SaveUser();
    //         return;
    //     }

    //     if (iapObject.price.type >= 0) GameManager.Instance.currencyManager.Earn(iapObject.price);
    // }

    private void Appear(float duration = 0.25f)
    {
        titleText.DOFade(1f, duration);
        background.DOFade(1f, duration);
        pattern.DOFade(0.05f, duration);

        codeButtonRect.anchoredPosition += new Vector2(0f, 1000f);
        codeButtonRect.DOAnchorPosY(codeButtonRect.anchoredPosition.y - 1000f, duration);
    }

    private void Disappear(float duration = 0.25f)
    {
        for (int i = 0; i < objectRects.Length; i++)
        {
            objectRects[i].DOScaleX(0f, 0.25f);
        }
        titleText.DOFade(0f, duration);
        background.DOFade(0f, duration);
        codeButtonRect.DOAnchorPosY(codeButtonRect.anchoredPosition.y + 1000f, duration);
        pattern.DOFade(0f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnPressedBackButton()
    {
        Disappear();
        GameManager.Instance.analyticManager.Clicked("shop-panel", "back-button");
    }

    private void OnPressedCodeButton()
    {
        ShopCodePanel.Get(b =>
        {
            if (b) Disappear();
        },
        transform);
    }

    private void OnDestroy()
    {
        backButton?.onClick.RemoveAllListeners();
        codeButton?.onClick.RemoveAllListeners();
    }

    public static EKCanvas Get()
    {
        return Instantiate(Resources.Load<GameObject>("_ui/_shop-panel/shop-canvas")).AddComponent<EKCanvas>().Construct(null);
    }
}