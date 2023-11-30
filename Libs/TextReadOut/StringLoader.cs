using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;

public class StringLoader : UdonSharpBehaviour
{
    [SerializeField] private VRCUrl _url;
    [SerializeField] private Text _text;

    public bool isUpdated = false; //更新済みかどうか
    public bool isUpdateInProgress = false; //更新中かどうか

    public void Update(){
        if (!isUpdated && !isUpdateInProgress) {
            LoadText();
        }
    }

    public void LoadText()
    {
        Debug.Log("Loading Text" + _url.Get());
        isUpdateInProgress = true;
        isUpdated = false;
        VRCStringDownloader.LoadUrl(_url, this.GetComponent<UdonBehaviour>());
    }
    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        _text.text = result.Result;
        isUpdateInProgress = false;
        isUpdated = true;
    }
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.Log("Text Loading Failed" + result.Error);
        isUpdateInProgress = false;
        LoadText();
    }
}