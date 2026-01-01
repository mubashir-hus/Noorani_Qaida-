using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GmManager : MonoBehaviour
{
    GmManager instance;
   // public GameObject PausePanel;
    public GameObject[] Levels;
    public static bool Paused;
    int a;
    public Sprite PlayImg;
    public Sprite Pause;
    public Button PlayP;
    private AudioSource bismillahaudiosource;
    public AudioClip bismillah;
    public GameObject bismillahpanel;
    public GameObject Bismillah;
    public GameObject[] MyPrefebs;
    public Transform Canvas;
    public Transform InsPos;
    public GameObject LoadingPanel;
    // Start is called before the first frame update
    private void OnEnable()
    {
        bismillahaudiosource = GetComponent<AudioSource>();
        bismillahaudiosource.clip = bismillah;
        bismillahpanel.SetActive(true);
        StartCoroutine(ReciteBissMilllah());
        GameObject Level;
        Level = Instantiate(MyPrefebs[MenuManager.LevelNumber], InsPos.position, Quaternion.identity,Canvas.transform);
        Level.transform.SetSiblingIndex(1);
        Ads.Instance.RequestBannerAdGamePlay();
        Time.timeScale = 1;
        
    }
    void Start()
    {

        /* for (int i=0;i<Levels.Length;i++)
         {
             Levels[i].SetActive(false);
         }
         Levels[MenuManager.LevelNumber].SetActive(true);
         instance = this;
         Debug.Log(Levels[MenuManager.LevelNumber].transform.name);
        */
        instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            SceneManager.LoadScene("Menu");
        }
    }
   public void Resume()
    {

    }
    public void RayCastOff()
    {
        for (int i = 0; i < LevelScript.instance.Alphabets.Length; i++)
        {
            LevelScript.instance.Alphabets[i].GetComponent<BoxCollider2D>().enabled = false;
        }
        StartCoroutine(Stoptime());
    }
    public void RayCastOn()
    {
        for (int i = 0; i < LevelScript.instance.Alphabets.Length; i++)
        {
            LevelScript.instance.Alphabets[i].GetComponent<BoxCollider2D>().enabled = true;
        }
     //   PausePanel.SetActive(false);
    }
    public IEnumerator Stoptime()
    {
        Paused = true;
        yield return new WaitForSeconds(0.2f);
        
        Time.timeScale = 0f;
    }
    public void MainMenu()
    {
          if (Ads.Instance != null)
        {
            Ads.Instance.ShowInterstitial();

        }
        LoadingPanel.SetActive(true);
        StartCoroutine(WaitLoading());
        
    }

    IEnumerator WaitLoading()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Menu");
    }

    public void CheckAudioPlay()
    {
      
            if(PlayP.GetComponent<Image>().sprite.name == Pause.name)
        {
            PlayP.GetComponent<Image>().sprite = PlayImg;
        }
        else
        {
            PlayP.GetComponent<Image>().sprite = Pause;
        }

        // StartCoroutine(Paudio.instance.PlayLoopClips());
        if (LevelScript.instance.Keep_Playing)
        {
            Debug.Log("Keep Playing ");
            LevelScript.instance.Keep_Playing = false;
            

        }
        else
        {
           
            LevelScript.instance.Keep_Playing = true;
            StartCoroutine(Paudio.instance.PlayLoopClips());
            

        }
        if (Levels[MenuManager.LevelNumber].transform.GetChild(0).GetComponent<ScrollRect>() !=null)
        {
            Debug.Log("Enter");
            if (Levels[MenuManager.LevelNumber].transform.GetChild(0).GetComponent<ScrollRect>().enabled == true)
            {

                Levels[MenuManager.LevelNumber].transform.GetChild(0).GetComponent<ScrollRect>().enabled = false;
            }
            else
            {
                Levels[MenuManager.LevelNumber].transform.GetChild(0).GetComponent<ScrollRect>().enabled = true;
            }
        }
        else
        {
            Debug.Log("Not Found Scroll Rect");
            Debug.Log("Name is:"+Levels[MenuManager.LevelNumber].transform.GetChild(0).name);
        }

    }

    public IEnumerator ReciteBissMilllah()
    {
        bismillahaudiosource.Play();

        do
        {
            float imageadd = bismillahaudiosource.time / bismillahaudiosource.clip.length;
            yield return null;
            // Debug.Log("Placing Voice time:" + a.time);
            Bismillah.transform.GetComponent<Image>().fillAmount += (imageadd / 50);
         //   Debug.Log(bismillahaudiosource.time);

        } while (Bismillah.transform.GetComponent<Image>().fillAmount < 1f);
        Invoke("SetActiveFalse", bismillahaudiosource.clip.length);
    }
    public void SetActiveFalse()
    {
        bismillahpanel.SetActive(false);
        Debug.Log("hide panel");

    }
    
}
