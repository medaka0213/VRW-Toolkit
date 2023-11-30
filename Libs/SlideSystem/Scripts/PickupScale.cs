
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

public class PickupScale : UdonSharpBehaviour
{
    [UdonSynced]
    public float scale = 1f;
    public float minScale = 1f;
    public float maxScale = 10f;
    public float offset = 0.1f;
    
    public Text dialogCurrentScale;
    public Text dialogMaxScale;
    public Transform parent; 

    void Start()
    {
        //dialogCurrentScale = transform.Find("Text-Size").GetComponent<Text>();
        //dialogMaxScale = transform.Find("Text-MaxSize").GetComponent<Text>();
        //parent = transform.Find("Parent").GetComponent<Transform>();

        UpdateUI();
    }

    void Update() {
        UpdateScale();
        UpdateUI();
    }

    void UpdateUI() {
        dialogCurrentScale.text = "Scale: x" + scale.ToString("G4");
        dialogMaxScale.text = "Max: x" + maxScale;
    }

    void UpdateScale() {
        parent.localScale = new Vector3(scale, scale, scale);
    }

    void _Scale (float _offset){
        scale *= 1f + _offset;

        if (scale > maxScale){ 
            scale = maxScale;
        } 
        else if (scale < minScale) {
            scale = minScale;
        }
    }

    public void ScaleUp() {
        takeOwnership();
        _Scale(offset);
    }

    public void ScaleDown() {
        takeOwnership();
        _Scale(-offset);
    }

    //オーナー判定
    private bool isOwner() => Networking.GetOwner(gameObject) == Networking.LocalPlayer;

    public void takeOwnership(){
        if (!isOwner()){
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }
}