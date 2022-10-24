using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Google.Play.Review;

public class ReviewManagerGP : MonoBehaviour
{
    private ReviewManager reviewManager;
    private PlayReviewInfo playReviewInfo;

    private UnityAction onComplete;

    public void Construct(UnityAction onComplete)
    {
        this.onComplete = onComplete;

        if (!GameManager.Instance.statics.useInAppReview)
        {
            Application.OpenURL(GameManager.Instance.statics.rateURL);
            onComplete?.Invoke();
            Destroy(gameObject);
            return;
        }

        reviewManager = new ReviewManager();
        if (reqCor == null) reqCor = StartCoroutine(RequestReview());
    }

    private Coroutine reqCor;
    private IEnumerator RequestReview()
    {
        var requestFlowOperation = reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.LogWarning(requestFlowOperation.Error.ToString());
            yield break;
        }
        playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
        yield return launchFlowOperation;
        playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.LogWarning("Review Error: " + requestFlowOperation.Error.ToString());
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.

        onComplete?.Invoke();
        Destroy(gameObject);
        reqCor = null;
    }

    private void OnDestroy()
    {
        if (reqCor != null) StopCoroutine(reqCor);
    }

    public static void Get(UnityAction onComplete)
    {
        GameObject go = new GameObject("review-manager");
        go.AddComponent<ReviewManagerGP>().Construct(onComplete);
    }
}