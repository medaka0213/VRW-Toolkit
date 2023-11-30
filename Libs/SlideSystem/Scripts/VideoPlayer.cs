using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Components.Video;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class StreamPlayer : UdonSharpBehaviour
{
    public VRCAVProVideoPlayer videoPlayer;

    public float syncTimerDuration = 1f;
    public float fps = 10f;
    [Space(10)] public VRCUrl url;

    [UdonSynced] private int slideIndex = 0;
    private bool errorOccurred = false;
    private float reloadTimer;    
    private float syncTimer;
    private bool loading = true;

    public Text diaplayDialog;
    public Text displayOwner;
    //public Slider seekModifier;    
    public Text Keypad;

    //追加
    public GameObject AccessDenied;
    public float timeToLoad = 1f;
    private bool isEntryLoadTimerOver = false;

    private void Start(){
        syncTimer = syncTimerDuration;
    }

    void Update(){
        UpdateLoadTimer();
        UpdateReloadTimer();

        if (!videoPlayer.IsReady)
            return;

        if (loading)
            return;

        UpdateSeek();
        //videoPlayer.SetTime(1f);
    }

    public void Sync() {
        diaplayDialog.text = "Loading...";
        loading = true;
        videoPlayer.Stop();
        videoPlayer.LoadURL(url);
    }
    public override void OnVideoReady() {
        diaplayDialog.text = "Ready";
        loading = false;
        AccessDenied.SetActive(false);
        videoPlayer.Play();
        UpdateSeek();
    }
    public override void OnVideoStart() {
        diaplayDialog.text = "Start";
    }
    public override void OnVideoError(VideoError videoError) {
        diaplayDialog.text = $"Error: {videoError} Reloading...";
        if ( $"{videoError}" == "AccessDenied"){
            AccessDenied.SetActive(true);
        } else {
            AccessDenied.SetActive(false);
        }
        SetReloadTimer(0.1f);
    }
    public override void OnVideoEnd() {
        diaplayDialog.text = "End";
    }

    //リロードタイマー
    private void SetReloadTimer (float interval) {
        reloadTimer = interval;
        errorOccurred = true;
    }
    private void UpdateReloadTimer (){
        if (!errorOccurred){
            return;
        }

        reloadTimer -= Time.deltaTime;
        if (reloadTimer < 0f){
            errorOccurred = false;
            Sync();
        }
    }

    //オーナー判定
    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    private void UpdateUI(){
        float curTime = videoPlayer.GetTime();
        float dur = videoPlayer.GetDuration();
        float durIndex = dur * fps;

        int durIndexInt = (int)(durIndex-durIndex%1f -2f);

        if (slideIndex <= 0f){
            diaplayDialog.text = $"Slide-0 of {(durIndexInt)}";
        } else {
            diaplayDialog.text = $"Slide-{slideIndex-1} of {(durIndexInt)}, {curTime} sec";
        }

        if (isOwner()){
            if (slideIndex-1 > durIndexInt ){
                slideIndex = durIndexInt;
            } else if (slideIndex < 0) {
                slideIndex = 0;
            }
        }

        /*
        if(curTime>0f){
            seekModifier.value = curTime / dur;
        }
        */

        //displayOwner.text = "owner: " + Networking.GetOwner(gameObject).displayName;
    }

    private void _UpdateSeek(float time){
        displayOwner.text = "seekTime = " + time;
        if (time >= 0f){
            videoPlayer.SetTime(time);
        } else {
            videoPlayer.SetTime(0f);
        }
        videoPlayer.Pause();
        UpdateUI();
    }

    public void UpdateSeek(){
        if (Time.time % 2f >= 1f) {
            _UpdateSeek((float)slideIndex/fps + 1f/fps*(2f/3f));
        } else {
            _UpdateSeek((float)slideIndex/fps + 1f/fps*(1f/3f));
        }
    }

    public void UpdateSeekForward(){
        slideIndex +=1;
        UpdateSeek();
    }

    public void UpdateSeekBackward(){
        slideIndex -=1;
        UpdateSeek();
    }

    /*
    public void UpdateSeekTime()
    {
        var dur = videoPlayer.GetDuration();
        var time = dur * seekModifier.value * fps;
        slideIndex = (int)time;
        UpdateSeek();
    }
    */

    public void EnterKeypad(){
        takeOwnership();
        slideIndex = int.Parse(Keypad.text)+1;
        UpdateSeek();
    }

    private void UpdateLoadTimer(){
        if (timeToLoad < 0){
            if( isEntryLoadTimerOver ){
                return;
            }
            isEntryLoadTimerOver = true;
            Sync();
        } else {
            timeToLoad -= Time.deltaTime;
        }
    }
}