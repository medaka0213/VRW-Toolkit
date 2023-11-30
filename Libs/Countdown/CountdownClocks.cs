using System;
using System.IO;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.StringLoading;

public class CountdownClocks : UdonSharpBehaviour
{
    [SerializeField] private VRCUrl _url;
    [SerializeField] public Text _text;

    private string loadedText = "";

    public bool isUpdated = false; //更新済みかどうか
    public bool isUpdateInProgress = false; //更新中かどうか

    public void Update()
    {
        if (!isUpdated && !isUpdateInProgress)
        {
            _text.text = "Loading...";
            LoadText();
        }
        if (isUpdated && _text)
        {
            _text.text = GetCountdown(loadedText);
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
        loadedText = result.Result;
        isUpdateInProgress = false;
        isUpdated = true;
    }
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.Log("Text Loading Failed" + result.Error);
        isUpdateInProgress = false;
        LoadText();
    }


    public string GetCountdown(string str)
    {
        string[] lines = str.Split('\n');
        string currentSection = "";
        string countdownString = "";

        foreach (var line in lines)
        {
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line.Trim('[', ']');
            }
            else
            {
                var keyValue = line.Split('=');
                if (keyValue[0] == "datetime")
                {
                    DateTime launchTime;
                    if (DateTime.TryParse(keyValue[1] + "Z", out launchTime))
                    {
                        TimeSpan timeRemaining = launchTime - DateTime.Now;
                        string timeSign = timeRemaining.TotalSeconds < 0 ? "+" : "-";
                        countdownString += $"{timeSign} {timeRemaining:dd\\ hh\\:mm\\:ss} ... {currentSection}\n";
                    }
                }
            }
        }
        return countdownString;
    }
}
