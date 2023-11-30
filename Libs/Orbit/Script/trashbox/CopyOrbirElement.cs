
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CopyOrbirElement : UdonSharpBehaviour
{
    public Orbit target_to;
    public Orbit target_from;

    void Start()
    {
        
    }

    void Update(){
        target_to.SetProgramVariable("epochTimeISO", target_from.epochTimeISO);
        target_to.SetProgramVariable("raan_deg", target_from.raan_deg);
        target_to.SetProgramVariable("mean_anomaly", target_from.mean_anomaly);
        target_to.SetProgramVariable("Ap", target_from.Ap);
        target_to.SetProgramVariable("Pe", target_from.Pe);
        target_to.SetProgramVariable("incl_deg", target_from.incl_deg);
        target_to.SetProgramVariable("aop_deg", target_from.aop_deg);
        target_to.SetProgramVariable("isCircular", target_from.isCircular);
        target_to.SetProgramVariable("isSunSync", target_from.isSunSync);
        target_to.SetProgramVariable("excludeOutliers", target_from.excludeOutliers);
        target_to.SetProgramVariable("calcCoordinate", target_from.calcCoordinate);
        target_to.SetProgramVariable("adjustRAANChanges", target_from.adjustRAANChanges);
    }
}
