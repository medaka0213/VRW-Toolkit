
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ToOrbitSimu : UdonSharpBehaviour
{
    public float padLatitude = 0f;
    public float padLongitude = 0f;
    public float padAzimuth = 0f;
    public float targetInclination = 0f;
    
    public Telemetry telemetry;
    public Vincenty vincenty;
    public OrbitSimu orbitSimu;

    public Transform rocketBody;
    
    void Start()
    {
        CalcAzimuth();
    }

    void Update()
    {
        CalcAzimuth();
        vincenty.Latitude = padLatitude;
        vincenty.Longitude = padLongitude;
        vincenty.Bearing = padAzimuth;
        vincenty.DistanceKm = (float)telemetry.downrange_distance;

        vincenty.CalculatePosition();

        //軌道シミュ
        orbitSimu.latitude = vincenty.currentLatitude;
        orbitSimu.longitude = vincenty.currentLongitude;
        orbitSimu.height = (float)telemetry.altitude;
        orbitSimu.direction = (float)vincenty.currentBearing;

        float angle = (float)telemetry.angle;
        if (angle < 0f)
        {
            angle += 180f;
        }

        rocketBody.localRotation = Quaternion.Euler(-angle, 0f, 0f);
    }

    public void CalcAzimuth()
    {
        if (Mathf.Abs(targetInclination) > Mathf.Abs(padLatitude)){
            padAzimuth = Mathf.Asin(Mathf.Cos(targetInclination * Mathf.PI / 180f) / Mathf.Cos(padLatitude * Mathf.PI / 180f)) / (Mathf.PI / 180f);

            //傾斜角がマイナスなら補正
            if (targetInclination < 0f)
            {
                padAzimuth = 180f - padAzimuth;
            }

        } else {
            padAzimuth = 0f;
        }
    }
}
