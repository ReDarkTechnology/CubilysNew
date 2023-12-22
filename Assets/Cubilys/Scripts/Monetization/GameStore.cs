using UnityEngine;
// using UnityEngine.Advertisements;
using UnityEngine.Events;
using System.Collections;

namespace Cubilys.Monetization
{
    public class GameStore : MonoBehaviour
    {
        [Header("IF YOU'RE REMOVING/MODIFYING THE GAME TO CHEAT, PLEASE CONSIDER DONATING AT https://ko-fi.com/bunzhizendi" +
            "I DON'T HAVE THAT MUCH INCOME JUST FROM MAKING ONE GAME, SO PLEASE SUPPORT ME!")]
        public const string appID = "4931475";
        public string placementId = "video";

        public UnityEvent OnAdFinished;
        public UnityEvent OnAdSkipped;
        public UnityEvent OnAdFailed;

        private void Start()
        {
            //Advertisement.Initialize(appID, Application.isEditor);
        }

        public void ShowAd()
        {
            //StartCoroutine(ShowAdAsync(request));
        }

        IEnumerator ShowAdAsync()
        {
            yield return null;
            /*Debug.Log("AD: Started showing process");
            int attempts = 0;

            while (!Advertisement.isInitialized)
            {
                Debug.Log("AD: Isn't initialized, returning in 0.25 seconds");
                attempts++;
                yield return new WaitForSeconds(0.25f);
            }

            if (attempts >= 50)
            {
                Debug.Log("AD: Too much attempts in waiting for the ad to be ready (+50)");
                OnAdFailed.Invoke();
                if (request.onAdShowed != null)
                    request.onAdShowed.Invoke(ShowResult.Failed);
            }
            else
            {
                attempts = 0;
                while (!Advertisement.IsReady(placementId) && attempts < 50)
                {
                    Debug.Log("AD: Isn't ready, returning in 0.25 seconds");
                    attempts++;
                    yield return new WaitForSeconds(0.25f);
                }

                if (attempts >= 50)
                {
                    Debug.Log("AD: Too much attempts in waiting for the ad to be ready (+50)");
                    OnAdFailed.Invoke();
                    if (request.onAdShowed != null)
                        request.onAdShowed.Invoke(ShowResult.Failed);
                }
                else
                {
                    PlacementState ad = PlacementState.Disabled;
                    ad = Advertisement.GetPlacementState(placementId);
                    Debug.Log("AD: Getting advertisement placement state");

                    if (ad == PlacementState.Ready)
                    {
                        Debug.Log("AD: Is ready!");
                        var options = new ShowOptions();
                        options.resultCallback += WhenAdvFinished;
                        if (request.onAdShowed != null)
                            options.resultCallback += request.onAdShowed.Invoke;
                        Advertisement.Show(placementId, options);
                    }
                    else
                    {
                        Debug.Log("AD: Failed to be placed!");
                        OnAdFailed.Invoke();
                        if (request.onAdShowed != null)
                            request.onAdShowed.Invoke(ShowResult.Failed);
                    }
                }
            }*/
        }

        /*public void WhenAdvFinished(ShowResult result)
        {
            if (result == ShowResult.Finished) OnAdFinished.Invoke();
            else if (result == ShowResult.Skipped) OnAdSkipped.Invoke();
            else if (result == ShowResult.Failed) OnAdFailed.Invoke();
        }*/
    }

    /*[System.Serializable]
    public class AdRequest
    {
        public string adType = "video";
        public System.Action<ShowResult> onAdShowed;

        public AdRequest() { }
        public AdRequest(string type, System.Action<ShowResult> onEnd)
        {
            adType = type;
            onAdShowed = onEnd;
        }
    }*/
}