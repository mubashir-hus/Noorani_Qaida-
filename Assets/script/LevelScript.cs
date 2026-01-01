using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    public static LevelScript instance;
    public AudioClip[] LevelAudioClips;
    public bool Keep_Playing;
    public bool AlreadyPlaying;
    public GameObject[] Alphabets;
    public Image ScrollAbleImage;
    public GameObject SabaqTitleUrdu;
    public GameObject SabaqTitleEnglish;
    private RectTransform This;
    public CheckLession SetLesson;
    public bool ChangePos;

    void Start()
    {
        Keep_Playing = false;
        instance = this;

    }
    private void OnEnable()
    {
        Debug.Log("Lesson Number:" + SetLesson);
        if (PlayerPrefs.GetString("Language") == "URDU")
        {

            SabaqTitleUrdu.SetActive(true);

        }
        if (PlayerPrefs.GetString("Language") == "ENGLISH")
        {

            SabaqTitleEnglish.SetActive(true);

        }
        Debug.Log(((int)SetLesson));
        This = GetComponent<RectTransform>();
        if ((int)SetLesson == 3)
        {
            This.localPosition = new Vector2(326f, -281f);
        }
        if ((int)SetLesson == 2 || (int)SetLesson == 4 || (int)SetLesson == 5 || (int)SetLesson == 6 || (int)SetLesson == 8 || (int)SetLesson == 9 || (int)SetLesson == 10 || (int)SetLesson == 11 || (int)SetLesson == 12 || (int)SetLesson == 13 || (int)SetLesson == 17)
        {
            This.localPosition = new Vector2(0f, -211f);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public enum CheckLession
    {
        lesson_one = 1,
        lesson_two = 2,
        lesson_three = 3,
        lesson_four = 4,
        lesson_five = 5,
        lesson_six = 6,
        lesson_seven = 7,
        lesson_eight = 8,
        lesson_nine = 9,
        lesson_ten = 10,
        lesson_eleven = 11,
        lesson_tewlve = 12,
        lesson_thirteen = 13,
        lesson_seventeen = 14,
        lesson_eighteen = 15,
        lesson_ninteen = 16,
        lesson_twenty = 17,
    }

}
