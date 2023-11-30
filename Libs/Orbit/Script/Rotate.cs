using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Rotate : UdonSharpBehaviour
{
    public float initAngleEarth;
    public float initAngleSun;
    public GravitySource GravitySource;
    [SerializeField]float angleEarth;
    [SerializeField]float angleSun;

    public Transform earth;
    public Transform sun;
    public Transform sunLight;
    public Transform sunAxis;
    float timePerYear;
    float timePerDay;

    float otherObjectInfoTimer;
    float otherObjectInfoTimerMax = 1f;

    void Start(){
        UpdateOtherObjectInfo();
    }

    void Update(){
        UpdateOtherObjectInfoTimer();
        angleEarth = initAngleEarth;        
        angleSun = initAngleSun;

        float timeInDay = (float)((GravitySource.elapsedTime % timePerDay) / timePerDay);
        float dayInYear = (float)((GravitySource.elapsedTime % timePerYear) / timePerYear);

        angleEarth += 360f * timeInDay;
        angleSun += 360f * dayInYear;
        
        
        if (earth){
            if (!GravitySource.isGalileoBeaten){
                float earthRotation = CalcEarthRotationRevs(GravitySource.elapsedTime);
                earth.localRotation =  Quaternion.Euler(0f, -earthRotation*360, 0f);
            } else {
                float earthRotation = CalcEarthRotationRevs(0d);
                earth.localRotation =  Quaternion.Euler(0f, earthRotation*360, 0f);
            }

        }
        if (sun){
            if (GravitySource.isGalileoBeaten){
                sun.localRotation =  Quaternion.Euler(0f, angleEarth, 0f);
            } else {
                sun.localRotation =  Quaternion.Euler(0f, initAngleSun, 0f);
            }
            sunLight.localRotation =  Quaternion.Euler(0f, angleSun, 0f);
            sunAxis.localRotation = Quaternion.Euler(0f, -angleSun, 0f);
        }
    }

    void UpdateOtherObjectInfoTimer(){
        otherObjectInfoTimer -= Time.deltaTime;
        if (otherObjectInfoTimer < 0f){
            otherObjectInfoTimer = otherObjectInfoTimerMax;
            UpdateOtherObjectInfo();
        }
    }

    void UpdateOtherObjectInfo(){
        timePerYear = GravitySource.orbitalPeriod;
        timePerDay = GravitySource.rotationPeriod;
    }

    float CalcEarthRotationRevs(double time_seconds){
        double epoch = (double)DateTimeOffset.Parse("2000-01-01T12:00:00+00:00").ToUnixTimeMilliseconds() / 1000d;
        double revs = 0.7790572732640  + (time_seconds - epoch) / GravitySource.rotationPeriod;
        return (float)(revs%1d);
    }
}


