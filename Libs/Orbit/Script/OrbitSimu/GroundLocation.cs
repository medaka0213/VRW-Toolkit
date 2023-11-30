
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GroundLocation : UdonSharpBehaviour
{
    
    public OrbitSimu orbitSimu;
    public float altitude = 0.0f;
    public float latitude = 0.0f;
    public float longitude = 0.0f;

    Transform latitudeTransfrom;
    Transform longitudeTransfrom;
    Transform altitudeTransfrom;
    
    public float updateInterval = 0.5f; 
    private float updateTimer = 0f;

    void Start()
    {
        longitudeTransfrom = gameObject.transform.Find("Longitude");
        latitudeTransfrom = gameObject.transform.Find("Longitude/Latitude");
        altitudeTransfrom = gameObject.transform.Find("Longitude/Latitude/Altitude");
        UpdateTransform();
    }

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        UpdateTransform();
    }

    void UpdateTransform(){
        float _altitude = ((float)orbitSimu.radiusOfEarth + altitude) / (float)orbitSimu.radiusOfEarth;
        latitudeTransfrom.localRotation = Quaternion.Euler(-90 + latitude, 0f, 0f);
        longitudeTransfrom.localRotation = Quaternion.Euler(0f, -longitude, 0f);
        altitudeTransfrom.localPosition = new Vector3(0f, _altitude, 0f);
    }
}
