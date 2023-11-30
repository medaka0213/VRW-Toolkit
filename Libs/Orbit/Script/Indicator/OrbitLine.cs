using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class OrbitLine : UdonSharpBehaviour
{
    public Orbit orbit;
    public bool is3D = true;
    
    public LineRenderer lineRenderer;
    public Material lineMaterial;

    public float lineStartPhase = 0f;
    public float lineEndPhase = 1f;

    public float updateInterval = 0.5f;
    [SerializeField]private float updateTimer = 0f;

    GameObject[] linePoints;

    void Start ()
    {
        //直下の子オブジェクトを取得
        linePoints = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) {
            linePoints[i] = transform.GetChild(i).gameObject;
        }

        if (!lineRenderer) lineRenderer = GetComponent<LineRenderer>();
        if (lineMaterial) lineRenderer.material = lineMaterial;
    }

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            Debug.Log("update");
            ManagedFixedUpdate();
        }
    }
    void ManagedFixedUpdate(){
        int n_iter = linePoints.Length;
        lineRenderer.positionCount = n_iter;
        for(int i = 0; i < n_iter; i++){
            float phase = Mathf.Lerp(lineStartPhase, lineEndPhase, (float)i / (n_iter - 1));
            linePoints[i].transform.localPosition = CalcPosition(phase);
            //日付変更線を超えていたら終了
            if (!is3D && i>0) {
                if ( Mathf.Abs(linePoints[i].transform.localPosition.z - linePoints[i-1].transform.localPosition.z) > 180) {
                    lineRenderer.positionCount = i;
                    break;
                }
            }
            lineRenderer.SetPosition(i, linePoints[i].transform.position);
        }
    }

    Vector3 CalcPosition(float phase){
        Vector3 pos = orbit.CalcPosition(orbit.elapsedTime, phase);
        if (!is3D) {
            pos = orbit.CalcLocation(pos);
            return new Vector3(pos.x, 0, pos.y);
        }
        return pos;
    }
}
