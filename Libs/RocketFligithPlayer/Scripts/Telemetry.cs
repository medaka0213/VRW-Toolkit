using System;

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

public class Telemetry : UdonSharpBehaviour
{
    public Text telemetry;

    float[] _time;

    double[] _altitude;

    float[] _velocity;
    float[] _velocity_x;
    float[] _velocity_y;

    float[] _acceleration;
    
    double[] _downrange_distance;
    
    float[] _angle;
    float[] _q;
    
    public float time;
    public double altitude;
    public float velocity;
    public float velocity_x;
    public float velocity_y;
    public float acceleration;
    public double downrange_distance;
    public float angle;
    public float q;

    public double seekTime = 10f;
    [UdonSynced]public bool paused = true;

    [UdonSynced] public double t_zero = 0d;
    public double missionElapsedTime = 0d;

    [UdonSynced] int currentIndex = 0;

    void Start()
    {
        ParseTelemetry();
    }

    void Update(){
        SetElapsedTime();
        CalcCurrentValues();
    }


        void OnPlayerJoined(VRCPlayerApi player)
    {
        if (isOwner && t_zero == 0d)
        {
            UpdateTime(0d);
        }
    }


    public bool isOwner => Networking.IsOwner(gameObject);
    public void TakeOwnership()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

    //時間のリセット
    public void SetElapsedTime()
    {
        if (paused) return;
        double now = DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000d;
        missionElapsedTime = now - t_zero;
    }

    //時間の操作
    public void UpdateTime(double time){
        TakeOwnership();

        double now = DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000d;
        missionElapsedTime = time;
        t_zero = now - missionElapsedTime;

        currentIndex = 0;
    }

    public void ResetTime(){
        UpdateTime(0d);
    }

    public void TogglePaused(){
        TakeOwnership();
        paused = !paused;
    }

    public void SeekForward(){
        UpdateTime(missionElapsedTime + seekTime);
    }

    public void SeekBackward(){
        UpdateTime(missionElapsedTime - seekTime);
    }

    public void ParseTelemetry(){
        string[] lines = telemetry.text.Split('\n');
        _time = new float[lines.Length];
        _altitude = new double[lines.Length];
        _velocity = new float[lines.Length];
        _velocity_x = new float[lines.Length];
        _velocity_y = new float[lines.Length];
        _acceleration = new float[lines.Length];
        _downrange_distance = new double[lines.Length];
        _angle = new float[lines.Length];
        _q = new float[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {   
            string[] tokens = lines[i].Split(' ');

            if (tokens.Length < 9)
            {
                continue;
            }

            _time[i] = float.Parse(tokens[0]);
            _altitude[i] = double.Parse(tokens[1]);
            _velocity[i] = float.Parse(tokens[2]);
            _velocity_x[i] = float.Parse(tokens[3]);
            _velocity_y[i] = float.Parse(tokens[4]);
            _acceleration[i] = float.Parse(tokens[5]);
            _downrange_distance[i] = double.Parse(tokens[6]);
            _angle[i] = float.Parse(tokens[7]);
            _q[i] = float.Parse(tokens[8]);
        }
    }

    void SmoothValues(){
        if (currentIndex == 0) return;

        time = (float)missionElapsedTime;
        float timeOffset = (time - _time[currentIndex - 1]) / (_time[currentIndex] - _time[currentIndex - 1]);

        altitude = _altitude[currentIndex - 1] + (_altitude[currentIndex] - _altitude[currentIndex - 1]) * (double)timeOffset;
        velocity = _velocity[currentIndex - 1] + (_velocity[currentIndex] - _velocity[currentIndex - 1]) * timeOffset;
        velocity_x = _velocity_x[currentIndex - 1] + (_velocity_x[currentIndex] - _velocity_x[currentIndex - 1]) * timeOffset;
        velocity_y = _velocity_y[currentIndex - 1] + (_velocity_y[currentIndex] - _velocity_y[currentIndex - 1]) * timeOffset;
        acceleration = _acceleration[currentIndex - 1] + (_acceleration[currentIndex] - _acceleration[currentIndex - 1]) * timeOffset;
        downrange_distance = _downrange_distance[currentIndex - 1] + (_downrange_distance[currentIndex] - _downrange_distance[currentIndex - 1]) * (double)timeOffset;
        angle = _angle[currentIndex - 1] + (_angle[currentIndex] - _angle[currentIndex - 1]) * timeOffset;
        q = _q[currentIndex - 1] + (_q[currentIndex] - _q[currentIndex - 1]) * timeOffset;
    }

    void CalcCurrentValues(){
        for ( int i = currentIndex; i < _time.Length; i++ ){
            if ( _time[i] > missionElapsedTime ){
                time = _time[i];
                altitude = _altitude[i];
                velocity = _velocity[i];
                velocity_x = _velocity_x[i];
                velocity_y = _velocity_y[i];
                acceleration = _acceleration[i];
                downrange_distance = _downrange_distance[i];
                angle = _angle[i];
                q = _q[i];

                currentIndex = i;
                break;
            }
        }
        if (!(currentIndex == _time.Length-1)){
            SmoothValues();
        }
    }

    public float LastDownrangeDistance(){
        return (float)_downrange_distance[_time.Length-1];
    }

    public float LastAltitude(){
        return (float)_altitude[_time.Length-1];
    }
}
