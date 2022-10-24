using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class IAPManager : ManagerBase, IStoreListener
{
    public string noAdsKey = "no_ads";

    private IStoreController controller;
    private IExtensionProvider extensions;
    private bool isInitialized { get { return controller != null && extensions != null; } }
    private UnityAction<bool> onComplete;

    private void Awake() => base.onConstructed.AddListener(this.OnConstructed);

    private void OnConstructed()
    {
        base.onConstructed.RemoveListener(this.OnConstructed);
        Initialize();
    }

    private void Initialize()
    {
        if (isInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for (int i = 0; i < GameManager.Instance.dataManager.iap.objects.Count; i++)
        {
            IAPObject obj = GameManager.Instance.dataManager.iap.objects[i];
            builder.AddProduct(obj.id, obj.consumable ? ProductType.Consumable : ProductType.NonConsumable);
        }

        Unity.Services.Core.UnityServices.InitializeAsync();
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error) { }
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) { }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (System.String.Equals(purchaseEvent.purchasedProduct.definition.id, processId, System.StringComparison.Ordinal))
            onComplete?.Invoke(true);
        else
            onComplete?.Invoke(false);

        onComplete = null;
        processId = null;
        return PurchaseProcessingResult.Complete;
    }

    public string GetPrice(string id)
    {
        if (controller == null || controller.products == null || controller.products.WithID(id) == null) return "none";
        return controller.products.WithID(id).metadata.localizedPriceString;
    }

    private string processId;
    public void Buy(string id, UnityAction<bool> onComplete = null)
    {
        if (!isInitialized) return;
        this.onComplete = onComplete;
        processId = id;
        Product p = controller.products.WithID(id);
        if (p != null && p.availableToPurchase) controller.InitiatePurchase(p);
    }
}