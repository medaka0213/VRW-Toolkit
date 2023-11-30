
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SlideListAutoSwitch : UdonSharpBehaviour
{
    public UnityVideoPlayer mainSlide;
    public GameObject[] targetObjects;
    public VRCUrl targeturl;

    void Start()
    {
        
    }

    void Update(){
        CheckUrl();
    }

    public void CheckUrl(){
        if (mainSlide.url.Get() ==  targeturl.Get()){
            foreach (GameObject i in targetObjects) {
                i.SetActive (true);
            }
        } else {
            foreach (GameObject i in targetObjects) {
                i.SetActive (false);
            }
        }
    }
}
