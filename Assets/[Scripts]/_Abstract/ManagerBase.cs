using UnityEngine;
using UnityEngine.Events;

public abstract class ManagerBase : MonoBehaviour
{
    protected UnityEvent onConstructed = new UnityEvent();

    public ManagerBase Construct()
    {
        onConstructed.Invoke();
        return this;
    }

    private void OnDestroy()
    {
        onConstructed.RemoveAllListeners();
        onConstructed = null;
    }
}