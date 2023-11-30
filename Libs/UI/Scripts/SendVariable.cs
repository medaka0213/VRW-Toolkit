
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SendVariable : UdonSharpBehaviour
{
    public Slider slider;
    public UdonSharpBehaviour target;
    public string targetVariableName;
    public bool onwerOnly = false;
    
    [UdonSynced] public float syncValue; 

    void Start()
    {
        syncValue = slider.value;
    }

    void Update(){
        target.SetProgramVariable(targetVariableName, syncValue);
    }

    public void SetValue(){
        if (!onwerOnly){
            takeOwnership();
        }
        if (isOwner() || !onwerOnly){
            syncValue = slider.value;
            RequestSerialization();
        }
    }

    //オーナー判定
    public bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    void OnPlayerJoined(){
        if (isOwner()){
            RequestSerialization();
        }
    }
}
