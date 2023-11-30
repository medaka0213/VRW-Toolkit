
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// 子オブジェクトの平均近点角を均等にする
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Constellation : UdonSharpBehaviour
{
    GameObject[] childObj;
    void Start()
    {
        GameObject[] childObj = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++) {
            childObj[i] = transform.GetChild(i).gameObject;
            UdonBehaviour udonBehaviour =  (UdonBehaviour) childObj[i].GetComponent(typeof(UdonBehaviour));

            float mean_anomaly = 360f * i / childObj.Length;
            udonBehaviour.SetProgramVariable("mean_anomaly", mean_anomaly);
            udonBehaviour.SetProgramVariable("trailTimeMult", 1.2f/childObj.Length);
        }
    }
}
