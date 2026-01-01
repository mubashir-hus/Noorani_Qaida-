using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SplashLoader : MonoBehaviour
{
   void Start()
    {
        Time.timeScale = 1;
        Ads.Instance.RequestInterstitial();
        Ads.Instance.CreateAndLoadRewardedAd(); 

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        StartCoroutine(LoadMainMenu());

    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(3f);
        
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
       
    }
}

