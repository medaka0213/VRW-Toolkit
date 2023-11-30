
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class AnimBoolController : UdonSharpBehaviour
{
    [UdonSynced] public bool value;
    public string AnimationValueName = "bool1";
    public GameObject indicator; 
    public Animator animator;

    void Start()
    {
        
    }

    void Update() {
        animator.SetBool(AnimationValueName, value);
        if(indicator){
            indicator.SetActive(value);
        }
    }

    void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RequestSerialization();
        }
    }

    void TakeOwnership()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    void SetAnimationValue (bool _value) {
        TakeOwnership();
        value = _value;
        RequestSerialization();
    }

    public void ToggleAnimationBool (){
        SetAnimationValue( !value);
    }

    public void SetAnimationBoolFalse (){
        SetAnimationValue(false);
    }

    public void SetAnimationBoolTrue (){
        SetAnimationValue(true);
    }
}
