using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class OrbitSimu : UdonSharpBehaviour
{
    public Orbit orbit;
    public AtmosphereSimu atmosphereSimu;

    public float timeInDay = 0f;
    public float dayInYear = 0f;
    public float latitude = 0f;
    public float longitude = 0f;
    public float height = 420f;
    public float direction = 0f;

    public float radiusOfEarth = 6371f;
    public float radiusOfCelestialSphere = 10000f;

    public bool isDebug = false;

    private Transform EarthPosition;
    private Transform SunPosition;

    private Transform skyboxLatitude;
    private Transform skyboxLongitude;
    private Transform skyboxTimeOffset;
    private Transform skyboxDayOffset;
    private Transform skyboxAir;

    private Transform earthLatitude;
    private Transform earthLongitude;

    private Transform sunLatitude;
    private Transform sunLongitude;
    private Transform sunTimeOffset;
    private Transform sunDayOffset;

    public float earthCenterPosY = 5000f;


    void Start()
    {
        skyboxLatitude = gameObject.transform.Find("SkyBox/Latitude");
        skyboxLongitude = gameObject.transform.Find("SkyBox/Latitude/Longitude");
        skyboxTimeOffset = gameObject.transform.Find("SkyBox/Latitude/Longitude/Time-Offset");
        skyboxDayOffset = gameObject.transform.Find("SkyBox/Latitude/Longitude/Time-Offset/Axis/Season-Offset");
        skyboxAir = gameObject.transform.Find("SkyboxAir");

        EarthPosition = gameObject.transform.Find("Earth");

        earthLatitude = gameObject.transform.Find("Earth/Latitude");
        earthLongitude = gameObject.transform.Find("Earth/Latitude/Longitude");

        SunPosition = gameObject.transform.Find("Sun");
        sunLatitude = gameObject.transform.Find("Sun/Latitude");
        sunLongitude = gameObject.transform.Find("Sun/Latitude/Longitude");
        sunTimeOffset = gameObject.transform.Find("Sun/Latitude/Longitude/Time-Offset");
        sunDayOffset = gameObject.transform.Find("Sun/Latitude/Longitude/Time-Offset/Axis/Season-Offset");
    }

    void Update()
    {
        UpdateTransForm();
        UpdateTimeOffset();
        UpdateHorizon();
        if (orbit)
        {
            UpdateOrbit();
        }
        if (atmosphereSimu)
        {
            UpdateAtm();
        }
    }

    public void UpdateTransForm()
    {
        float scaleOfEarth = earthCenterPosY * (radiusOfEarth / (radiusOfEarth + height));

        transform.localRotation = Quaternion.Euler(0f, -direction, 0f);

        skyboxLatitude.localRotation = Quaternion.Euler(90f - latitude, 0f, 0f);
        earthLatitude.localRotation = Quaternion.Euler(90f - latitude, 0f, 0f);
        sunLatitude.localRotation = Quaternion.Euler(90f - latitude, 0f, 0f);

        skyboxLongitude.localRotation = Quaternion.Euler(0f, longitude, 0f);
        earthLongitude.localRotation = Quaternion.Euler(0f, longitude, 0f);
        sunLongitude.localRotation = Quaternion.Euler(0f, longitude, 0f);

        skyboxTimeOffset.localRotation = Quaternion.Euler(0f, (timeInDay - dayInYear) * 360f, 0f);
        sunTimeOffset.localRotation = Quaternion.Euler(0f, (timeInDay - dayInYear - 0.5f) * 360f, 0f);

        sunDayOffset.localRotation = Quaternion.Euler(0f, dayInYear * 360f, 0f);
        skyboxDayOffset.localRotation = Quaternion.Euler(0f, 0f, -dayInYear * 360f);

        EarthPosition.localPosition = new Vector3(0f, -earthCenterPosY, 0f);
        SunPosition.localPosition = new Vector3(0f, -earthCenterPosY, 0f);

        EarthPosition.localScale = new Vector3(
            scaleOfEarth,
            scaleOfEarth,
            scaleOfEarth
        );
    }

    public void UpdateTimeOffset()
    {
        if (!isDebug)
        {
            var now = DateTime.UtcNow;
            var timeSpan = new TimeSpan(now.Hour, now.Minute, now.Second);
            var daySpan = now - new DateTime(now.Year, 1, 1);

            timeInDay = (float)timeSpan.TotalSeconds / (24f * 60f * 60f);
            dayInYear = (float)daySpan.TotalDays / 365f;
        }
    }

    public void UpdateHorizon()
    {
        float angleHorizon = (float)Math.Acos(radiusOfEarth / (radiusOfEarth + height));
        float skyboxAirHeight = /*radiusOfEarth + height*/ -(float)Math.Tan(angleHorizon) * radiusOfCelestialSphere;
        skyboxAir.localPosition = new Vector3(0f, skyboxAirHeight, 0f);
    }

    public void UpdateOrbit()
    {
        latitude = orbit.latitude;
        longitude = orbit.longitude;
        height = orbit.altitude;
        direction = orbit.azimuth;
    }
    public void UpdateAtm()
    {
        atmosphereSimu.altitude = height;
    }

    public void SetHeight(float height)
    {
        this.height = height;
    }
}
