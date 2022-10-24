using UnityEngine;

public class CarPhotoMode : MonoBehaviour
{
    public string id;
    private Car car;

    private void Start()
    {
        car = GetComponent<Car>();
        car.Construct(GameManager.Instance.dataManager.cars.cars.Find(c => c.id == id));
    }
}