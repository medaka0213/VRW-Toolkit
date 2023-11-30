
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class RocketLauncher : UdonSharpBehaviour
{
    public bool enable = false;

    public Vector2[] waypoints;
    public float twr = 1.2f;
    public AnimationCurve altitiudeCurve;

    public Transform targetAzimuth;
    public Transform targetPos;
    public Transform targetPitch;
    public Transform targetRoll;

    public float initTPlus = -3f;
    public float tPlus = 0f;

    public Vector2 velocity;
    public Vector2 position;
    public Vector2 acceleration;
    public float pitch = 0f;
    public float roll = 0f;

    float maxHeight;
    float maxDownrange;

    public float azimuth = 0f;
    public float rollAngleTarget = 0f;
    public float rollStart = 10f;
    public float rollEnd = 20f;
    public float simulationEnd = 60f;
    public Animator[] animatorList;

    void Start()
    {
        maxHeight = waypoints[waypoints.Length-1].y;
        maxDownrange = waypoints[waypoints.Length-1].x;

        altitiudeCurve = new AnimationCurve();
        altitiudeCurve.AddKey(0, 0);
        altitiudeCurve.AddKey(waypoints[0].y/maxHeight, waypoints[0].x/maxDownrange);
        altitiudeCurve.SmoothTangents(0, 0);
        altitiudeCurve.SmoothTangents(1, 0);

        for (int i = 1; i < waypoints.Length; i++)
        {
            altitiudeCurve.AddKey(waypoints[i].y/maxHeight, waypoints[i].x/maxDownrange);
            altitiudeCurve.SmoothTangents(i+1, 0);
        }
        Reset();
    }

    void Update()
    {
        if (enable) {
            tPlus += Time.deltaTime;
            if (tPlus < 0) {
                ResetPosition();
            }
            if (tPlus > simulationEnd){
                Reset();
                enable = false;
            }

            if(rollStart < tPlus && tPlus < rollEnd){
                roll = Mathf.Lerp(initRoll(), rollAngleTarget, (tPlus - rollStart) / (rollEnd - rollStart));
            } else if (tPlus > rollEnd){
                roll = rollAngleTarget;
            } else {
                roll = initRoll();
            }
            CalcFlightPath();
        } else {
            ResetPosition();
        }

        targetPos.localPosition = new Vector3(position.x, position.y, 0);
        targetPitch.localRotation = Quaternion.Euler(0, 0, -pitch);
        targetRoll.localRotation = Quaternion.Euler(0, roll, 0);
        targetAzimuth.localRotation = Quaternion.Euler(0, azimuth, 0);
    }

    public void Reset(){
        tPlus = initTPlus;
        ResetPosition();
    }

    private float initRoll (){
        return -azimuth + this.transform.localRotation.eulerAngles.y;
    }

    public void ResetPosition(){
        velocity = new Vector2(0, 0);
        position = new Vector2(0, 0);
        acceleration = new Vector2(0, 0);
        pitch = 0f;
        roll = initRoll();
    }

    public void Enable (){
        Reset();
        for (int i = 0; i < animatorList.Length; i++)
        {
            animatorList[i].SetInteger("mode", 1);
        }
        enable = true;
    }

    public void Disable (){
        Reset();
        for (int i = 0; i < animatorList.Length; i++)
        {
            animatorList[i].SetInteger("mode", 0);
        }
        enable = false;
    }

    public void EnableToAll(){
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Enable");
    }

    public void DisableToAll(){
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Disable");
    }

    void CalcFlightPath(){
        float _pitch = pitch / 180 * Mathf.PI;
        acceleration = new Vector2(0, -9.8f);
        acceleration += 9.8f * twr * new Vector2(Mathf.Sin(_pitch), Mathf.Cos(_pitch));
        position += velocity * Time.deltaTime + acceleration * Time.deltaTime * Time.deltaTime / 2;
        position = new Vector2(altitiudeCurve.Evaluate(position.y/maxHeight)*maxHeight, position.y);
        velocity += acceleration * Time.deltaTime;
        pitch = Mathf.Atan2(altitiudeCurve.Evaluate(position.y/maxHeight + 0.0001f) - altitiudeCurve.Evaluate(position.y/maxHeight), 0.0001f) * 180 / Mathf.PI;
    }
}
