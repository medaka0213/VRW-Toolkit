
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Kamidana : UdonSharpBehaviour
{
    public OrbitSimu orbit_simu;
    public RotateObject iss_pitch;
    public float direction;

    public float updateInterval = 0.5f; 
    private float updateTimer = 0f;

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate() {
        direction = 180f - (float)orbit_simu.direction;
        float is_inverse = Mathf.Sign(Mathf.Cos(Mathf.PI / 180 * iss_pitch.angle_x));

        if (is_inverse == -1f){
            transform.localRotation = Quaternion.Euler(0f, - direction - 90f, 0f);
        } else {
            transform.localRotation = Quaternion.Euler(0f, direction - 90f, 0f);
        }
    }
}
