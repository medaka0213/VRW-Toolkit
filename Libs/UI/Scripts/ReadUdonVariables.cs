using System;
using UnityEngine;
using UnityEngine.UI;

using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ReadUdonVariables : UdonSharpBehaviour
{
    public UdonBehaviour targetUdon;
    public string variableName;
    public string format;
    public Text targetText;

    public float updateTimerDuration = .1f;
    float updateTimer;


    void Start()
    {
        updateTimer = updateTimerDuration;
        if (targetText){
            targetText = gameObject.GetComponent<Text>();
        }
    }

    void Update(){
        updateTimer -= Time.deltaTime;
        
        if (updateTimer < 0f){
            updateTimer = updateTimerDuration;
            UpdateText();
        }
    }

    public void UpdateText(){
        targetText.text = string.Format(format, targetUdon.GetProgramVariable(variableName));
    }
}
