using UnityEngine;
using DG.Tweening;

public class OpenablePart : MonoBehaviour
{
    public Vector3 targetRotation;
    // public Vector3 targetPosition;

    private Vector3 normalRotation;
    // private Vector3 normalPosition;

    private void Start()
    {
        // normalPosition = transform.localPosition;
        normalRotation = transform.localEulerAngles;
    }

    private bool isOpen;
    public void Set(float duration = 1f)
    {
        transform.DOKill();
        transform.DOLocalRotate(isOpen ? normalRotation : targetRotation, duration);
        // transform.DOLocalMove(isOpen ? normalPosition : targetPosition, duration);
        isOpen = !isOpen;
    }
}