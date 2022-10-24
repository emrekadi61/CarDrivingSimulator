using UnityEngine;
using DG.Tweening;

public class VideoCam1 : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 targetRot;

    public float duration = 2f;
    public float distance = 3f;

    private void Start()
    {
        transform.DOMove(targetPos, duration);
        transform.DORotate(targetRot, duration);
    }
}