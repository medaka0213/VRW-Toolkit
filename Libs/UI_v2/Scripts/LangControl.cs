
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LangControl : UdonSharpBehaviour
{
    public string language="Japanese"; //Japanese, English

    void Start()
    {
        
    }

    public void SetJapanese(){
        language="Japanese";
    }

    public void SetEnglish(){
        language="English";
    }

    public void Toggle(){
        if (language=="English"){
            SetJapanese();
        } else {
            SetEnglish();
        }
    }
}
