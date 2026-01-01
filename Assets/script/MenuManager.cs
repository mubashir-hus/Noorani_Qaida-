using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public static int LevelNumber;
    // Start is called before the first frame update
    public Text Name;
    public Text Country;
    public static MenuManager instance;
    public GameObject LoginPanel;
    public GameObject LevelSelection;
    public Sprite ToChange;
    private Sprite Temp;
    public GameObject QuitPannel;
    public GameObject RateUSPannel;
    public Text countryName;
    public Image Imageload;
    public GameObject loadingpanel;
    private string Date,time,year,month;
    public static bool BismillahComplete;
    public GameObject UpdateVersionPannel;
    void Start()
    {
      


    }
    private void OnEnable()
    {
        instance = this;
        if (PlayerPrefs.GetInt("NameSet") == 1)
        {
            LevelSelection.SetActive(true);
            LoginPanel.SetActive(false);
        }
        else
        {
            LoginPanel.SetActive(true);
            LevelSelection.SetActive(false);
        }
        Ads.Instance.RequestBannerAdMainMenu();

    }
    // Update is called once per frame

    public void SetLevelToLoad(int i)
    {
        LevelNumber = i;
        SceneManager.LoadScene("SampleScene");

    }
    public void ShareSomething()
    {

    }
    public void Quit()
    {
       
        QuitPannel.SetActive(true);
    }
    public void RateUs()
    {
        RateUSPannel.SetActive(true);
    }
    public void RateusClose()
    {
        RateUSPannel.SetActive(false);
    }
    public void KeepOnGame()
    {
        //  Debug.LogError("Keep  on game calls");
        QuitPannel.SetActive(false);
    }
    public void QuitApp()
    {
        Application.Quit();
    }
    public void RateUsOpen()
    {

        // Google Play APP Link
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.TheBrainStormers.NoraniQuaida");

        // Huawei APP Link
       // Application.OpenURL("https://appgallery.huawei.com/app/C105220025");


        // IOS APP Link
     // Application.OpenURL("https://apps.apple.com/us/app/noorani-qaida-by-e-school/id1596916293");


    }

    public void submit()
    {
        // Debug.Log("Selected Language is:"+PlayerPrefs.GetString("Language"));
        if (Name.text != null && Country.text != "SELECT" && Name.text != "" && PlayerPrefs.GetString("Language") != "")
        {
           
            Debug.Log("you're good to go");
            PlayerPrefs.SetString("Name", Name.text);
            StartCoroutine(sendData());
            LoginPanel.SetActive(false);
            LevelSelection.SetActive(true);

            PlayerPrefs.SetInt("NameSet", 1);
        }
        else
        {
            print("Check it again");
        }
    }

    public IEnumerator sendData()
    {

        WWWForm form = new WWWForm();
        form.AddField("username", Name.text);
        form.AddField("country_code", Country.text);
 
       //New API That is not working=(https://eschoolforall.com/api/student/al-quran-login)

        UnityWebRequest www = UnityWebRequest.Post(" https://eschoolforall.com/api/student/al-quran-register", form);
        yield return www.SendWebRequest();

        Debug.LogWarning(System.DateTime.Now.ToString("yyyy/MMM/dd HH:mm:ss"));
        Date = System.DateTime.Now.ToString("dd").ToString();
        month = System.DateTime.Now.ToString("MM").ToString();
        year  = System.DateTime.Now.ToString("yyyy").ToString();
        time = System.DateTime.Now.ToString("HH:mm:ss").ToString();


        Debug.LogWarning("Date is:"+Date+",Month:"+month+",year:"+year+"time:"+time);

        PlayerPrefs.SetInt("DataEnter", 1);

    }
    public void Share()
    {
        Debug.Log("Share App");
    }

    public void ChangeImage(Image WithChange)
    {
        Temp = WithChange.sprite;
        WithChange.sprite = ToChange;
        // Temp = Yes.GetComponent<Image>().sprite;
        //  Yes.GetComponent<Image>().sprite = ToChange;
    }
    public void ChangeImageUp(Image WithChange)
    {


        //  Temp = WithChange;
        WithChange.sprite = Temp;
        // Temp = Yes.GetComponent<Image>().sprite;
        //  Yes.GetComponent<Image>().sprite = ToChange;

    }

    public void ChangeImageWith(Sprite WithChange)
    {
        //Yes.GetComponent<Image>().sprite = Temp;
    }
    public void ToUpperCase()
    {
        countryName.ToString().ToUpper();
    }


    public IEnumerator RotatingShing()
    {

        loadingpanel.SetActive(true);
     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("SampleScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
          Imageload.transform.Rotate(new Vector3(0f, 0f, -3f ));
        }
    }
    public void Loading(int i)
    {
       
        LevelNumber = i;
        StartCoroutine(RotatingShing());
       
    }
  
}