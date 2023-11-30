
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class ToggleSwitch : UdonSharpBehaviour
{
    public GameObject Target;
    public bool isActive = false;
    public bool global = true;
    private GameObject Indicator;

    public float updateInterval = 0.5f; 
    private float updateTimer = 0f;

    void Start(){
        Indicator = transform.Find("Indicator").gameObject;
    }
    void Interact(){
        takeOwnership();
        toggle();
    }

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        SetActiveLocal(isActive);
    }

    public override void OnPlayerJoined(VRCPlayerApi player){
        if (global) {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "sync");
        } else {
            sync();
        }
    }

    public void toggle(){
        if (global) {
            takeOwnership();
            SetActiveGlobal(!isActive);
        } else {
            SetActiveLocal(!isActive);
        }
    }
    
    public void sync(){
        if(!isOwner()){
            return;
        }
        if (global) {
            SetActiveGlobal(isActive);
        } else {
            SetActiveLocal(isActive);
        }
    }

    public void setActive(){
        isActive = true;
        Target.SetActive(true);
        Indicator.SetActive(true);
    }

    public void setInactive(){
        isActive = false;
        Target.SetActive(false);
        Indicator.SetActive(false);    
    }

    public void SetActiveLocal(bool active){
        if (active){
            setActive();
        } else {
            setInactive();
        }
    }
    public void SetActiveGlobal(bool active){
        if (active){
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setActive");
        } else {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setInactive");
        }
    }


    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
