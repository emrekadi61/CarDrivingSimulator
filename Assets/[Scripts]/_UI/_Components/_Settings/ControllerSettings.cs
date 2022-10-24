using UnityEngine;
using UnityEngine.Events;

public class ControllerSettings : MonoBehaviour
{
    public RectTransform rect { get { return GetComponent<RectTransform>(); } }
    private Selector controllerSelector;
    private UnityAction<int> onSelected;

    public ControllerSettings Construct(UnityAction<int> onSelected = null)
    {
        this.onSelected = onSelected;
        controllerSelector = GetComponent<RectTransform>()
                                    .GetComponent<Selector>()
                                    .Construct(GameManager.Instance.dataManager.user.gameData.settings.controllerType, OnSelectedController);

        return this;
    }

    private void OnSelectedController(int index)
    {
        GameManager.Instance.dataManager.user.gameData.settings.controllerType = index;
        GameManager.Instance.dataManager.SaveUser();
        onSelected?.Invoke(index);
    }
}