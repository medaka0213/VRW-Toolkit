
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using UdonToolkit;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class InputUrl : UdonSharpBehaviour
{
    public bool useInputField = true;

    [HideIf("@!useInputField")]
    public VRCUrlInputField inputField;
    [HideIf("@useInputField")]
    public VRCUrl URL;

    public UdonSharpBehaviour targetUdon;
    public string targetVariableName;

    public void OnSetUrl(){
        if (useInputField) {
            targetUdon.SetProgramVariable(targetVariableName, inputField.GetUrl());
            inputField.SetUrl(VRCUrl.Empty);
        } else {
            targetUdon.SetProgramVariable(targetVariableName, URL);
        }
    }
}
