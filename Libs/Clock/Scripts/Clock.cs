using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine;
using UnityEngine.UI;

public class Clock : UdonSharpBehaviour
{
    public bool isSummerTime = false;
    public float utcPlus;
    public float utcPlus_Summer;

    private float descTimer = 0f;
    private int descTimerInterval = 2;

    public string title;
    public string title_Summer;
    
    public string descJP;
    public string descEN;

    public string descJP_Summer;
    public string descEN_Summer;

    Text mainField;
    Text titleField;
    Text subtitleDescFieldEN;
    Text subtitleDescFieldJP;
    Text subtitleDiffField;
    Image bgImage;

    //画面の色
    public Color color_title = new Color(1f, 1f, 1f, 1f);
    public Color color_bg = new Color(0.1f, 0.1f, 0.1f, 1f);

    void Start()
    {
        mainField = transform.Find("Canvas/Main").GetComponent<Text>();
        titleField = transform.Find("Canvas/Title").GetComponent<Text>();
        subtitleDescFieldJP = transform.Find("Canvas/DescriptionJP").GetComponent<Text>();
        subtitleDescFieldEN = transform.Find("Canvas/DescriptionEN").GetComponent<Text>();
        subtitleDiffField = transform.Find("Canvas/UtcPlus").GetComponent<Text>();
        bgImage = transform.Find("Canvas/Background").GetComponent<Image>();

        if (isSummerTime){
            titleField.text = title_Summer;
            subtitleDescFieldJP.text = descJP_Summer;
            subtitleDescFieldEN.text = descEN_Summer;
        }
        else{
            titleField.text = title;
            subtitleDescFieldJP.text = descJP;
            subtitleDescFieldEN.text = descEN;
        }

        float utcPlus_ = isSummerTime ? utcPlus_Summer : utcPlus;
        if (utcPlus_ > 0f){
            subtitleDiffField.text = $"UTC+{utcPlus_}";
        } else {
            subtitleDiffField.text = $"UTC{utcPlus_}";
        }

        //色設定
        titleField.color = color_title;
        subtitleDescFieldJP.color = color_title;
        subtitleDescFieldEN.color = color_title;
        subtitleDiffField.color = color_title;
        mainField.color = color_title;
        bgImage.color = color_bg;
    }

    void Update(){
        UpdateTime();
        UpdateDescTimer();
    }

    void UpdateTime(){
        var now = DateTime.UtcNow;
        now += new TimeSpan((int)utcPlus, (int)(utcPlus%1 * 60f), 0);
        mainField.text = now.ToString("HH:mm:ss");
    }

    void UpdateDescTimer(){
        var now = DateTime.UtcNow;
        if(now.Second % (descTimerInterval * 2) >= descTimerInterval ){
            subtitleDescFieldEN.enabled = true;
            subtitleDescFieldJP.enabled = false;
        } else {
            subtitleDescFieldEN.enabled = false;
            subtitleDescFieldJP.enabled = true;
        }
    }
}
