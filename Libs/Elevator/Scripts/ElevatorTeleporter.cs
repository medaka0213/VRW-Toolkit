
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ElevatorTeleporter : UdonSharpBehaviour
{
    public Transform teleportTarget;
    public bool keepPlayerRotation;
    public bool keepPlayerPosition;
    public AnimController animController;

    void Start()
    {
    }

    public void Trigger()
    {
        var player = Networking.LocalPlayer;
        Vector3 pos = teleportTarget.position;
        Quaternion rot = teleportTarget.rotation;
        if (keepPlayerPosition)
        {
            pos = player.GetPosition();
        }
        if (keepPlayerRotation)
        {
            rot = player.GetRotation();
        }
        player.TeleportTo(pos, rot);
        SetClose();
    }

    public void SetOpen(){
        if (animController)
        {
            animController.SendCustomEvent("Set1");
        }
    }

    public void SetClose(){
        if (animController)
        {
            animController.SendCustomEvent("Set0");
        }
    }
}
