
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonToolkit;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class WorldSetting : UdonSharpBehaviour
{
    
    [Header("[ ボイス設定 ]")]
    public bool setVoice = false;
    [HideIf("@!setVoice")]
    public float voiceDistanceNear = 0.0f;
    [HideIf("@!setVoice")]
    public float voiceDistanceFar = 25.0f;
    [HideIf("@!setVoice")]
    public float voiceVolumetricRadius = 0.0f;

    [Header("[ スピード設定 ]")]
    public bool setSpeed = true;
    [HideIf("@!setSpeed")]
    public float jumpImpulse = 3.0f;
    [HideIf("@!setSpeed")]
    public float walkSpeed = 2.0f;
    [HideIf("@!setSpeed")]
    public float runSpeed = 4.0f;
    [HideIf("@!setSpeed")]
    public float strafeSpeed = 2.0f;

    [Header("[ 定期実行する ]")]
    public bool isUpdate = false;
    [HideIf("@!isUpdate")]
    public float updateInterval = 0.5f;
    private float updateTimer = 0f;

    void Start()
    {
        
    }

    void Update(){
        if(isUpdate){
            updateTimer += Time.deltaTime;
            if (updateTimer > updateInterval){
                updateTimer = 0f;
                Debug.Log("update");
                ManagedFixedUpdate();
            }
        }
    }

    void ManagedFixedUpdate(){
        UpdatePlayerStatus(Networking.LocalPlayer);
    }

    void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal){
            UpdatePlayerStatus(player);
        }
    }

    public void UpdatePlayerStatus(VRCPlayerApi player){
        if (setVoice)
        {
            player.SetVoiceDistanceNear(voiceDistanceNear);
            player.SetVoiceDistanceFar(voiceDistanceFar);
            player.SetVoiceVolumetricRadius(voiceVolumetricRadius);
        }
        if (setSpeed)
        {
            player.SetJumpImpulse(jumpImpulse);
            player.SetWalkSpeed(walkSpeed);
            player.SetRunSpeed(runSpeed);
            player.SetStrafeSpeed(strafeSpeed);
        }
    }
}
