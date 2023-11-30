using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Orbit : UdonSharpBehaviour
{
    [Header("軌道計算の有効化 (無効化で緯度/経度/高度から位置を計算する")]
    public bool enabled = true;

    [Header("2行要素 (TLE)")]
    public string TLELine0 = "";
    public string TLELine1 = "";
    public string TLELine2 = "";
    public Text TLEReferance;

    public int maxOutlierFound = 100;
    int n_outlierFound = 0;
    public int max_n_calc = 1000;

    [Header("元期: Epoch")]
    public string epochTimeISO = "1970-01-01T00:00:00Z";
    [Header("遠点高度Ap, 近点高度Pe")]
    public float Ap = 3f;
    public float Pe = 1f;

    [Header("平均近点角E, 軌道傾斜角I, 昇交点経度RAAN, 近地点引数AOP")]
    public float mean_anomaly = 0f;
    public float incl_deg = 0f;
    public float raan_deg = 0f;
    public float aop_deg = 0f;

    [Header("その他設定")]
    public Color trailColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public GravitySource GravitySource;

    public bool calcCoordinate = false;
    public bool adjustRAANChanges = false; //摂動を修正
    public bool isSunSync = false;
    public bool isCircular = false;
    public bool excludeOutliers = true; //外れ値の処理
    public float phase_offset_for_trace = 0f;
    public float trailTimeMult = 1.2f;
    public float semi_major_axis;
    public float semi_minor_axis;

    float mult_x_u;
    float mult_y_u;
    float mult_z_u;
    float mult_x_v;
    float mult_y_v;
    float mult_z_v;
    float focus;
    float eccentric_anomaly; //離心近点角

    TrailRenderer Trail;
    float otherObjectInfoTimer;
    float otherObjectInfoTimerMax = 1f;

    [Header("デバッグ用")]
    [SerializeField] public float eccentricity;
    [SerializeField] public double epochTime;
    [SerializeField] public float orbitalPeriod = 1f;
    [SerializeField] public double elapsedTime; // エポックからの経過時間
    [SerializeField] public float latitude = 0f;
    [SerializeField] public float longitude = 0f;
    [SerializeField] public float altitude = 420f;
    [SerializeField] public float azimuth = 0f;

    double GM;
    float radiusOfEarth;
    float playbackSpeed;
    float timePerYear;
    float timePerDay;
    bool isGalileoBeaten;

    void Start()
    {
        Trail = transform.Find("Trail").GetComponent<TrailRenderer>();
        UpdateOtherObjectInfo();
        if (isCircular) excludeOutliers = false;
    }
    void Update()
    {
        if (enabled)
        {
            // 円軌道の場合、遠点高度と近点高度を同じにする
            if (isCircular) Pe = Ap;
            UpdateOtherObjectInfoTimer();
            //ミッション経過時間を設定
            elapsedTime = GravitySource.elapsedTime - epochTime;
            gameObject.transform.localPosition = CalcPosition(elapsedTime, phase_offset_for_trace);
            if (calcCoordinate) CalcCoordinate();
        }
        else
        {
            if (Trail) Trail.time = 0f;
            //軌道計算が無効化されている場合、緯度/経度/高度から位置を計算する
            CalcLocationInverse();
        }
    }

    void onEnable()
    {
        UpdateOtherObjectInfo();
    }

    void UpdateOtherObjectInfoTimer()
    {
        otherObjectInfoTimer -= Time.deltaTime;
        if (otherObjectInfoTimer < 0f)
        {
            otherObjectInfoTimer = otherObjectInfoTimerMax;
            UpdateOtherObjectInfo();
        }
    }

    void UpdateOtherObjectInfo()
    {
        ParseTLE();
        CalcOrbitalInfo();

        if (Trail)
        {
            Trail.time = orbitalPeriod * trailTimeMult / playbackSpeed;
            Trail.startColor = trailColor;
            Trail.endColor = trailColor;
        }
        epochTime = (double)DateTimeOffset.Parse(epochTimeISO).ToUnixTimeMilliseconds() / 1000d;

        GM = GravitySource.GM;
        radiusOfEarth = GravitySource.radius;
        playbackSpeed = GravitySource.playbackSpeed;
        timePerYear = GravitySource.orbitalPeriod;
        timePerDay = GravitySource.rotationPeriod;
        isGalileoBeaten = GravitySource.isGalileoBeaten;
    }

    void CalcOrbitalInfo()
    {
        //軌道要素を計算する
        float raan = raan_deg / 180f * Mathf.PI;
        float incl = incl_deg / 180f * Mathf.PI;
        float aop = aop_deg / 180f * Mathf.PI;

        //時間毎の変化量を補正
        if (adjustRAANChanges)
        {
            float mult = 0.174f / Mathf.Pow(semi_major_axis / radiusOfEarth, 3.5f) * (float)(elapsedTime / timePerDay);
            aop += (2f - 2.5f * Mathf.Pow(Mathf.Sin(incl), 2f)) * mult;
            raan -= Mathf.Cos(incl) * mult;
        }

        mult_x_u = Mathf.Cos(raan) * Mathf.Cos(aop) - Mathf.Sin(raan) * Mathf.Cos(incl) * Mathf.Sin(aop);
        mult_x_v = -Mathf.Cos(raan) * Mathf.Sin(aop) - Mathf.Sin(raan) * Mathf.Cos(incl) * Mathf.Cos(aop);
        mult_y_u = Mathf.Sin(incl) * Mathf.Sin(aop);
        mult_y_v = Mathf.Sin(incl) * Mathf.Cos(aop);
        mult_z_u = Mathf.Cos(raan) * Mathf.Cos(incl) * Mathf.Sin(aop) + Mathf.Sin(raan) * Mathf.Cos(aop);
        mult_z_v = Mathf.Cos(raan) * Mathf.Cos(incl) * Mathf.Cos(aop) - Mathf.Sin(raan) * Mathf.Sin(aop);

        semi_major_axis = (Ap + Pe) / 2f + radiusOfEarth;
        focus = semi_major_axis - Pe - radiusOfEarth;
        semi_minor_axis = (float)(Mathf.Sqrt(Mathf.Pow(semi_major_axis, 2f) - Mathf.Pow(focus, 2f)));
        eccentricity = (float)(Mathf.Sqrt(Mathf.Pow(semi_major_axis, 2f) - Mathf.Pow(semi_minor_axis, 2f)) / semi_major_axis);
        orbitalPeriod = 2f * Mathf.PI * semi_major_axis / Mathf.Sqrt((float)GM / (float)1e+9 / semi_major_axis);
    }

    public Vector3 CalcPosition(double _elapsedTime, float _phase_offset_for_trace)
    {
        if (_elapsedTime == 0) _elapsedTime = elapsedTime;

        //経過時間から離心近点角を計算する
        _elapsedTime += _phase_offset_for_trace * orbitalPeriod;
        double revs = _elapsedTime / (double)orbitalPeriod + (double)mean_anomaly / 360d;
        float _eccentric_anomaly = SolveKepler(2 * Mathf.PI * (float)(revs % 1d));

        //軌道要素を計算する
        float raan = raan_deg / 180f * Mathf.PI;
        float incl = incl_deg / 180f * Mathf.PI;
        float aop = aop_deg / 180f * Mathf.PI;

        float stm_major = semi_major_axis * Mathf.Cos(_eccentric_anomaly) - semi_major_axis * eccentricity;
        float stm_minor = semi_major_axis * Mathf.Sin(_eccentric_anomaly) * Mathf.Sqrt(1f - Mathf.Pow(eccentricity, 2f));

        Vector3 pos = new Vector3(
            stm_major * mult_x_u + stm_minor * mult_x_v,
            stm_major * mult_y_u + stm_minor * mult_y_v,
            stm_major * mult_z_u + stm_minor * mult_z_v
        );

        if (isGalileoBeaten)
        {
            float earthRotation = -CalcEarthRotationRevs(_elapsedTime + epochTime) * 2 * Mathf.PI;
            pos = new Vector3(
                pos.x * Mathf.Cos(earthRotation) - pos.z * Mathf.Sin(earthRotation),
                pos.y,
                pos.x * Mathf.Sin(earthRotation) + pos.z * Mathf.Cos(earthRotation)
            );
        }

        //1フレームあたりの移動量から、外れ値を計算
        //float deltaPosLimit = 2f * Mathf.PI * semi_major_axis / orbitalPeriod * Time.deltaTime * playbackSpeed;
        // と、やるのがなんか無理だったので適当にしろ
        float deltaPosLimit = semi_major_axis;
        if (excludeOutliers)
        {
            if ((gameObject.transform.localPosition - pos).magnitude > deltaPosLimit)
            {
                if (n_outlierFound > maxOutlierFound)
                {
                    n_outlierFound = 0;
                }
                else
                {
                    n_outlierFound += 1;
                    return pos;
                }
            }
        }
        return pos;
    }

    float SolveKepler(float mean_anomaly)
    {
        float eccentric_anomaly;

        if (eccentricity < .1)
        {
            return mean_anomaly;
        }

        eccentric_anomaly = mean_anomaly * 2f;
        float error = mean_anomaly;
        float min_error = 1f / 360f * Mathf.PI * 2f; //誤差1度以下なら何でも良いと思う
        int n_calc = 0;

        while (Mathf.Abs(error) > min_error && n_calc < max_n_calc)
        {
            error = mean_anomaly - eccentric_anomaly + eccentricity * Mathf.Sin(eccentric_anomaly);
            eccentric_anomaly -= error / (eccentricity * Mathf.Cos(eccentric_anomaly) - 1f);
            n_calc += 1;
        }
        return eccentric_anomaly;
    }

    float CalcEarthRotationRevs(double time_seconds)
    {
        double epoch = (double)DateTimeOffset.Parse("2000-01-01T12:00:00+00:00").ToUnixTimeMilliseconds() / 1000d;
        double revs = 0.7790572732640 + (time_seconds - epoch) / GravitySource.rotationPeriod;
        return (float)(revs % 1d);
    }

    public Vector3 CalcLocation(Vector3 pos)
    {
        //緯度・経度・高度の計算
        float altitude = pos.magnitude - radiusOfEarth;
        float latitude = Mathf.Asin(pos.y / pos.magnitude) * 180f / Mathf.PI;
        float longitude = Mathf.Atan2(pos.z, pos.x) * 180f / Mathf.PI;
        if (longitude > 180f)
        {
            longitude -= 360f;
        }
        else if (longitude < -180f)
        {
            longitude += 360f;
        }
        return new Vector3(latitude, longitude, altitude);
    }

    public float CalcAzimuth(double _elapsedTime, float _phase_offset_for_trace)
    {
        Vector3 location_from = CalcLocation(CalcPosition(_elapsedTime, _phase_offset_for_trace));
        Vector3 location_to = CalcLocation(CalcPosition(_elapsedTime, _phase_offset_for_trace + 0.01f));

        float y1 = location_from.x / 180f * Mathf.PI;
        float x1 = location_from.y / 180f * Mathf.PI;
        float y2 = location_to.x / 180f * Mathf.PI;
        float x2 = location_to.y / 180f * Mathf.PI;
        return 90f - Mathf.Atan2(
            Mathf.Cos(y1) * Mathf.Tan(y2) - Mathf.Sin(y1) * Mathf.Cos(x2 - x1),
            Mathf.Sin(x2 - x1)
        ) * 180f / Mathf.PI;
    }

    public void CalcLocationInverse()
    {
        //緯度・経度・高度から座標を計算 (CalcLocationの逆関数)
        float _latitude = latitude / 180f * Mathf.PI;
        float _longitude = longitude / 180f * Mathf.PI;
        float _altitude = altitude + radiusOfEarth;
        Vector3 pos = new Vector3(
            _altitude * Mathf.Cos(_latitude) * Mathf.Cos(_longitude),
            _altitude * Mathf.Sin(_latitude),
            _altitude * Mathf.Cos(_latitude) * Mathf.Sin(_longitude)
        );
        transform.localPosition = pos;
    }

    public bool isGeoStationary()
    {
        return Mathf.Abs(eccentricity) < 0.0001f && Mathf.Abs(incl_deg) < 1f && Mathf.Abs(orbitalPeriod - GravitySource.rotationPeriod) < 100f;
    }

    public void CalcCoordinate()
    {
        Vector3 pos = transform.localPosition;
        Vector3 location = CalcLocation(pos);
        latitude = location.x;
        longitude = location.y;
        altitude = location.z;
        if (this.isGeoStationary())
        {
            azimuth = 0f;
        }
        else
        {
            azimuth = CalcAzimuth(elapsedTime, phase_offset_for_trace);
        }
    }

    //TLEを処理
    public void ParseTLE()
    {
        if (TLEReferance)
        {
            SetTLE(TLEReferance.text);
        }
        if (string.IsNullOrEmpty(TLELine1) || string.IsNullOrEmpty(TLELine2))
        {
            return;
        }
        // 直接書かれてるやつ
        incl_deg = float.Parse(TLELine2.Substring(8, 16 - 8));
        raan_deg = float.Parse(TLELine2.Substring(17, 25 - 17));
        aop_deg = float.Parse(TLELine2.Substring(34, 42 - 34));
        mean_anomaly = float.Parse(TLELine2.Substring(43, 51 - 43));

        float mean_motion = float.Parse(TLELine2.Substring(52, 63 - 52));
        orbitalPeriod = 1f / mean_motion * 24f * 3600f; //軌道周期[s]
        eccentricity = float.Parse("0." + TLELine2.Substring(26, 33 - 26)); //離心率
        semi_major_axis = Mathf.Pow((float)(GravitySource.GM / 1e+9d) / Mathf.Pow(2 * Mathf.PI, 2f) * Mathf.Pow(orbitalPeriod, 2f), .333333333f);

        Ap = semi_major_axis * (1f + eccentricity) - GravitySource.radius;
        Pe = semi_major_axis * (1f - eccentricity) - GravitySource.radius;

        //元期年: 1957年を基準に上2桁を推定
        int epoch_time_years = int.Parse(TLELine1.Substring(18, 20 - 18));
        float epoch_time_days = float.Parse(TLELine1.Substring(20, 32 - 20));

        if (epoch_time_years < 57)
        {
            epoch_time_years += 2000;
        }
        else
        {
            epoch_time_years += 1900;
        }

        DateTimeOffset epoch_time = DateTimeOffset.FromUnixTimeSeconds(0);
        epoch_time = epoch_time.AddYears(epoch_time_years - 1970);
        epoch_time = epoch_time.AddDays(epoch_time_days - 1);
        epochTimeISO = epoch_time.ToString("o");
    }

    public void SetTLE(string TLE)
    {
        char[] separator = new char[] { '\n' };
        string[] splitted = TLE.Split(separator);
        TLELine0 = splitted[0].Trim();
        TLELine1 = splitted[1].Trim();
        TLELine2 = splitted[2].Trim();
    }
}
