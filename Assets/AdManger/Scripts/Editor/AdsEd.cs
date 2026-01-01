
using UnityEditor;
using UnityEngine;
using GoogleMobileAds.Editor;
using GooglePlayServices;
using System.IO;
using System.Collections.Generic;
using GoogleMobileAds;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Ads))]
public class AdsEd : Editor
{

    private string path = "Assets/AdManger/Scripts/Editor/idstext.txt";
    private List<string> adIdsList = new List<string>();
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Ads manager = (Ads)target;

        GUILayout.Space(20);
        if (GUILayout.Button("Ids Done"))
        {

            AssignIds(manager);
        }
        GUILayout.Space(10);
    }

    void Clean(Ads Ads)
    {
        Ads.admobAppId =  Ads.admobAppId.Trim();
        Ads.rectangleAdId =  Ads.rectangleAdId.Trim();

        for (int i = 0; i < 3; i++)
        {
            if(i<Ads.appOpenAdIds.Count)
            Ads.appOpenAdIds[i] =  Ads.appOpenAdIds[i].Trim();
            
            if (i < Ads.interstitalAdIds.Count)
                Ads.interstitalAdIds[i] = Ads.interstitalAdIds[i].Trim();
            
            if(i < Ads.bannerAdIds.Count)
            Ads.bannerAdIds[i] = Ads.bannerAdIds[i].Trim();
            
            if(i < Ads.rewardedAdIds.Count)
                Ads.rewardedAdIds[i] = Ads.rewardedAdIds[i].Trim();
        }
    }

    void AssigningIds(Ads Ads)
    {
        StreamReader reader = new StreamReader(path);
        string[] adIds = reader.ReadToEnd().Split('\n');
        Debug.Log(adIds.Length);
        reader.Close();
        adIdsList.Clear();
        for (int i = 0; i < adIds.Length; i++)
        {
            if (adIds[i].Contains("ca-app-pub-"))
                adIdsList.Add(adIds[i]);

        }
        Ads.appOpenAdIds.Clear();
        Ads.bannerAdIds.Clear();
        Ads.interstitalAdIds.Clear();
        Ads.rewardedAdIds.Clear();
        for (int i = 0; i < adIdsList.Count; i++)
        {
          
           if(i== 0)
            {
                Ads.admobAppId = adIdsList[i];
            }

           if(i >= 1 && i < 4)
            {
                Ads.appOpenAdIds.Add(adIdsList[i]);
            }
           if(i >=4 && i < 6)
            {
                Ads.bannerAdIds.Add(adIdsList[i]);
            }
           if(i == 6)
            {
                Ads.rectangleAdId = adIdsList[i];
            }
           if( i >= 7 && i < 10)
            {
                Ads.interstitalAdIds.Add(adIdsList[i]);
            }
           if(i == 10)
            {
                Ads.rewardedAdIds.Add(adIdsList[i]);
            }
        }
        if(adIds.Length > 33)
        {
                Ads.privacyPolicyLink = adIds[34];

#if UNITY_ANDROID
        
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, adIds[36]);
#endif
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        
    }

    void ChangePrefabValues()
    {
        GameObject Obj = Selection.activeGameObject;

        if (PrefabUtility.IsPartOfAnyPrefab(Obj))
        {
            GameObject prefabContainer = PrefabUtility.GetOutermostPrefabInstanceRoot(Obj);
            EditorUtility.SetDirty(prefabContainer);
            PrefabUtility.ApplyPrefabInstance(prefabContainer, InteractionMode.AutomatedAction);
        }
    }
    void Resolver(Ads Ads)
    {


        if (Ads.admobAppId == string.Empty)
        {
            EditorUtility.DisplayDialog("Kindly Assign Google App Id", "Admob App Id is Empty.. Kindly Assign New Id", "ok");
            return;
        }
        EditorUtility.DisplayDialog("Please Wait", "Please Wait Untill Android Resolver is Running", "OK");
        PlayServicesResolver.MenuForceResolve();
    }

   
    public void AssignIds(Ads Ads)
    {
        AssigningIds(Ads);
        Clean(Ads);

        GoogleMobileAdsSettings googleAdsSettings = GoogleMobileAdsSettings.LoadInstance();
        
        googleAdsSettings.adMobAndroidAppId = "";
        if (Ads.admobAppId == string.Empty )
            EditorUtility.DisplayDialog("Kindly Chnage Google App Id", "Issue", "Not resolved");
            
        
#if UNITY_ANDROID
        googleAdsSettings.adMobAndroidAppId = Ads.admobAppId;
#elif UNITY_IPHONE
        googleAdsSettings.adMobIOSAppId = Ads.admobAppId;
#endif

 
      int option =   EditorUtility.DisplayDialogComplex("Reminder!", "Please Check Privacy Policy URL", "Resolve", "Cancel", ""  );

        switch(option)
        {
            case 0:
                Resolver(Ads);
                break;
            case 1:
                break;
            default:
                Debug.LogError("Error");
                break;
        }
        ChangePrefabValues();
    }
}
