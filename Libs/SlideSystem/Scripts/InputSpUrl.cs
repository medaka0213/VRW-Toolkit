
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InputSpUrl : UdonSharpBehaviour
{
    
    public VRCUrlInputField inputField;
    public VRCUrl url;

    public UdonSharpBehaviour targetUdon;
    public string targetVariableName;

    public void OnSetUrl(){
        targetUdon.SetProgramVariable(targetVariableName, inputField.GetUrl());
        inputField.SetUrl(VRCUrl.Empty);
    }
}
