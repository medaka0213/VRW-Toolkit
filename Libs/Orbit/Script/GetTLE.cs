using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// HttpClient利用のため
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Http;

#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode] //SendMessageでエラーが出ないように
public class GetTLE : MonoBehaviour
{
    string TLERootURL = "http://www.celestrak.org/NORAD/elements/gp.php?CATNR=";
    public int satelliteID = 25544;
    public Text target;
    public bool isUpdate = false;

    //multiline
    [SerializeField]
    [Multiline]
    string TLE;

    public async void AsyncHttpGet()
    {
        string url = TLERootURL + satelliteID;

        Uri uriResult;
        Uri.TryCreate(url, UriKind.Absolute, out uriResult);

        HttpClient httpClient = new HttpClient();
        HttpRequestMessage request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uriResult
        };

        HttpResponseMessage response = await httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            //テキストを取得
            string text = await response.Content.ReadAsStringAsync();
            
            //テキストを表示
            if (target.gameObject.activeSelf && isUpdate)
            {
                target.text = text;
                TLE = text;
            }
        }
        else
        {
            Debug.LogError("HttpGet Error : " + response.StatusCode);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GetTLE))]//拡張するクラスを指定
public class GetTLEEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI();
        GetTLE getTLE = target as GetTLE;

        //ボタンを表示
        if (GUILayout.Button("Get TLE"))
        {
            Debug.Log("Getting TLE...");
            getTLE.SendMessage("AsyncHttpGet", null, SendMessageOptions.DontRequireReceiver);
        }
    }
}
#endif
