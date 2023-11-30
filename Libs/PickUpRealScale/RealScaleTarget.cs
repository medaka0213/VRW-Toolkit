
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RealScaleTarget : UdonSharpBehaviour
{
    public Transform[] targetTransforms;
    
    void Start()
    {
        // targetTransforms ... 複数選択
        targetTransforms = new Transform[transform.Find("display").childCount];
        for (int i = 0; i < transform.Find("display").childCount; i++)
        {
            targetTransforms[i] = transform.Find("display").GetChild(i);
        }
    }
}
