
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AtmosphereSimu : UdonSharpBehaviour
{
    public float altitude = 0.0f;
    
    public MeshFilter SkyboxAir;
    public MeshFilter SkyboxStars;

    void Start()
    {
        
    }

    void Update()
    {
        float atmosphereThickness = CalcAtmosphereThickness(altitude);
        
        //マテリアルを取得する
        Material skyboxAir = SkyboxAir.GetComponent<MeshRenderer>().material;
        skyboxAir.SetFloat("_AtmosphereThickness", atmosphereThickness * 0.22f);

        //マテリアルの透明度を変更する
        Material skyboxStars = SkyboxStars.GetComponent<MeshRenderer>().material;
        Color color = skyboxStars.color;
        skyboxStars.SetColor("_Color", new Color(color.r, color.g, color.b, 1f-atmosphereThickness));
    }

    float CalcAtmosphereThickness(float altitude)
    {
        //参考: http://zakii.la.coocan.jp/physics/31_pressure.htm
        float maxHeight = 44.33514f; 
        float minHeight = 0.0f;
        if (altitude < minHeight)
        {
            return 1f;
        }
        else if (altitude > maxHeight)
        {
            return 0f;
        }
        else
        {
            float maxPress = 1013.25f;
            float press = Mathf.Pow((altitude - minHeight) / 11.880516f, 5.225877f);
            return (maxPress - press) / maxPress;
        }
    }
}
