
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using HoshinoLabs.IwaSync3.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GuppyAddressInput : UdonSharpBehaviour
{
    public VRCUrlInputField addressInput;
    public VideoCore iwasyncVideoCore;
    public UnityVideoPlayer slideSystem;
    public bool isLive = false;

    void Update(){
        TrackHead();
    }

    void TrackHead()
    {
        var player = Networking.LocalPlayer;
        if (player != null){
            var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            transform.position = headData.position;
            transform.rotation = headData.rotation;
        }
    }

    public void OnVideoUpdate(){
        uint mode = uint.Parse("32");
        if (!isLive) mode = uint.Parse("16");
        iwasyncVideoCore.TakeOwnership();
        iwasyncVideoCore.PlayURL(mode, addressInput.GetUrl());
        iwasyncVideoCore.RequestSerialization();

        //入力が終わったら非表示にする
        addressInput.SetUrl(VRCUrl.Empty);
        gameObject.SetActive(false);
    } 

    public void OnSlideUpdate(){
        slideSystem.TakeOwnership();
        slideSystem.SetUrl(addressInput.GetUrl());
        slideSystem.SetProgramVariable("slideIndex",0);

        //入力が終わったら非表示にする
        addressInput.SetUrl(VRCUrl.Empty);
        gameObject.SetActive(false);
    }
}
