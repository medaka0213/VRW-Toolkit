using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using UdonToolkit;
using HoshinoLabs.IwaSync3.Udon;

public class GuppyUrlInput : UdonSharpBehaviour
{
    public VRCUrlInputField addressInput;
    public UnityVideoPlayer slideSystem;


    int rescentVideoIndex = 0;
    public GameObject inputField;

    [FoldoutGroup("Videos")]
    public string mode = "live";

    [FoldoutGroup("Videos")]
    [ListView("VideoPlayer List")]
    public VideoController[] videoControllers;
    [FoldoutGroup("Videos")]
    [ListView("VideoPlayer List")]
    public VideoCore[] videoCores;


    void Start()
    {
        inputField.SetActive(false);
    }


    void Update()
    {
        if (inputField.activeSelf)
        {
            //頭を追従
            var player = Networking.LocalPlayer;
            if (player != null)
            {
                var headData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                transform.position = headData.position;
                transform.rotation = headData.rotation;
            }
        }
    }

    public void OpenInputField()
    {
        inputField.SetActive(true);
    }

    public void CloseInputField()
    {
        inputField.SetActive(false);
    }

    int getVideoPlayerIndex()
    {
        // loop  VideoCore
        for (int i = 0; i < videoCores.Length; i++)
        {
            if (!videoCores[i].isPlaying)
            {
                return i;
            }
        }
        return -1;
    }


    public void LockVideo()
    {
        foreach (var videoController in videoControllers)
        {
            videoController.TakeOwnership();
            videoController.LockOn();
        }
    }

    public void UnlockVideo()
    {
        foreach (var videoController in videoControllers)
        {
            videoController.TakeOwnership();
            videoController.LockOff();
        }
    }

    public void OnUrlInput()
    {
        if (mode == "slide")
        {
            string url = addressInput.GetUrl().Get();
            if (url.StartsWith("page"))
            {
                int value = 0;
                int.TryParse(url.Substring(4), out value);
                SetSlidePage(value);
            }
            else
            {
                OnSlideUpdate();
            }
        }
        else
        {
            string url = addressInput.GetUrl().Get();
            if (url == "unlock")
            {
                UnlockVideo();
            }
            else if (url == "lock")
            {
                LockVideo();
            }
            else if (url.StartsWith("set"))
            {
                float value = float.Parse(url.Substring(3));
                SetAllVideoTime(value);
            }
            else
            {
                uint _mode = uint.Parse("32");
                if (mode != "live") _mode = uint.Parse("16");

                int videoIndex = getVideoPlayerIndex();
                if (videoIndex != -1)
                {
                    VideoCore videoCore = videoCores[videoIndex];
                    videoCore.TakeOwnership();
                    videoCore.PlayURL(_mode, addressInput.GetUrl());
                    videoCore.RequestSerialization();

                    VideoController videoCtrl = videoControllers[videoIndex];
                    videoCtrl.LockOn();

                    rescentVideoIndex = videoIndex;
                }
            }
        }
        //入力が終わったら非表示にする
        addressInput.SetUrl(VRCUrl.Empty);
        inputField.SetActive(false);
    }

    public void OnSlideUpdate()
    {
        slideSystem.TakeOwnership();
        slideSystem.SetUrl(addressInput.GetUrl());
        slideSystem.SetProgramVariable("slideIndex", 0);
    }

    public void SetVideoTime(VideoCore core, float seekTimeSeconds)
    {
        core.TakeOwnership();
        core.clockTime = Networking.GetServerTimeInMilliseconds();
        core.Seek(seekTimeSeconds);
        core.RequestSerialization();
    }

    public void SetAllVideoTime(float seekTimeSeconds)
    {
        foreach (var videoCore in videoCores)
        {
            SetVideoTime(videoCore, seekTimeSeconds);
        }
    }

    public void SetSlidePage(int page)
    {
        slideSystem.TakeOwnership();
        slideSystem.SetProgramVariable("slideIndex", page);
        slideSystem.UpdateSeek();
    }
}