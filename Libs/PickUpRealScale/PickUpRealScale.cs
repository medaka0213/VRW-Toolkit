
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PickUpRealScale : UdonSharpBehaviour
{
    public GameObject body;
    public GameObject dummy;
    public Vector3 offset = new Vector3(0, 0, -1); 
    public float minScale = .01f;
    public float maxScale = 1f;

    [UdonSynced, FieldChangeCallback(nameof(scale))] private float _scale;
    public float scale
    {
        private set
        {
            _scale = value;
            if (value < minScale) value = minScale;
            if (value > maxScale) value = maxScale;

            if (dummy != null)
            {
                if (value < minScale * 2f)
                {
                    dummy.SetActive(false);
                }
                else
                {
                    dummy.SetActive(true);
                }
            }
            
            if (body != null)
            {
                body.transform.localScale = new Vector3(value, value, value);
                body.transform.localPosition = offset * (value - minScale);
            }
        }
        get => _scale;
    }

    void Start()
    {
        body.SetActive(true);
        dummy.SetActive(false);
    }

    void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal)
        {
            RequestSerialization();
        }
    }

    public void Minimize()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        scale = minScale;
        RequestSerialization();
    }
    
    public void Maximize()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        scale = maxScale;
        RequestSerialization();
    }

    public void Toggle()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        scale = scale == minScale ? maxScale : minScale;
        RequestSerialization();
    }
}
