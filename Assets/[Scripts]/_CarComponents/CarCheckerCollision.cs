using UnityEngine;

public class CarCheckerCollision : MonoBehaviour
{
    private RCC_CarControllerV3 rcc;
    private RCC_AICarController ai;

    public CarCheckerCollision Construct(RCC_CarControllerV3 rcc)
    {
        this.rcc = rcc;
        ai = GetComponent<RCC_AICarController>();
        return this;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Props")) return;

        if (other.gameObject.GetComponent<MeshRenderer>())
        {
            BreakableObject bo = other.gameObject.GetComponent<BreakableObject>();
            if (!bo) bo = other.gameObject.AddComponent<BreakableObject>();
            bo.OnCollision(rcc);
        }
        else if (other.transform.parent.GetComponent<MeshRenderer>())
        {
            BreakableObject bo = other.transform.parent.GetComponent<BreakableObject>();
            if (!bo) bo = other.transform.parent.gameObject.AddComponent<BreakableObject>();
            bo.OnCollision(rcc);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ai != null) return;
        InGameRewards r = other.GetComponentInParent<InGameRewards>();
        if (!r) return;
        r.OnCollected();
    }
}