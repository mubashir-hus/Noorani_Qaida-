using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LanaguageSelectionPanel : MonoBehaviour
{
    public Text LanguageValue;
    // Start is called before the first frame update

   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Language(Text Language)
    {

        //Debug.Log(LanguageValue);
        PlayerPrefs.SetString("Language", Language.text);
        if (Language.text == "ENGLISH")
        {
            Debug.Log(Language.text + "is selected");

        }
        if (Language.text == "URDU")
        {
            Debug.Log(Language.text + "is selected");

        }

    }
}
