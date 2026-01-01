using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class MenuPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] Urdu;
    public GameObject[] English;
    GetVersionFromApi GetDataBaseVersion;
    string DeviceVersion;
    public static MenuPanel instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
       

    }
    private void OnEnable()
    {
        DeviceVersion = Application.version;

      //  StartCoroutine(Get_App_Version());


        PlayerPrefs.SetInt("BismillahAlreadyplayed", 1);
        /* }
         else
         {
             MenuManager.instance.bismillahpanel.SetActive(false);

         }

         Debug.Log("COUNTRY IS" + MenuManager.instance.Country.text);
     */
        ChangeLanguage();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ChangeLanguage()
    {
        Debug.Log("Change value for language : " + PlayerPrefs.GetString("Language"));
        if (PlayerPrefs.GetString("Language") == "URDU")
        {
            Debug.Log("Set values for urdu");
            for (int i = 0; i < Urdu.Length; i++)
            {
                Urdu[i].SetActive(true);
            }
        }
        ////
        if (PlayerPrefs.GetString("Language") == "ENGLISH")
        {
            Debug.Log("Set values for english");
            for (int i = 0; i < English.Length; i++)
            {
                English[i].SetActive(true);
            }

        }
    }
    public IEnumerator Get_App_Version()
    {
        Debug.Log("Calls");
        WWWForm form = new WWWForm();
        UnityWebRequest www = UnityWebRequest.Post("https://eschoolforall.com/api/app/version", form);
        www.downloadHandler = new DownloadHandlerBuffer();
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            yield return www.SendWebRequest();
            Debug.Log(www.downloadHandler.text);
            if (www.isDone)
            {
                GetDataBaseVersion = JsonConvert.DeserializeObject<GetVersionFromApi>(www.downloadHandler.text);


                Debug.Log("Version in Data Base Is" + GetDataBaseVersion.data.alquran_app_version);
                PlayerPrefs.SetString("ApiVersionForGoogle", GetDataBaseVersion.data.alquran_app_version);

                Debug.Log("Current App version" + DeviceVersion);
                Debug.Log("Current App version" + GetDataBaseVersion.data.eschool_app_huawei_version);

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
                
            }
        }

    }
}
