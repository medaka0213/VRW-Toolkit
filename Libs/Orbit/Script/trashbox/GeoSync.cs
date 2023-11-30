
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GeoSync : UdonSharpBehaviour
{
    public Orbit target;
    public GravitySource GravitySource;
    public float meanMotion = 1f;
    public float eccentricity = 0f;

    void Start()
    {
        float orbitalPeriod = 1f / meanMotion * GravitySource.rotationPeriod; //軌道周期[s]
        float semi_major_axis = Mathf.Pow(orbitalPeriod, 2f) * (float)GravitySource.GM/(float)1e+9  / Mathf.Pow(2f * Mathf.PI, 2);
        semi_major_axis = Mathf.Pow(semi_major_axis, .333333333f);
        float Ap = semi_major_axis * (1f + eccentricity) - GravitySource.radius;
        float Pe = semi_major_axis * (1f - eccentricity) - GravitySource.radius;

        target.SetProgramVariable("Ap", Ap);
        target.SetProgramVariable("Pe", Pe);
    }
}
