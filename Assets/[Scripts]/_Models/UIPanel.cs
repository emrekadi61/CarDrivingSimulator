[System.Serializable]
public class UIPanel
{
    public string name;
    public string path;
    public PanelType type;
}

public enum PanelType : int
{
    OrbitPanel = 0,
    ShowroomMainPanel = 1,
    SettingsPanel = 2,
    GameMainPanel = 3,
    GamePusePanel = 4
}