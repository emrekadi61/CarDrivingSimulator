using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RatePanel : MonoBehaviour
{
    private readonly string RATE_DATA = "rate-panel-rate-result";
    private readonly string SURVEY_DATA = "rate-panel-survey-result";

    private int rateResult;
    private int surveyResult;

    public RectTransform backgroundRect;
    public RectTransform patternRect;

    private RectTransform rect;
    private Image background;
    private Image pattern;

    private RatePanelModel rateData;
    private int langIndex = 0;

    public RatePanel Construct()
    {
        rateData = GameManager.Instance.dataManager.ratePanel;
        langIndex = rateData.datas.FindIndex(c => c.lang == GameManager.Instance.deviceLanguage);
        if (langIndex == -1) langIndex = 0;

        if (!rateData.levels.Contains(LevelManager.Instance.playCount))
        {
            Destroy(gameObject);
            return this;
        }

        surveyResult = PlayerPrefs.GetInt(SURVEY_DATA, -1);
        rateResult = PlayerPrefs.GetInt(RATE_DATA, -1);

        if (rateResult == 0 || rateResult == 1)
        {
            Destroy(gameObject);
            return this;
        }

        rect = GetComponent<RectTransform>();
        background = backgroundRect.GetComponent<Image>();
        pattern = patternRect.GetComponent<Image>();

        Appear();

        return this;
    }

    private void Appear(float duration = 0.25f)
    {
        Color c = background.color; c.a = 0f; background.color = c;
        c = pattern.color; c.a = 0f; pattern.color = c;

        background.DOFade(0.8f, duration);
        pattern.DOFade(0.05f, duration);

        if (surveyResult == -1)
        {
            RatePanelMiniSurvey.Get(OnSurveyResult, rect, rateData.datas[langIndex].survey);
            return;
        }

        RatePanelMain.Get(OnRatingResult, rect, rateData.datas[langIndex].main);
    }

    private void Disappear(float duration = 0.25f)
    {
        background.DOFade(0f, duration);
        pattern.DOFade(0f, duration).OnComplete(() => Destroy(gameObject));
    }

    private void OnSurveyResult(bool result)
    {
        PlayerPrefs.SetInt(SURVEY_DATA, result ? 1 : 0);
        PlayerPrefs.Save();

        if (result)
        {
            RatePanelMain.Get(OnRatingResult, rect, rateData.datas[langIndex].main);
            return;
        }
        Disappear();
    }

    private void OnRatingResult(int result)
    {
        PlayerPrefs.SetInt(RATE_DATA, result);
        PlayerPrefs.Save();

        if (result == 0)
        {
            ReviewManagerGP.Get(() => Disappear());
            return;
        }

        Disappear();
    }

    public static EKCanvas Get()
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/_rate-panel/rate-canvas");
        RatePanel rp = Instantiate(prefab).GetComponent<RatePanel>().Construct();
        return rp.gameObject.AddComponent<EKCanvas>().Construct(null);
    }
}

//  ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//  ||                            ||                          ||
//  ||   SURVEY RESULT            ||  RATE RESULT             ||
//  ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
//  ||   -1 => no any result      ||  -1 => no any result     ||
//  ||    0 => yes                ||   0 => yes               ||
//  ||    1 => no                 ||   1 => no                ||
//  ||                            ||   2 => later             ||
//  ||                            ||                          ||
//  ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||