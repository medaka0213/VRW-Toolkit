
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class MultilingualText : UdonSharpBehaviour
{
    public string language="Japanese"; //Japanese, English
    public LangControl langControl;
    public Text target;
    [SerializeField, TextArea]public string JapaneseText ="";
    [SerializeField, TextArea]public string EnglishText ="";

    void Start()
    {
        SetText();
    }

    void Update(){
        SetText();
    }

    public void SetText(){
        if (langControl){
            language = langControl.language;
        }
        
        if (language == "Japanese" && JapaneseText != ""){
            target.text = JapaneseText;
        }
        else if (language=="English" && EnglishText != ""){
            target.text = EnglishText;
        }
    }
}
