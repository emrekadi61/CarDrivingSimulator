using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    public void OnCollision(RCC_CarControllerV3 other)
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.AddForce(Mathf.Clamp(other.speed, 1f, 30f) * (transform.position - other.transform.position).normalized, ForceMode.Impulse);

        Destroy(gameObject, 3f);
    }
}