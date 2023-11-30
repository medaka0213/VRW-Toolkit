using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrtitalText : UdonSharpBehaviour
{
    public Orbit orbit;
    public LookAtPlayercs lookAtPlayercs;
    public bool earthOrbit = false;

    public float updateInterval = 0.5f; 
    private float updateTimer = 0f;

    void Start()
    {
        
    }

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        orbit.CalcCoordinate();
        string titleText = orbit.gameObject.name;
        string descText = "";
        if (earthOrbit){
            descText = String.Format("{0:#,0} km", orbit.altitude);
        } else {
            float au = 149597870.7f;
            descText = String.Format("{0:#.##} AU = {1:#,0} km", orbit.altitude, orbit.altitude * au);
        }
        
        if (lookAtPlayercs){
            lookAtPlayercs.titleText = titleText;
            lookAtPlayercs.descText = descText;
        }
    }
}
