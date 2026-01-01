using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class BgImage : MonoBehaviour
{
    GetVersionFromApi GetDataBaseVersion;
    string DeviceVersion;
    // Start is called before the first frame update
    void Start()
    {
   //    StartCoroutine(Get_App());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator Get_App()
    {
        Debug.Log("Calls");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post("https://eschoolforall.com/api/app/version", form);
        www.downloadHandler = new DownloadHandlerBuffer();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            yield return www.SendWebRequest();
            /*try
            {*/
            Debug.Log(www.downloadHandler.text);
            if (www.isDone)
            {
                GetDataBaseVersion = JsonConvert.DeserializeObject<GetVersionFromApi>(www.downloadHandler.text);


               // Debug.Log("Version in Data Base Is" + GetDataBaseVersion.data.alquran_app_version);
                PlayerPrefs.SetString("ApiVersionForGoogle", GetDataBaseVersion.data.alquran_app_version);

                Debug.Log("Current App version" + Application.version);

               // Debug.Log("Current App version" + GetDataBaseVersion.data.alquran_app_version);
                Debug.Log("Google Current App version" + GetDataBaseVersion.data.alquran_app_ios_version);

                // if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {

                    if (Application.version == GetDataBaseVersion.data.alquran_app_huawei_version)
                    {

                    }
                    else
                    {
                        MenuManager.instance.UpdateVersionPannel.SetActive(true);
                    }
                }
            }
            else
            {
                //  NetCheck = true;
                //    Toolbox.Instance.gameManager.InitDialogueBox("Connectivity issue. Please connect with internet to Continue");
            }
        }

    }
}
