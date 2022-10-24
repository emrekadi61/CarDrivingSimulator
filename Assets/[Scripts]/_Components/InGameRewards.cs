using UnityEngine;
using DG.Tweening;

public class InGameRewards : MonoBehaviour
{
    private float rotationSpeed = 50f;
    private bool collected;
    private Price price;

    private void Start()
    {
        price = new Price();
        price.type = transform.gameObject.name.Contains("dollar") ? 0 : 1;
        price.amount = price.type == 0 ? Random.Range(1, 6) * 500 : Random.Range(1, 3);
    }

    private void Update()
    {
        if (collected) return;
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime, Space.Self);
    }

    public void OnCollected(float duration = 0.5f)
    {
        collected = true;
        GetComponentInChildren<Collider>().enabled = false;
        transform.DORotateQuaternion(Quaternion.Euler(0, 360f * 6f, 0), duration);
        transform.DOScale(0.25f, duration).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        transform.DOJump(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), 1f, 1, duration);

        LevelManager.Instance.onMoneyEarn.Invoke(price);
    }
}
