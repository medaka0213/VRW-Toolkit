using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GravitySource : UdonSharpBehaviour
{
    // GM; 標準重力パラーメータ [m*m*m/s/s]
    public double GM=3.98600441e+14;
    // R; 重力源の半径 [km]
    public float radius = 6378.1f; //地球の半径
    // 再生速度
    [UdonSynced] public float playbackSpeed = 21600f;
    public string playbackSpeedFormat = "21600";
    // 重力源の時点を考慮するか否か
    [UdonSynced] public bool isGalileoBeaten = false;
    // シミュレーション経過時間 [s]
    public double elapsedTime = 0f;
    // シミュレーション経過時間のオフセット
    [UdonSynced] public double elapsedTimeOffset = 0f;
    // 重力源の自転周期 [s]
    public float rotationPeriod = 86164.098691f; //地球の自転周期
    // 重力源の公転周期 [s]
    public float orbitalPeriod = 31556925f; //地球の公転周期

    public bool formatElapsedTime = false;
    [SerializeField] string elapsedTimeFormatted = "0.000";

    // リアル→1分→1時間→6時間→1日→1週間→1カ月→半年→1年
    float[] rotationMatrix = new float[9] { 1f, 60f, 3600f, 21600f, 86400f, 604800f, 2628000f, 15768000f, 31536000f};

    void Update() {
        elapsedTime = (double)DateTimeOffset.Now.ToUnixTimeMilliseconds()  / 1000d * (double)playbackSpeed + elapsedTimeOffset;
        elapsedTime = elapsedTime % 253402300799d;
        if (formatElapsedTime) {
            elapsedTimeFormatted = new DateTime((long)(elapsedTime * 10000000d)).ToString("yyyy-MM-dd HH:mm:ss.fff zzz");
        }
    }

    public void ToggleGalileoBeat(){
        if(!isOwner()){
            takeOwnership();
        }
        isGalileoBeaten = !isGalileoBeaten;
    }

    public void SetElapsedTimeOffset(double offset){
        if(!isOwner()){
            takeOwnership();
        }
        elapsedTimeOffset += offset;
    }

    public void Add1Day(){
        SetElapsedTimeOffset(86400d);
    }
    public void Add1Hour(){
        SetElapsedTimeOffset(3600d);
    }
    public void Add1Minute(){
        SetElapsedTimeOffset(60d);
    }
    public void Substract1Day(){
        SetElapsedTimeOffset(-86400d);
    }
    public void Substract1Hour(){
        SetElapsedTimeOffset(-3600d);
    }
    public void Substract1Minute(){
        SetElapsedTimeOffset(-60d);
    }

    public void SetPlaybackSpeed(float speed){
        if(!isOwner()){
            takeOwnership();
        }
        playbackSpeed = speed;
        playbackSpeedFormat = playbackSpeed + "";
    }
    public void SetFaster(){
        for (int i = 0; i < rotationMatrix.Length; i++) {
            if (playbackSpeed < rotationMatrix[i]) {
                SetPlaybackSpeed(rotationMatrix[i]);
                return;
            }
        }
    }
    public void SetSlower(){
        for (int i = rotationMatrix.Length - 1; i >= 0; i--) {
            if (playbackSpeed > rotationMatrix[i]) {
                SetPlaybackSpeed(rotationMatrix[i]);
                return;
            }
        }
    }

    public void ResetElapsedTimeOffset(){
        if(!isOwner()){
            takeOwnership();
        }
        elapsedTimeOffset = 0d;
    }

    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership(){
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }
}
