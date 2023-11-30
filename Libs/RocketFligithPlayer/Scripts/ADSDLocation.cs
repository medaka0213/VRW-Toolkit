
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ADSDLocation : UdonSharpBehaviour
{
    public ToOrbitSimu toOrbitSimu;
    Telemetry telemetry;

    public Vincenty vincenty;
    public GroundLocation groundLocation;
    
    public float updateInterval = 0.5f; 
    private float updateTimer = 0f;
    
    void Start()
    {
        telemetry = toOrbitSimu.telemetry;
    }


    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        CalcLocation();
        groundLocation.latitude = vincenty.currentLatitude;
        groundLocation.longitude = vincenty.currentLongitude;
    }

    void CalcLocation(){
        //緯度経度の計算
        vincenty.Latitude = toOrbitSimu.padLatitude;
        vincenty.Longitude = toOrbitSimu.padLongitude;
        vincenty.Bearing = toOrbitSimu.padAzimuth;
        vincenty.DistanceKm = (float)telemetry.LastDownrangeDistance();

        vincenty.CalculatePosition();
    }
}
