using UnityEngine;
using UnityEngine.Events;
using Firebase.Firestore;

public class FirebaseCouponChecker : MonoBehaviour
{
    public async void CheckCode(string code, UnityAction<Price> onComplete)
    {
        FirebaseFirestore firestore = FirebaseFirestore.DefaultInstance;
        DocumentReference dr = firestore.Collection("coupon").Document(code);
        DocumentSnapshot ds = await dr.GetSnapshotAsync();

        if (ds.Exists)
        {
            Coupon c = ds.ConvertTo<Coupon>();
            Price p = new Price(c.type, c.amount);
            c.remainder -= 1;

            if (c.remainder <= 0) await dr.DeleteAsync();
            else await dr.SetAsync(c);

            onComplete?.Invoke(p);
        }
        else onComplete?.Invoke(null);
    }
}

[FirestoreData]
public class Coupon
{
    [FirestoreProperty] public int type { get; set; }
    [FirestoreProperty] public int amount { get; set; }
    [FirestoreProperty] public int remainder { get; set; }
}