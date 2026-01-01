using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Paudio : MonoBehaviour
{

    public AudioSource Source;
    public AudioClip Clip;
    Coroutine c;
    public static Paudio instance;

    void Start()
    {
        Source = GetComponent<AudioSource>();
        Source.clip = Clip;
        instance = this;
    }
    private void OnEnable()
    {
     
        Source.volume = 1f;
    }
    void OnMouseDown()
    {
            iTween.ShakePosition(this.transform.gameObject, new Vector3(0.2f, 0f, 0f), 0.6f);
            Source.clip = Clip;
            Source.Play();
        
        //StopAllCoroutines();
    }
     public void PlayClip()
    {
        for (int i=0;i<LevelScript.instance.Alphabets.Length;i++)
        {
            LevelScript.instance.Alphabets[i].transform.GetComponent<AudioSource>().enabled = false;
        }
        Debug.Log(this.transform.name);
        this.transform.GetComponent<AudioSource>().enabled = true;
        Source.Stop();
        iTween.ShakePosition(this.transform.gameObject, new Vector3(0.2f, 0f, 0f), 0.6f);
        Source.clip = Clip;
        Source.Play();
        StartCoroutine(EnableButtons());
        
        
    }


    public IEnumerator EnableButtons()
    {
        yield return new WaitUntil(() => !Source.isPlaying);
        for (int i = 0; i < LevelScript.instance.Alphabets.Length; i++)
        {
            LevelScript.instance.Alphabets[i].transform.GetComponent<AudioSource>().enabled = true;
        }
    }

    /// <summary>
    /// ///////////Play Alll New
    public IEnumerator PlayLoopClips()
    {
        Debug.Log("PlayClipLoop Calls");
        if (!LevelScript.instance.Keep_Playing)
        {
            yield break;
        }
        for (int i = 0; i < LevelScript.instance.LevelAudioClips.Length; i++)
        {
                Debug.Log("i is:"+i);
                for (int j = i + 1; j <= LevelScript.instance.LevelAudioClips.Length; j++)
                {
            //    Debug.Log("Length is:"+LevelScript.instance.LevelAudioClips.Length);
                    for (int k = j; k <= LevelScript.instance.LevelAudioClips.Length; k++)
                    {
                        if (LevelScript.instance.Keep_Playing)
                        {
                            Source.clip = LevelScript.instance.LevelAudioClips[k-1];
                        //    Debug.Log("Shaked at index:"+ k + "is"+ LevelScript.instance.Alphabets[k - 1].transform.gameObject.name);
                            iTween.ShakePosition(LevelScript.instance.Alphabets[k-1].transform.gameObject, new Vector3(0.2f, 0f, 0f), 0.6f);
                        if (LevelScript.instance.Alphabets[k - 1].transform.gameObject.GetComponent<NeetToScroll>())
                        {
                           
                    //        Debug.Log("Scroll It");
                    //        Debug.Log("Scroll panel named:"+LevelScript.instance.Alphabets[k - 1].transform.parent.name);
                    //        Debug.Log("position is:"+LevelScript.instance.Alphabets[k - 1].transform.parent.GetComponent<RectTransform>().localPosition);
                            LevelScript.instance.Alphabets[k - 1].transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(LevelScript.instance.Alphabets[k - 1].transform.parent.GetComponent<RectTransform>().localPosition.x, (LevelScript.instance.Alphabets[k - 1].transform.parent.GetComponent<RectTransform>().localPosition.y + 7.2f), LevelScript.instance.Alphabets[k - 1].transform.parent.GetComponent<RectTransform>().localPosition.z);
                        }
                        Source.Play();
                        yield return new WaitUntil(()=>!Source.isPlaying);
                            if (GmManager.Paused)
                            {
                                Debug.Log("Paused Bool is:" + GmManager.Paused);
                                iTween.ShakePosition(LevelScript.instance.Alphabets[k - 1].transform.gameObject, new Vector3(0f, 0f, 0f), 0f);  
                        yield break;
                            }
                        yield return new WaitForSeconds(0.5f);

                        /*}
                        else
                        {
                            Debug.Log("Paused Bool is:" + GmManager.Paused);

                            yield break;
                        }*/
                    }
                    else
                    {

                        yield break;
                     }

                    break;

                }
            }

             break;
        }
     }
    public IEnumerator StopPreviousAndContinue()
    {
        LevelScript.instance.Keep_Playing = false;
        Debug.Log("Coroutine stopped");
        yield return new WaitForSeconds(2f);
        LevelScript.instance.Keep_Playing = true;
        StartCoroutine(PlayLoopClips());
        Debug.Log("startAgain");
        
    }



}

