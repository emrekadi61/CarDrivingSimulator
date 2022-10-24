using UnityEngine;
using UnityEngine.Events;

public class GraphicQualitySettings : MonoBehaviour
{
    public RectTransform rect { get { return GetComponent<RectTransform>(); } }
    private Selector qualitySelector;
    private UnityAction<int> onSelected;

    public GraphicQualitySettings Construct(UnityAction<int> onSelected = null)
    {
        this.onSelected = onSelected;
        qualitySelector = GetComponent<RectTransform>()
                               .GetComponent<Selector>()
                               .Construct(GameManager.Instance.dataManager.user.gameData.settings.qualityLevel, OnSelectedQuality);

        return this;
    }

    private void OnSelectedQuality(int index)
    {
        GameManager.Instance.dataManager.user.gameData.settings.qualityLevel = index;
        GameManager.Instance.dataManager.SaveUser();
        QualitySettings.SetQualityLevel(index);
        onSelected?.Invoke(index);
        GameManager.Instance.OnChangedGraphicQuality(index);
    }
}