
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.Udon.Common.Interfaces;

using UdonToolkit;

//[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class InputFieldTrigger : UdonSharpBehaviour
{
    InputField inputField;

    public bool setUdonVariable = true;
    public bool fireUdonEvents = true;

    [FoldoutGroup("Udon Target")]
    public bool networked;
    [FoldoutGroup("Udon Target")] [HideIf("@!networked")]
    public NetworkEventTarget networkTarget;

    [FoldoutGroup("Udon Target")] [ListView("Udon Target List")]
    public UdonSharpBehaviour[] udonTargets;

    [FoldoutGroup("Udon Target")] [ListView("Udon Target List")] [HideIf("@!setUdonVariable")]
    public string[] udonVariables;

    [FoldoutGroup("Udon Target")] [ListView("Udon Target List")] [Popup("behaviour", "@udonTargets", true)] [HideIf("@!fireUdonEvents")]
    public string[] udonEvents;


    void Start()
    {
        inputField = gameObject.GetComponent<InputField>();
    }

    public void SetText()
    {
        for (int i = 0; i < udonTargets.Length; i++){
            if (udonVariables[i] != ""){
                udonTargets[i].SetProgramVariable(udonVariables[i], inputField.text);
            }
        }
    }

    public void Trigger(){
        if (setUdonVariable){
            if (networked){
                SendCustomNetworkEvent(networkTarget, "SetText");
            } else {
                SetText();
            }
        }
        if (fireUdonEvents){
            if (networked){
                for (int i = 0; i < udonTargets.Length; i++){
                    if (udonEvents[i] != ""){
                        udonTargets[i].SendCustomNetworkEvent(networkTarget, udonEvents[i]);
                    }
                }
            } else {
                for (int i = 0; i < udonTargets.Length; i++){
                    if (udonEvents[i] != ""){
                        udonTargets[i].SendCustomEvent(udonEvents[i]);
                    }
                }
            }
        }
        inputField.text = "";
    }
}
