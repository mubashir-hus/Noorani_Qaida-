using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;


public class ShareLink : MonoBehaviour
{

    string subject = "Noraani Qaida App";
    string body = "https://apps.apple.com/us/app/e-school-the-best-school/id1598557497";
    string text = "Noorani Qaida is developed by the E School For All The purpose is to help students learn how to recite the Holy Quran.Please download and use free of cost for your child and share with the ones you know.For I Phone: https://apps.apple.com/us/app/noorani-qaida-by-e-school/id1596916293  For Android (google Play)  https://play.google.com/store/apps/details?id=com.TheBrainStormers.NoraniQuaida  For Huawei https://appgallery.huawei.com/app/C105220025";

#if UNITY_IPHONE

     [DllImport("__Internal")]
     private static extern void sampleMethod(string iosPath, string message);

     [DllImport("__Internal")]
     private static extern void sampleTextMethod(string message);

#endif
    private void Start()
    {

    }
    public void OnAndroidTextSharingClick()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.TheBrainStormers.NoraniQuaida");
        //StartCoroutine(ShareAndroidText());

    }
    IEnumerator ShareAndroidText()
    {
        yield return new WaitForEndOfFrame();

        //execute the below lines if being run on a Android device
#if UNITY_ANDROID
        //Reference of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //Reference of AndroidJavaObject class for intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        //add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Text Sharing ");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);
        //get the current activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //start the activity by sending the intent data
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
        currentActivity.Call("startActivity", jChooser);
#endif
    }


    public void OniOSTextSharingClick()
    {

#if UNITY_IPHONE || UNITY_IPAD
         string shareMessage = "Wow I Just Share Text ";
         sampleTextMethod(shareMessage);

#endif
    }

    public void RateUs()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=YOUR_ID");
#elif UNITY_IPHONE
         Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif


    }
}