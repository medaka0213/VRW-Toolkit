using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class UnityVideoPlayer : UdonSharpBehaviour
{
    public BaseVRCVideoPlayer videoPlayer;

    public float syncTimerDuration = 1f;
    public float fps = 10f;
    [UdonSynced] public VRCUrl url;
    private VRCUrl _urlPrev;
    private VRCUrl _urlMain;
    public VRCUrl urlSub;
    [UdonSynced] private int slideIndex = 0;

    private bool errorOccurred = false;
    private float reloadTimer;
    private float syncTimer;
    private bool loading = true;

    public Text diaplayDialog;
    public Text displayOwner;
    public GameObject AccessDenied;
    public float timeToLoad = 1f;
    private bool isEntryLoadTimerOver = false;

    private void Start()
    {
        syncTimer = syncTimerDuration;
        _urlMain = url;
        _urlPrev = url;
    }

    void Update()
    {
        UpdateLoadTimer();
        UpdateReloadTimer();

        if (!videoPlayer.IsReady)
            return;

        if (loading)
            return;

        if (_urlPrev.Get() != url.Get())
        {
            diaplayDialog.text = "URL Changed";
            onVideoUrlChanged();
        }

        UpdateSeek();
    }

    public void Sync()
    {
        diaplayDialog.text = "読み込み中...";
        loading = true;
        videoPlayer.Stop();
        videoPlayer.LoadURL(url);
    }

    public void SyncToAll()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Sync");
    }

    public void SetUrl(VRCUrl _url)
    {
        TakeOwnership();
        slideIndex = 0;
        url = _url;
    }

    public void SetMainUrl()
    {
        SetUrl(_urlMain);
    }

    public void SetSubUrl()
    {
        SetUrl(urlSub);
    }

    void onVideoUrlChanged()
    {
        if (url.Get() == "")
        {
            diaplayDialog.text = "無効なURLです。処理を中断します。";
            url = _urlPrev;
        }
        else
        {
            _urlPrev = url;
            Sync();
        }
    }

    public override void OnVideoReady()
    {
        diaplayDialog.text = "Ready";
        loading = false;
        AccessDenied.SetActive(false);
        videoPlayer.Play();
        UpdateSeek();
    }
    public override void OnVideoStart()
    {
        diaplayDialog.text = "Start";
    }
    public override void OnVideoError(VideoError videoError)
    {
        if ($"{videoError}" == "AccessDenied")
        {
            AccessDenied.SetActive(true);
        }
        else
        {
            AccessDenied.SetActive(false);
        }
        if ($"{videoError}" == "RateLimited")
        {
            diaplayDialog.text = $"Error: 混雑しています。しばらくお待ちください...";
        }
        else
        {
            diaplayDialog.text = $"Error: {videoError} Reloading...";
        }
        SetReloadTimer(0.1f);
    }
    public override void OnVideoEnd()
    {
        diaplayDialog.text = "End";
    }

    //リロードタイマー
    private void SetReloadTimer(float interval)
    {
        reloadTimer = interval;
        errorOccurred = true;
    }
    private void UpdateReloadTimer()
    {
        if (!errorOccurred)
        {
            return;
        }

        reloadTimer -= Time.deltaTime;
        if (reloadTimer < 0f)
        {
            errorOccurred = false;
            Sync();
        }
    }

    //オーナー判定
    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    public void TakeOwnership()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    private void UpdateUI()
    {
        float curTime = videoPlayer.GetTime();
        float dur = videoPlayer.GetDuration();
        float durIndex = dur * fps;

        int durIndexInt = (int)(durIndex - durIndex % 1f - 2f);

        if (slideIndex <= 0f)
        {
            diaplayDialog.text = $"Slide-0 of {(durIndexInt)}";
        }
        else
        {
            diaplayDialog.text = $"Slide-{slideIndex} of {(durIndexInt)}";
        }

        if (isOwner())
        {
            if (slideIndex - 1 > durIndexInt)
            {
                slideIndex = durIndexInt;
            }
            else if (slideIndex < 0)
            {
                slideIndex = 0;
            }
        }

        var owner = Networking.GetOwner(gameObject);
        if (owner != null)
        {
            displayOwner.text = "owner: " + owner.displayName;
        }
    }

    private void _UpdateSeek(float time)
    {
        videoPlayer.Play();
        if (time >= 0f)
        {
            videoPlayer.SetTime(time);
        }
        else
        {
            videoPlayer.SetTime(0f);
        }
        videoPlayer.Pause();
        UpdateUI();
    }

    public void UpdateSeek()
    {
        if (Time.time % 2f >= 1f)
        {
            _UpdateSeek((float)slideIndex / fps + 1f / fps * (2f / 3f));
        }
        else
        {
            _UpdateSeek((float)slideIndex / fps + 1f / fps * (1f / 3f));
        }
    }

    public void UpdateSeekForward()
    {
        slideIndex += 1;
        UpdateSeek();
    }

    public void UpdateSeekBackward()
    {
        slideIndex -= 1;
        UpdateSeek();
    }

    private void UpdateLoadTimer()
    {
        if (timeToLoad < 0)
        {
            if (isEntryLoadTimerOver)
            {
                return;
            }
            isEntryLoadTimerOver = true;
            Sync();
        }
        else
        {
            timeToLoad -= Time.deltaTime;
        }
    }
}