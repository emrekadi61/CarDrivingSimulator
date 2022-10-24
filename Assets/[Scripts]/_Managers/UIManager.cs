using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public List<UIPanel> panels;
    [HideInInspector] public List<EKCanvas> canvasList = new List<EKCanvas>();

    private void Start()
    {
        OnConstructed();
    }

    private void OnConstructed()
    {
        for (int i = 0; i < panels.Count; i++)
        {
            EKCanvas ec = GetPanel(panels[i]);
            ec.gameObject.name = panels[i].name;
            ec.canvas.sortingOrder = canvasList.Count;
            canvasList.Add(ec);
        }
    }

    public void Push(EKCanvas ec)
    {
        canvasList.Add(ec);
        ec.transform.SetParent(transform);
        ec.canvas.sortingOrder = canvasList.Count - 1;
    }

    public void Pop(EKCanvas ec)
    {
        if (!canvasList.Contains(ec)) return;
        canvasList.Remove(ec);
        for (int i = 0; i < canvasList.Count; i++) canvasList[i].canvas.sortingOrder = i;
    }

    private EKCanvas GetPanel(UIPanel panel)
    {
        GameObject prefab = Resources.Load<GameObject>("_ui/" + panel.path);
        return Instantiate(prefab, transform).AddComponent<EKCanvas>().Construct(panel);
    }
}