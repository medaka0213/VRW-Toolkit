
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class GetDistanceFromMe : UdonSharpBehaviour
{
    public LookAtPlayercs lookAtPlayer;
    Transform target;
    public bool isKm=true;

    void Start()
    {
        target = lookAtPlayer.target;
    }

    void Update(){
        float distance = 0f;
        if (target){
            Vector3 delltaPos = target.position - this.transform.position;
            distance = delltaPos.magnitude;
        } else {
            var player = Networking.LocalPlayer;
            if (player != null){
                var trackData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                Vector3 delltaPos = trackData.position - this.transform.position;
                distance = delltaPos.magnitude;
            }
        }

        if (isKm){
            distance /= 1000f;
            lookAtPlayer.SetProgramVariable("descText", string.Format("{0:F1} km",distance));
        } else {
            lookAtPlayer.SetProgramVariable("descText", string.Format("{0:F1} m",distance));
        }

    }
}
