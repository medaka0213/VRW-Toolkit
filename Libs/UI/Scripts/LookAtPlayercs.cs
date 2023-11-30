using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

using VRC.SDKBase;
using VRC.Udon;

public class LookAtPlayercs : UdonSharpBehaviour
{    
    public Transform target;
    Text title;
    [SerializeField, TextArea]public string titleText;


    Text desc;
    [SerializeField, TextArea]public string descText;

    public float updateInterval = 0.5f; 
    private float updateTimer = 0f;

    void Start(){
        title = transform.Find("Canvas/Offset/TextTitle").GetComponent<Text>();
        desc = transform.Find("Canvas/Offset/TextDesc").GetComponent<Text>();

        // "___" で分けて、タイトルと詳細にする
        if (titleText == ""){
            string parantName = transform.parent.name;
            string splitter = "___";

            titleText = parantName.Split(splitter.ToCharArray())[0];

            if (parantName.Split(splitter.ToCharArray()).Length > 1){
                descText = parantName.Split(splitter.ToCharArray())[1];
            } else {
                descText = "";
            }
        }
    }

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        if (title){
            title.text = titleText;
        }

        if (desc){
            desc.text = descText;
        }
        
        if (target){
            this.transform.LookAt(target);
        } else {
            var player = Networking.LocalPlayer;
            if (player != null){
                var trackData = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
                this.transform.LookAt(trackData.position);
            }
        }
        
    }
}
