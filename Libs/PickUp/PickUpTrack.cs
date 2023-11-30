using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PickUpTrack : UdonSharpBehaviour
{
    public GameObject dialogTracked;
    public GameObject dialogUntracked;
    public bool isTracked=false;
    public bool isHeld = false;

    private Vector3 _deltaPositionLocal;
    private Quaternion _deltaRotationLocal;
    private Quaternion _attachRotationLocal;

    void Update(){
        if (dialogTracked){
            dialogTracked.SetActive(isTracked);
        }

        if (dialogUntracked){
            dialogUntracked.SetActive(!isTracked);
        }

        updateTransform();
    }

    public void OnPickup(){
        isHeld = true;
    }

    public void OnPickupUseUp(){
        Toggle();
    }

    public void OnDrop(){
        isHeld = false;
    }

    public void Toggle(){
        isTracked = !isTracked;
    }

    void updateTransform(){
        var p = Networking.LocalPlayer;
        if (p==null) return;

        var _targetBone = HumanBodyBones.Hips;
        var pos = p.GetPosition();
        var rot = p.GetRotation();

        if (isTracked && !isHeld){
            transform.position = pos + rot * _deltaRotationLocal * _deltaPositionLocal;
            transform.rotation = rot * _deltaRotationLocal * _attachRotationLocal;

        } else {
            _deltaPositionLocal = transform.position - pos;
            _deltaRotationLocal = Quaternion.Inverse(rot);
            _attachRotationLocal = transform.rotation;
        }
    }
}
