using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using HoshinoLabs.IwaSync3.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class GuppyKeyCtrl : UdonSharpBehaviour
{
    public UnityVideoPlayer slideSystem;
    public string[] vrchatNames = { "medaka0228", "guppy0228" };

    //URl入力 関連
    public GuppyUrlInput urlInput;

    public UnityVideoPlayer[] reloadTargets;

    public void SetSlideIndex(int index)
    {
        slideSystem.TakeOwnership();
        slideSystem.SetProgramVariable("slideIndex", index);
    }

    public void SetAllSlides()
    {
        slideSystem.SetMainUrl();
    }

    public void SetShortSlides()
    {
        slideSystem.SetSubUrl();
    }

    public void SyncSlideToAll()
    {
        SetSlideIndex(0);
        slideSystem.SyncToAll();
        for (int i = 0; i < reloadTargets.Length; i++)
        {
            reloadTargets[i].SyncToAll();
        }
    }

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

    void Update()
    {
        if (isGuppyLocal())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SyncSlideToAll();
            }
            // L: ライブURLの入力
            else if (Input.GetKeyDown(KeyCode.L))
            {
                urlInput.OpenInputField();
                urlInput.mode = "live";
            }
            // O: ビデオURLの入力
            else if (Input.GetKeyDown(KeyCode.O))
            {
                urlInput.OpenInputField();
                urlInput.mode = "live";
            }
            // S: スライドURLの入力
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                urlInput.OpenInputField();
                urlInput.mode = "slide";
            }
            // K: 入力を閉じる
            else if (Input.GetKeyDown(KeyCode.K))
            {
                urlInput.CloseInputField();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                slideSystem.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "UpdateSeekForward");
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                slideSystem.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "UpdateSeekBackward");
            }
        }
    }
}
