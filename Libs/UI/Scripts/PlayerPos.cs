
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerPos : UdonSharpBehaviour
{
    public float updateInterval = 0.1f; 
    private float updateTimer = 0f;

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        var player = Networking.LocalPlayer;
        if (player != null){
            var trackData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            this.transform.position = trackData.position;
        }
    }
}
