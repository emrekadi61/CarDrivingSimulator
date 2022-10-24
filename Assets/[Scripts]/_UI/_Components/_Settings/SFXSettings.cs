using UnityEngine;
using UnityEngine.Events;

public class SFXSettings : MonoBehaviour
{
    public RectTransform rect { get { return GetComponent<RectTransform>(); } }
    public RectTransform musicSliderRect;
    public RectTransform fxSliderRect;
    public RectTransform hapticSelectorRect;

    private CustomSlider musicSlider;
    private CustomSlider fxSlider;
    private Selector hapticSelector;

    private UnityAction<float> onMusicChanged;
    private UnityAction<float> onFXChanged;
    private UnityAction<int> onHapticChanged;

    public SFXSettings Construct(UnityAction<float> onMusicChanged = null, UnityAction<float> onFXChanged = null, UnityAction<int> onHapticChanged = null)
    {
        this.onMusicChanged = onMusicChanged;
        this.onFXChanged = onFXChanged;
        this.onHapticChanged = onHapticChanged;

        musicSlider = musicSliderRect
                                    .GetComponent<CustomSlider>()
                                    .Construct(GameManager.Instance.dataManager.user.gameData.settings.musicLevel, OnChangedMusicSlider);

        fxSlider = fxSliderRect
                                    .GetComponent<CustomSlider>()
                                    .Construct(GameManager.Instance.dataManager.user.gameData.settings.fxLevel, OnChangedFXSlider);

        hapticSelector = hapticSelectorRect
                                    .GetComponent<Selector>()
                                    .Construct(GameManager.Instance.dataManager.user.gameData.settings.haptic ? 1 : 0, OnSelectedHaptic);

        return this;
    }

    private void OnChangedMusicSlider(float val)
    {
        GameManager.Instance.dataManager.user.gameData.settings.musicLevel = val;
        GameManager.Instance.dataManager.SaveUser();
        onMusicChanged?.Invoke(val);
        GameManager.Instance?.audioManager?.SetVolume("theme", GameManager.Instance.currentScene.Contains("game") ? val * GameManager.Instance.statics.gameSceneMusicLevelFactor : val);
    }

    private void OnChangedFXSlider(float val)
    {
        GameManager.Instance.dataManager.user.gameData.settings.fxLevel = val;
        GameManager.Instance.dataManager.SaveUser();
        onFXChanged?.Invoke(val);
        GameManager.Instance?.audioManager?.SetVolume("tap", val);
    }

    private void OnSelectedHaptic(int index)
    {
        GameManager.Instance.dataManager.user.gameData.settings.haptic = index == 1;
        GameManager.Instance.dataManager.SaveUser();
        onHapticChanged?.Invoke(index);
    }
}