using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CalcPosition: UdonSharpBehaviour
{
    public Transform target;
    public GravitySource GravitySource;

    public bool SendVariables = false;
    public UdonSharpBehaviour target_orbitsimu;
    public float latitude;
    public float longitude;
    public float altitude;
    public float height;

    void Update() {
        UpdateOrbitalElement();
        if (SendVariables){
            SendOrbitalElement();
        }
    }


    void UpdateOrbitalElement(){
        Vector3 pos = target.localPosition;

        altitude = pos.magnitude - GravitySource.radius;
        latitude = 90f - (Mathf.Acos(pos.y/ (altitude-GravitySource.radius)) * 180f / Mathf.PI);
        longitude = Mathf.Sign(pos.z) * Mathf.Acos(pos.x / Mathf.Sqrt(Mathf.Pow(pos.x, 2f) + Mathf.Pow(pos.z, 2f))) * 180f / Mathf.PI;
    }

    void SendOrbitalElement(){
        if (target_orbitsimu){
            target_orbitsimu.SetProgramVariable("latitude", latitude);
            target_orbitsimu.SetProgramVariable("longitude", longitude);
            target_orbitsimu.SetProgramVariable("height", altitude);
        }
    } 
}
