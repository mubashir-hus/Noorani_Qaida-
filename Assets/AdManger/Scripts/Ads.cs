using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class Ads : MonoBehaviour
{
    private static Ads instance;

    [Space(10)]
    [Header("Admob")]
    private AppOpenAd appOpenAd;
    public string admobAppId;
    public List<string> appOpenAdIds;
    int appOpenIndex = 0, appOpenFailedToLoadCounter = 0;
    private bool appOpenAdLoad, appOpenStopped;


    [Space(10)]
    [Header("Bannner")]
    private BannerView bannerView, rectangleAdBannerView;
    public List<string> bannerAdIds;
    [Space(5)]
    [SerializeField] private AdPosition bannerAdPositionMainMenu;
    [SerializeField] private BannerAdType mainMenuBannerType;
    [Space(10)]
    [SerializeField] private AdPosition gamePlayBannerAdPosition;
    [SerializeField] private BannerAdType gamePlayBannerType;
    [Space(10)]
    [Header("InterStitails")]
    public List<string> interstitalAdIds;
    private InterstitialAd interstitial;

    [Space(10)]
    [Header("Rectangle")]
    [SerializeField] private AdPosition rectangleAdPosition;
    [SerializeField] private bool useTestAds;
    [Header("Rewarded")]
    private RewardedAd rewardedAd;
    public List<string>  rewardedAdIds;
    [Header("Reactangle")]
    public string rectangleAdId;

    [Header("Privacy")]
    public string privacyPolicyLink;
    [Header("Consent")]
    public GameObject consentDialog;
    public static bool consent = false;
    [Header("Orientation")]
    public ScreenOrientation screenOrientation;

    [Header("Time OF App")]
    private DateTime appExpiry;
    private readonly TimeSpan appOpenTimeout = TimeSpan.FromHours(3);

    [HideInInspector] private int rewardedFC = 0, initAdIndex = 0,  rewardIndex = 0 ,interstitialFC = 0;
    private bool isInterstitialStopped = false,isRewardedStopped = false,initLoaded = false,rewardedLoaded = false;

    [HideInInspector]
    public UnityEvent onRewardedAdComplete;
    public static string removeAdsPrefs = "RemoveAds";
   

    public enum BannerAdType
    {
        Simple,
        Smart
    }
    public static Ads Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<Ads>();
            else if (instance == null)
               Debug.Log("Cannot Find Ads");
            return instance;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }

    private void Start()
    {

        if (PlayerPrefs.GetInt("consentShown") == 0)
        {
            consentDialog.SetActive(true);
            PlayerPrefs.SetInt("consentShown", 1);
        }
        else
        {
            if (PlayerPrefs.GetInt("Consent") == 1)
            {
                AcceptConsent();

            }
            else
            {
                DoNotAcceptGDPRConsenet();
            }
        }
        
    }

    #region app open region
    public bool IsAppOpenAdAvailable
    {
        get
        {
            return (appOpenAd != null
                    && appOpenAd.CanShowAd()
                    && DateTime.Now < appExpiry);
        }
    }

    void LoadAppOpenAgain()
    {


        if (!IsAppOpenAdAvailable && appOpenAdLoad == false)
        {
            appOpenIndex = (appOpenIndex + 1) % appOpenAdIds.Count;
        }

        RequestAndLoadAppOpenAd();

    }
    private void RequestAndLoadAppOpenAd()
    {


        AdRequest request = new AdRequest();
        request.Extras.Add("npa", consent ? "1" : "0");
        if (appOpenAd != null)
        {
            DestroyAppOpenAd();
        }
#if UNITY_ANDROID
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/9257395921" : appOpenAdIds[appOpenIndex];
#elif UNITY_IPHONE

        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/9257395921" : appOpenAdIds[appOpenIndex];
#endif
        // Create a new app open ad instance.
        AppOpenAd.Load(adUnitId, request,
            (AppOpenAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {

                    PrintStatus("App open ad failed to load with error: " +
                         loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {

                    PrintStatus("App open ad failed to load.");
                    return;
                }


                PrintStatus("App Open ad loaded. Please background the app and return.");
                appOpenAd = ad;
                appExpiry = DateTime.Now + appOpenTimeout;

                ad.OnAdFullScreenContentOpened += () =>
                {

                    PrintStatus("App open ad opened.");

                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                    RequestAndLoadAppOpenAd();
                    PrintStatus("App open ad closed.");

                };
                ad.OnAdImpressionRecorded += () =>
                {

                    PrintStatus("App open ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {

                    PrintStatus("App open ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {

                    PrintStatus("App open ad failed to show with error: " +
                         error.GetMessage());
                    if (appOpenStopped)
                        return;


                    appOpenAdLoad = false;

                    if (appOpenFailedToLoadCounter++ < 20)
                    {
                        double retryDelay = Math.Pow(2, Math.Min(6, appOpenFailedToLoadCounter));
                        //await Task.Delay(2000);

                        // LoadAppOpenAgain();
                        Invoke(nameof(LoadAppOpenAgain), (float)retryDelay);
                    }
                    else
                    {
                        PrintStatus("Appopen is not available Right Now");
                        appOpenStopped = true;
                    }
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "App open ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);

                    PrintStatus(msg);
                };
            });

    }
    public void DestroyAppOpenAd()
    {
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }
    }
    private void ShowAppOpneAdIfAvailable()
    {
        if (!IsAppOpenAdAvailable)
        {
            return;
        }

        appOpenAd.Show();

    }
    private void OnAppStateChanged(AppState state)
    {
        // Display the app open ad when the app is foregrounded.
        PrintStatus("App State is " + state);

        // OnAppStateChanged is not guaranteed to execute on the Unity UI thread.
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (state == AppState.Foreground)
            {
                ShowAppOpneAdIfAvailable();
            }
        });
    }
    #endregion

    #region Helpers Region
    public bool IsRewardedAvailable()
    {
        if(rewardedAd == null)
        {
            return false;
        }
        else
            return rewardedAd.CanShowAd();
    }
  

    public void IntializeMobileAds()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            if(PlayerPrefs.GetInt(removeAdsPrefs) != 1)
            {
                CreateBannerView(0, mainMenuBannerType, bannerAdPositionMainMenu);
                RequestBannerAdMainMenu();

            }
            RequestInit(0);
            LoadRewardedAd();
            RequestAndLoadAppOpenAd();  
          
          
        });
    }

    public void AcceptConsent()
    {

        consentDialog.SetActive(false);
        PlayerPrefs.SetInt("Consent", 1);
        consent = true;
        IntializeMobileAds();
        SceneManager.LoadScene(1);
    }

    public void DoNotAcceptGDPRConsenet()
    {

        consentDialog.SetActive(false);
        PlayerPrefs.SetInt("Consent", 0);
        consent = false;
        IntializeMobileAds();
        SceneManager.LoadScene(1);
    }

    public void ShowPrivacy()
    {
        Application.OpenURL(privacyPolicyLink);
    }

    private void PrintStatus(string message)
    {
        
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            PrintStatus(message);
        });
    }

    public void RequestBannerAdMainMenu()
    {
        if (PlayerPrefs.GetInt(removeAdsPrefs) == 1)
            return;
        CreateBannerView(0, mainMenuBannerType, bannerAdPositionMainMenu);
        AdRequest adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }
    
    public void RequestBannerAdGamePlay()
    {
        if (PlayerPrefs.GetInt(removeAdsPrefs) == 1)
            return;
        CreateBannerView(1, gamePlayBannerType,gamePlayBannerAdPosition);
        AdRequest adRequest = new AdRequest();
        bannerView.LoadAd(adRequest);
    }
   
    public void ShowBanner()
    {
        if(bannerView != null && PlayerPrefs.GetInt(removeAdsPrefs) != 1)
        {
            bannerView.Show();
        }
    }
    public void RequestInterstitial()
    {

        RequestInit(initAdIndex);
    }

    public void  CreateAndLoadRewardedAd()
    {
        LoadRewardedAd();
    }

    public void ShowInterstitial()
    {
        if (PlayerPrefs.GetInt(removeAdsPrefs) != 1)
        {
            if (interstitial == null)
                return;
            if(interstitial != null && interstitial.CanShowAd()) 
            {
                interstitial.Show();
            }
            else
            {
                Debug.LogError("Interstital is Not Avaliable Requesting Again*****");
                RequestInit(initAdIndex);
            }
        }
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
               
                onRewardedAdComplete?.Invoke();
                PrintStatus("Rewarded ad granted a reward: " + reward.Amount);
            });
        }
        else
        {
              AndroidDialogAndToastBinding.instance.toastLong("Sorry! No Video is available. Kindly Try Again Later or Check Your Internet.");
            PrintStatus("Rewarded ad is not ready yet.");
           CreateAndLoadRewardedAd();
        }
    }


    public void HideBanner()
    {
        if(bannerView == null)
        {
            bannerView.Hide();  
        }
    }

    public void HideRectangle()
    {
        if (rectangleAdBannerView == null)
            return;
        else
            rectangleAdBannerView.Hide();
    }
    #endregion
   
    #region BannerAds

    private void CreateBannerView(int idIndex,  BannerAdType type, AdPosition pos)
    {
      
        PrintStatus("Creating banner view*********");
#if UNITY_ANDROID
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/6300978111" : bannerAdIds[rewardIndex];
#elif UNITY_IPHONE
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/2934735716" : bannerAdIds[rewardIndex];

#endif
        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyBanner();
        }
        if(type == BannerAdType.Simple)
        {
            // Create a 320x50 banner at top of the screen
            bannerView = new BannerView(bannerAdIds[idIndex], AdSize.Banner, pos);
        }
       else if(type == BannerAdType.Smart)
        {
            AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            bannerView = new BannerView(bannerAdIds[0], adaptiveSize, pos);

        }

        // Register for ad events.

        bannerView.OnBannerAdLoaded += () =>
        {
            PrintStatus("Banner ad loaded.");
           
        };
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            PrintStatus("Banner ad failed to load with error: " + error.GetMessage());
          
        };
        bannerView.OnAdImpressionRecorded += () =>
        {
            PrintStatus("Banner ad recorded an impression.");
        };
        bannerView.OnAdClicked += () =>
        {
            PrintStatus("Banner ad recorded a click.");
        };
        bannerView.OnAdFullScreenContentOpened += () =>
        {
            PrintStatus("Banner ad opening.");
          
        };
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Banner ad closed.");
           
        };
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Banner ad received a paid event.",
                                        adValue.CurrencyCode,
                                        adValue.Value);
            PrintStatus(msg);
        };

    }

    private void DestroyBanner()
    {
        bannerView.Destroy();
    }

  

#endregion

    #region Intertestial ads


    public void ReInitializeInit()
    {
        if (!isInterstitialStopped)
        {

            // Clean up interstitial ad before creating a new one.
            if (this.interstitial != null)
            {
                this.interstitial.Destroy();
            }
            if (!interstitial.CanShowAd() && !initLoaded)
            {
                initAdIndex = (initAdIndex + 1) % interstitalAdIds.Count;

            }
            RequestInit(initAdIndex);
        
        }
    }


    private void RequestInit(int initIndex)
    {


        AdRequest adRequest = new AdRequest();
        adRequest.Extras.Add("npa", consent ? "1" : "0");
#if UNITY_ANDROID
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/1033173712" : interstitalAdIds[initIndex];
#elif UNITY_IPHONE
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/4411468910" : interstitalAdIds[initIndex];

#endif
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                   
                    HandleInterstitialFailedToLoad(loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {
                   PrintStatus("Interstitial ad failed to load.");
                    HandleInterstitialFailedToLoad(loadError.GetMessage());
                    return;
                }

               PrintStatus("Interstitial ad loaded.");
                interstitial = ad;
                HandleInterstitialLoaded();
                ad.OnAdFullScreenContentOpened += () =>
                {
                   PrintStatus("Interstitial ad opening.");
                
                };
                ad.OnAdFullScreenContentClosed += () =>
                {
                   PrintStatus("Interstitial ad closed.");
                    ReInitializeInit();
                };
                ad.OnAdImpressionRecorded += () =>
                {

                   

                   PrintStatus("Interstitial ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {
                   PrintStatus("Interstitial ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                   PrintStatus("Interstitial ad failed to show with error: " +
                                error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Interstitial ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                   PrintStatus(msg);
                };
            });

      

    }

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            interstitialFC = 0;
            initLoaded = true;
            MonoBehaviour.print("HandleInterstitialLoaded event received");

          
        });
    }

    private void HandleInterstitialFailedToLoad(string msg)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {

            if (isInterstitialStopped)
                return;


            initLoaded = false;

            if (interstitialFC++ < 20)
            {
                double retryDelay = Math.Pow(2, Math.Min(6, interstitialFC));

                Invoke(nameof(ReInitializeInit), (float)retryDelay);
            }
            else
            {
               PrintStatus("Interstitail is not available Right Now");
                isInterstitialStopped = true;
            }
            MonoBehaviour.print(
                "HandleInterstitialFailedToLoad event received with message: " + msg);

          
        });
    }


    #endregion
#endregion

    #region Rewarded
    private void LoadRewardedAd()
    {

#if UNITY_ANDROID
        string adUnitId = useTestAds? "ca-app-pub-3940256099942544/5224354917" : rewardedAdIds[rewardIndex];
#elif UNITY_IPHONE
        string adUnitId = useTestAds? "ca-app-pub-3940256099942544/1712485313" : rewardedAdIds[rewardIndex];

#endif


        AdRequest adRequest = new AdRequest();
        adRequest.Extras.Add("npa", consent ? "1" : "0");
     
        
        // create new rewarded ad instance
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {
                   PrintStatus("Rewarded ad failed to load with error: " +
                                loadError.GetMessage());
                    HandleRewardedAdFailedToLoad(loadError.GetMessage());
                    return;
                }
                else if (ad == null)
                {

                   PrintStatus("Rewarded ad failed to load.");
                    HandleRewardedAdFailedToLoad(loadError.GetMessage());
                    return;
                }


               PrintStatus("Rewarded ad loaded.");
                rewardedAd = ad;
                HandleRewardedAdLoaded();
                ad.OnAdFullScreenContentOpened += () =>
                {

                   PrintStatus("Rewarded ad opening.");
                    // OnAdOpeningEvent.Invoke();
                };
                ad.OnAdFullScreenContentClosed += () =>
                {

                   PrintStatus("Rewarded ad closed.");
                    LoadRewardedAd();
                };
                ad.OnAdImpressionRecorded += () =>
                {

                   PrintStatus("Rewarded ad recorded an impression.");
                };
                ad.OnAdClicked += () =>
                {

                   PrintStatus("Rewarded ad recorded a click.");
                };
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    LoadRewardedAd();
                    PrintStatus("Rewarded ad failed to show with error: " +
                               error.GetMessage());
                };
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    string msg = string.Format("{0} (currency: {1}, value: {2}",
                                               "Rewarded ad received a paid event.",
                                               adValue.CurrencyCode,
                                               adValue.Value);
                   PrintStatus(msg);
                };
            });
    }

    void RequestRewarded()
    {
        rewardedAd?.Destroy();
            rewardIndex = (rewardIndex + 1) % rewardedAdIds.Count;
        LoadRewardedAd();
    }
   
    #region RewardedAd callback handlers

    public void HandleRewardedAdLoaded()
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            rewardedLoaded = true;
            MonoBehaviour.print("HandleRewardedAdLoaded event received");
        });
    }

    public void HandleRewardedAdFailedToLoad(string args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {
            if (isRewardedStopped)
                return;


            rewardedLoaded = false;

            if (rewardedFC++ < 20)
            {
                double retryDelay = Math.Pow(2, Math.Min(6, rewardedFC));

                Invoke(nameof(RequestRewarded), (float)retryDelay);
            }
            else
            {
               PrintStatus("Rewarded is not available Right Now");
                isRewardedStopped = true;
            }
            MonoBehaviour.print(
                "HandleRewardedAdFailedToLoad event received with message: " + args);
        });
    }

    #endregion
    #endregion

    #region Recatangle_Ad
    public void RequestRectangleAd()
    {
#if UNITY_ANDROID
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/6300978111" : rectangleAdId;
#elif UNITY_IPHONE
        string adUnitId = useTestAds ? "ca-app-pub-3940256099942544/2934735716" : rectangleAdId;

#endif

        // Clean up banner ad before creating a new one.
        if (rectangleAdBannerView != null)
        {
            rectangleAdBannerView.Destroy();
        }

        // Create a 320x50 banner at the top of the screen.
        rectangleAdBannerView = new BannerView(adUnitId, AdSize.MediumRectangle, rectangleAdPosition);

        rectangleAdBannerView.OnBannerAdLoaded += () =>
        {
            PrintStatus("**************Rectangle Loaded*************");

        };

        rectangleAdBannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            PrintStatus("Rectangle ad failed to load with error: " + error.GetMessage());
            HandleRectangleAdFailedToLoad(error.GetMessage());

        };
        rectangleAdBannerView.OnAdFullScreenContentOpened += () =>
        {
            PrintStatus("Rectangle ad opening.");

        };
        rectangleAdBannerView.OnAdFullScreenContentClosed += () =>
        {
            PrintStatus("Rectangle ad closed.");

        };
        rectangleAdBannerView.OnAdPaid += (AdValue adValue) =>
        {
            string msg = string.Format("{0} (currency: {1}, value: {2}",
                                        "Rectangle ad received a paid event.",
                                        adValue.CurrencyCode,
                                        adValue.Value);
            PrintStatus(msg);
        };
        AdRequest adRequest = new AdRequest();
        adRequest.Extras.Add("npa", consent ? "1" : "0");
        rectangleAdBannerView.LoadAd(adRequest);
    }
    #region RectangleAd callback handlers

    public void HandleRectangleAdLoaded(object sender, EventArgs args)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {

            MonoBehaviour.print("HandleAdLoaded event received");
        });
        //isRectangleLoaded = true;
    }

    public void HandleRectangleAdFailedToLoad(string err)
    {
        MobileAdsEventExecutor.ExecuteInUpdate(() =>
        {

            MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + err);
        });
    }

    #endregion
    #endregion


}
