
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class GuppySpeakerLimitter : UdonSharpBehaviour
{
    [UdonSynced] public bool enableReadLog = false;
    public string ownerName;

    public WorldSetting worldSetting;
    public string[] vrchatNames = { "medaka0228", "guppy0228" };

    public float updateInterval = 0.5f;
    private float updateTimer = 0f;

    public GameObject indicator;

    public bool isGuppyLocal()
    {
        var player = Networking.LocalPlayer;
        if (player == null) return false;

        if (player.IsUserInVR())
        {
            return false;
        }

        foreach (string i in vrchatNames)
        {
            if (player.displayName == i)
            {
                return true;
            }
        }
        return false;
    }

    public VRCPlayerApi GuppyPlayer()
    {
        var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);
        foreach (VRCPlayerApi player in players)
        {
            foreach (string i in vrchatNames)
            {
                if (player.displayName == i)
                {
                    return player;
                }
            }
        }
        return null;
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval)
        {
            updateTimer = 0f;
            Debug.Log("update");
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate()
    {
        if (isGuppyLocal())
        {
            worldSetting.setVoice = false;
            var players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(players);
            foreach (VRCPlayerApi player in players)
            {
                SetSpeaker(player);
            }
        }

        var owner = Networking.GetOwner(gameObject);
        if (owner != null)
        {
            ownerName = owner.displayName;
        }

        if (indicator != null)
        {
            indicator.SetActive(enableReadLog);
        }
    }

    public void SetSpeaker(VRCPlayerApi player)
    {
        if (IsOwner(player) && enableReadLog)
        {
            player.SetVoiceVolumetricRadius(worldSetting.voiceVolumetricRadius);
            player.SetVoiceDistanceFar(worldSetting.voiceDistanceFar);
            player.SetVoiceDistanceNear(worldSetting.voiceDistanceNear);
        }
        else
        {
            player.SetVoiceVolumetricRadius(0.0f);
            player.SetVoiceDistanceFar(0.0f);
            player.SetVoiceDistanceNear(0.0f);
        }
    }
    public void TakeOwnership()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
    /*
    public void ReleaseOwnership(){
        var player = GuppyPlayer();
        if (player != null){
            Networking.SetOwner(player, gameObject);
        }
    }
    */
    public bool IsOwner(VRCPlayerApi player)
    {
        return Networking.GetOwner(gameObject) == player;
    }

    public void SetSpeakerButton()
    {
        TakeOwnership();
        enableReadLog = true;
    }
    public void ReleaseSpeakerButton()
    {
        enableReadLog = false;
    }

    public void OnPickupUseUp()
    {
        ReleaseSpeakerButton();
    }

    public void OnPickupUseDown()
    {
        SetSpeakerButton();
    }
}
