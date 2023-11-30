
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotateObject : UdonSharpBehaviour
{
    public float angle_x = 0f;
    public float angle_y = 0f;
    public float angle_z = 0f;
    public bool inversed = false;

    void Start()
    {
        
    }

    public void Update(){
        if (inversed) {
            transform.localRotation = Quaternion.Euler(-angle_x, -angle_y, -angle_z);
        } else {
            transform.localRotation = Quaternion.Euler(angle_x, angle_y, angle_z);
        }
    }

    public void SetQuaternion(Quaternion q){
        angle_x = q.eulerAngles.x;
        angle_y = q.eulerAngles.y;
        angle_z = q.eulerAngles.z;
    }
}
