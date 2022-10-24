using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ShowroomCameraManager : Singleton<ShowroomCameraManager>
{
    [Header("DISTANCE"), Space(5)]
    public float distance = 6.0f;

    [Header("SPEED's"), Space(5)]
    public float xSpeed = 25f;
    public float ySpeed = 12f;
    public float friction = 5f;

    [Header("LIMIT's"), Space(5)]
    public float yMinLimit = 10f;
    public float yMaxLimit = 50f;

    [Header("POINT's"), Space(5)]
    public CameraPoint[] points;

    private float x = 0f;
    private float y = 0f;
    private float currentDistance;
    private Vector2 delta;
    private Transform orbitTarget;

    private Vector2 input { get { return InputManagerForOrbit.Instance.input; } }
    private bool pressing { get { return InputManagerForOrbit.Instance.pressing; } }
    private bool rotating = true;
    [HideInInspector] public bool orbitable;

    private void Start()
    {
        orbitTarget = new GameObject("orbit-target").transform;
        orbitTarget.position = Vector3.zero;
        orbitable = true;

        CameraPoint cp = FindPoint("default");
        x = cp.x;
        y = cp.y;

        currentDistance = distance;
    }

    private void Update()
    {
        if (orbitTarget == null || InputManagerForOrbit.Instance == null || !orbitable) return;

        if (rotating)
        {
            if (pressing) delta = InputManagerForOrbit.Instance.delta;
            else delta = Vector2.Lerp(delta, Vector2.zero, Time.deltaTime * friction);
            x += delta.x * xSpeed * Time.deltaTime;
            y -= delta.y * xSpeed * Time.deltaTime;
        }

        if (x > 360f) x -= 360f;
        else if (x < -360f) x += 360f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion r = Quaternion.Euler(y, x, 0f);
        Vector3 p = r * new Vector3(0f, 0.5f, -distance) + orbitTarget.position;

        transform.position = p;
        transform.rotation = r;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void Focus(string _point, float duration = 1f, UnityAction onComplete = null)
    {
        CameraPoint cp = FindPoint(_point);
        if (cp == null) return;
        rotating = false;
        DOTween.To(() => x, val => x = val, cp.x, duration);
        DOTween.To(() => y, val => y = val, cp.y, duration);
        DOTween.To(() => distance, val => distance = val, cp.distance, duration)
        .OnComplete(() =>
        {
            rotating = true;
            onComplete?.Invoke();
        });
    }

    public void Focus(CameraPoint cp, float duration = 1f, UnityAction onComplete = null)
    {
        if (cp == null) return;

        rotating = false;
        DOTween.To(() => x, val => x = val, cp.x, duration);
        DOTween.To(() => y, val => y = val, cp.y, duration);
        DOTween.To(() => distance, val => distance = val, cp.distance, duration)
        .OnComplete(() =>
        {
            rotating = true;
            onComplete?.Invoke();
        });
    }

    public CameraPoint GetCurrentPoint() { return new CameraPoint(x, y, distance); }

    private CameraPoint FindPoint(string _name) { return points.ToList().Find(c => c.name == _name); }

    [System.Serializable]
    public class CameraPoint
    {
        public string name;
        public float x;
        public float y;
        public float distance;

        public CameraPoint(float x, float y, float distance, string name = null)
        {
            this.x = x;
            this.y = y;
            this.distance = distance;
            this.name = name;
        }
    }
}