
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class HIdeByLocation : UdonSharpBehaviour
{
    public float maxHeight = -50f;
    public float minHeight = -200f;

    float updateTimer = 0f;
    public float updateInterval = 0.1f;
    
    void Start()
    {
        
    }

    void Update ()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval)
        {
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate()
    {
        var player = Networking.LocalPlayer;
        if (player == null)
        {
            return;
        }
        var pos = player.GetPosition();
        if (pos.y > maxHeight || pos.y < minHeight)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
