
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Vincenty : UdonSharpBehaviour
{

    public float Latitude = 50.390202f;
    public float Longitude = -3.9204310000000078f;
    public float Bearing = 225f;
    public float DistanceKm = 1000f;

    public float currentLatitude;
    public float currentLongitude;
    public float currentBearing;

    public int maxCalcNum = 100; 
    public float minError = 0.001f;

    public bool realTimeCalc = false;

    void Update(){
        if (realTimeCalc) {
            CalculatePosition();
        }
    }

    public void CalculatePosition()
    {
        float Distance = DistanceKm * 1000f;

        float lat1 = Latitude * (Mathf.PI / 180f);
        float lon1 = Longitude * (Mathf.PI / 180f);
        float brg = Bearing * (Mathf.PI / 180f);

        float a = 6378137.0f;
        float b = 6356752.314245f;

        float f = 1f / 298.257223563f;
        float sb = Mathf.Sin(brg);
        float cb = Mathf.Cos(brg);
        float tu1 = (1f-f) * Mathf.Tan(lat1);
        float cu1 = 1f / Mathf.Sqrt((1+tu1*tu1));
        float su1 = tu1 * cu1;
        float s2 = Mathf.Atan2(tu1, cb);
        float sa = cu1 * sb;
        float csa = 1f - sa * sa;
        float us = csa * (a * a - b * b) / (b * b);
        float A = 1 + us / 16384f * (4096f + us * (- 768f + us * (320f - 175f * us)));
        float B = us / 1024f * (256f + us * (-128f + us * (74f - 47f * us)));
        float s1 = Distance / (b * A);
        float s1p = 2f * Mathf.PI;

        
        float cs1m = Mathf.Cos(2f * s2 + s1);
        float ss1 = Mathf.Sin(s1);
        float cs1 = Mathf.Cos(s1);

        for ( int i=0; i<maxCalcNum || Mathf.Abs(s1-s1p) > minError; i++ )
        {
            cs1m = Mathf.Cos(2 * s2 + s1);
            ss1 = Mathf.Sin(s1);
            cs1 = Mathf.Cos(s1);
            float ds1 = B * ss1 * (cs1m + B / 4 * (cs1 * (-1f + 2f * cs1m * cs1m) - B / 6f * cs1m * (-3f + 4f * ss1 * ss1) * (-3f + 4f * cs1m * cs1m)));
            s1p = s1;
            s1 = Distance / (b * A) + ds1;
        }

        float t = su1 * ss1 - cu1 * cs1 * cb;
        float lat2 = Mathf.Atan2(su1 * cs1 + cu1 * ss1 * cb, (1f - f) * Mathf.Sqrt(sa * sa + t * t));
        float l2 = Mathf.Atan2(ss1 * sb, cu1 * cs1 - su1 * ss1 * cb);
        float c = f / 16f * csa * (4f + f * (4f - 3f * csa));
        float l = l2 - (1f - c) * f * sa * (s1 + c * ss1 * (cs1m + c * cs1 * (-1f + 2f * cs1m * cs1m)));
        float d = Mathf.Atan2(sa, -t);
        float finalBrg = d + 2f * Mathf.PI;
        float backBrg = d + Mathf.PI;
        float lon2 = lon1 + l;

        lat2 = lat2 * 180f / Mathf.PI;
        lon2 = lon2 * 180f/ Mathf.PI;
        finalBrg = finalBrg * 180f/ Mathf.PI;
        backBrg = backBrg * 180f / Mathf.PI;

        if (lon2 < -180f)
        {
            lon2 = lon2 + 360f;
        }
        if (lon2 > 180f)
        {
            lon2 = lon2 - 360f;
        }

        if (finalBrg < 0f)
        {
            finalBrg = finalBrg + 360f;
        }
        if (finalBrg > 360f)
        {
            finalBrg = finalBrg - 360f;
        }

        currentLatitude = lat2;
        currentLongitude = lon2;
        currentBearing = finalBrg;
    }
}