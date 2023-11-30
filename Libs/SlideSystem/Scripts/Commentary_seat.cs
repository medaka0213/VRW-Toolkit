
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Commentary_seat : UdonSharpBehaviour
{
    public string vrchatNameProd = "guppy0228";
    public string vrchatNameDebug = "medaka0228";
    public bool isDebug = false;

    string vrchatName;
    
    void Start()
    {
        if (isDebug)
        {
            vrchatName = vrchatNameDebug;
        }
        else
        {
            vrchatName = vrchatNameProd;
        }
    }

    void Update(){
        var player = Networking.LocalPlayer;
        if (player==null) return;
        if (player.displayName == vrchatName){
            player.TeleportTo(transform.position, transform.rotation);
        }
    }
}
