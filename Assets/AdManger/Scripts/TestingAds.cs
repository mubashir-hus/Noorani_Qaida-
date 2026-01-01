using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingAds : MonoBehaviour
{

    //rectangle
    public void ShowRecatngle()
    {
        Ads.Instance.RequestRectangleAd();
    }
    //banners
    public void RequestBannerAd()
    {
        Ads.Instance.RequestBannerAdMainMenu();   
    }

    //Interstitails
    public void RequestInterstial()
    {
        Ads.Instance.RequestInterstitial();
    }
    public void ShowInterstial()
    {
        Ads.Instance.ShowInterstitial();
    }

    //rewarded
    public void RequestRewarded()
    {
        Ads.Instance.CreateAndLoadRewardedAd();
    }
    public void ShowRewarded()
    {
        Ads.Instance.ShowRewardedAd();
    }
   

}
