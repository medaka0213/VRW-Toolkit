
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

public class RocketFlightBody : UdonSharpBehaviour
{
    public Telemetry simulator;
    public float posScaleX = 1f;
    public float posScaleY = 1f;

    void Start()
    {
        
    }
    
    void Update(){
        UpdatePos();
    }

    void UpdatePos(){
        transform.localPosition = new Vector3(
            0f,
            (float)simulator.altitude * 1000f * posScaleY,
            (float)simulator.downrange_distance * 1000f * posScaleX
        );

        float angle = (float)simulator.angle; 
        angle = Mathf.Rad2Deg * Mathf.Atan2(Mathf.Cos(angle*Mathf.Deg2Rad) * posScaleX, Mathf.Sin(angle*Mathf.Deg2Rad) * posScaleY);
        if (simulator.angle < 0f){
            angle += 180f;
        }

        transform.localRotation = Quaternion.Euler(
            (float)angle, 0f, 0f
        );
    }
}
