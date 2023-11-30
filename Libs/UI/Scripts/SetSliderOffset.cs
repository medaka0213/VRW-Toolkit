using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SetSliderOffset : UdonSharpBehaviour
{
    public Slider slider;
    public float offset;

    void Start()
    {
        
    }

    public void Minus(){
        slider.value -= offset;
    }

    public void Plus(){
        slider.value += offset;
    }
}
