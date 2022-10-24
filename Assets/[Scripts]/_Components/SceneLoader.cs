using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public RectTransform backgroundRect;
    public RectTransform iconRect;
    public RectTransform sliderRect;
    public RectTransform sliderFillRect;
    public RectTransform sliderTextRect;
    public RectTransform hintRect;
    public RectTransform hintTextRect;

    private Image background;
    private Text hintText;
    private Text sliderText;

    private string sceneName;

    public SceneLoader Construct(string sceneName, bool direct = false)
    {
        this.sceneName = sceneName;
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (direct)
        {
            StartCoroutine(LoadSceneAsync(direct));
            backgroundRect.gameObject.SetActive(false);
            iconRect.gameObject.SetActive(false);
            sliderRect.gameObject.SetActive(false);
            hintRect.gameObject.SetActive(false);
            return this;
        }

        background = backgroundRect.GetComponent<Image>();
        hintText = hintTextRect.GetComponent<Text>();
        sliderText = sliderTextRect.GetComponent<Text>();

        sliderWidth = sliderFillRect.parent.GetComponent<RectTransform>().rect.size.x;

        hintText.text = GameManager.Instance.dataManager.hints.hints[Random.Range(0, GameManager.Instance.dataManager.hints.hints.Count)];

        Appear();
        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;
        iconRect.anchoredPosition += new Vector2(0f, -1000f);
        sliderRect.anchoredPosition += new Vector2(0f, -1000f);
        hintRect.anchoredPosition += new Vector2(0f, -1000f);

        background.DOFade(1f, duration);
        iconRect.DOAnchorPosY(iconRect.anchoredPosition.y + 1000f, duration);
        sliderRect.DOAnchorPosY(sliderRect.anchoredPosition.y + 1000f, duration);
        hintRect.DOAnchorPosY(hintRect.anchoredPosition.y + 1000f, duration).OnComplete(() => StartCoroutine(LoadSceneAsync()));
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration);
        iconRect.DOAnchorPosY(iconRect.anchoredPosition.y - 1000f, duration);
        sliderRect.DOAnchorPosY(sliderRect.anchoredPosition.y - 1000f, duration);
        hintRect.DOAnchorPosY(hintRect.anchoredPosition.y - 1000f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        Disappear();
    }

    private float sliderWidth;
    private void SetSliderFill(float val)
    {
        sliderFillRect.sizeDelta = new Vector2(val * sliderWidth, sliderFillRect.sizeDelta.y);
    }

    private IEnumerator LoadSceneAsync(bool direct = false)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (!direct)
        {
            while (!ao.isDone)
            {
                float progress = Mathf.Clamp01(ao.progress / 0.9f);
                sliderFillRect.sizeDelta = new Vector2(progress * sliderWidth, sliderFillRect.sizeDelta.y);
                sliderText.text = (int)(progress * 100f) + " %";
                yield return null;
            }
        }
        yield return null;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void Get(string sceneName, bool direct = false)
    {
        SceneLoader sl = Instantiate(Resources.Load<GameObject>("_prefabs/scene-loader")).GetComponent<SceneLoader>().Construct(sceneName, direct);
    }
}