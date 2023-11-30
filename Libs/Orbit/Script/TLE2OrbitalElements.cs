using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class TLE2OrbitalElements : UdonSharpBehaviour
{
    public Text TLE;
    public Orbit target;
    public bool setOnEnable = true;

    void Start(){
        if (setOnEnable) SetOrbitalElements();
    }

    void OnEnable(){
        if (setOnEnable) SetOrbitalElements();
    }

    public void SetOrbitalElements(){
        target.TLEReferance = TLE;
        target.ParseTLE();
    } 
}
