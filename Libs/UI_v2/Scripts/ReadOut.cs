
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class ReadOut : UdonSharpBehaviour
{
    public float updateInterval = 1f; 
    private float updateTimer = 0f;
    public Text targetText;

    public UdonBehaviour target;
    public string targetVariableName;

    public string textPrefix;
    public string textSuffix;

    void Start()
    {
        targetText = this.GetComponent<Text>();
    }

    void Update(){
        updateTimer += Time.deltaTime;
        if (updateTimer > updateInterval){
            updateTimer = 0f;
            ManagedFixedUpdate();
        }
    }

    void ManagedFixedUpdate(){
        if (targetText){
            targetText.text = textPrefix + target.GetProgramVariable(targetVariableName) + textSuffix;
        }
    }
}
