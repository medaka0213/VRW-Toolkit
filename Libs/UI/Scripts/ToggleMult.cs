
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ToggleSwitchMult : UdonSharpBehaviour
{
    Text Label;
    [SerializeField, TextArea]public string labelText;
    public Color indicatorColor;
    [UdonSynced]public bool isActive = false;
    private Image Indicator;
    public GameObject[] Targets;

    void Start(){
        Indicator = transform.Find("Indicator").GetComponent<Image>();
        Indicator.color = indicatorColor;
        Label = transform.Find("Label").GetComponent<Text>();
        Label.text = labelText;
    }
    void Interact(){
        if(!isOwner()){
            takeOwnership();
        }
        toggle();
    }

    public override void OnPlayerJoined(VRCPlayerApi player){
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "sync");
    }
    public void sync(){
        if(!isOwner()){
            return;
        }
        if(isActive){
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setActive");
        } else {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setInactive");
        }
    }

    public void toggle(){
        if(!isOwner()){
            return;
        }
        if(isActive){
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setInactive");
        } else {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setActive");
        }
    }

    public void setActive(){
        isActive = true;
        Indicator.enabled = isActive;    
        for (int i = 0; i < Targets.Length; i++){
            Targets[i].SetActive(isActive);
        }
    }

    public void setInactive(){
        isActive = false;
        Indicator.enabled = isActive;    
        for (int i = 0; i < Targets.Length; i++){
            Targets[i].SetActive(isActive);
        }
    }

    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
