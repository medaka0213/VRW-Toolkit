
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AnimController : UdonSharpBehaviour
{
    [UdonSynced] public int mode = 0;
    public int maxMode = 9;
    public string AnimationIntName = "mode";
    public Animator animator;
    public Transform pickup;

    GameObject showInPickup;

    void Start()
    {
        if (!pickup) {
            pickup = transform;
        }
        
        if (pickup.Find("ShowInPickUp")){
            showInPickup = pickup.Find("ShowInPickUp").gameObject;
            showInPickup.SetActive(false);
        }
    }

    void Update() {
        if (animator){
            animator.SetInteger(AnimationIntName, mode);
        }
    }

    void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RequestSerialization();
        }
    }

    public void ToggleShowInPickup () {
        if (!showInPickup) return;
        showInPickup.SetActive(!showInPickup.activeSelf);
    }

    void OnPickup()
    {
        ToggleShowInPickup();
    }

    void TakeOwnership()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    void SetAnimationInt (int inputInt) {
        TakeOwnership();
        mode = inputInt;
        if (mode > maxMode){
            mode = 0;
        }
        RequestSerialization();
    }

    public void InclAnimationInt(){
        SetAnimationInt (mode + 1);
    }

    public void OnPickupUseUp(){
        InclAnimationInt ();
    }

    public void Set0(){
        SetAnimationInt (0);
    }
    public void Set1(){
        SetAnimationInt (1);
    }

    public void Set2(){
        SetAnimationInt (2);
    }

    public void Set3(){
        SetAnimationInt (3);
    }

    public void Set4(){
        SetAnimationInt (4);
    }

    public void Set5(){
        SetAnimationInt(5);
    }

    public void Set6 (){
        SetAnimationInt(6);
    }

    public void Set7 (){
        SetAnimationInt(7);
    }

    public void Set8 (){
        SetAnimationInt(8);
    }

    public void Set9 (){
        SetAnimationInt(9);
    }

    public void Set10 (){
        SetAnimationInt(10);
    }

    public void Set11 (){
        SetAnimationInt(11);
    }

    public void Set12 (){
        SetAnimationInt(12);
    }

    public void Set13 (){
        SetAnimationInt(13);
    }

    public void Set14 (){
        SetAnimationInt(14);
    }

    public void Set15 (){
        SetAnimationInt(15);
    }

    public void Set16 (){
        SetAnimationInt(16);
    }

    public void Set17 (){
        SetAnimationInt(17);
    }

    public void Set18 (){
        SetAnimationInt(18);
    }

    public void Set19 (){
        SetAnimationInt(19);
    }

    public void Set20 (){
        SetAnimationInt(20);
    }
}
