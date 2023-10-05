using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;

namespace VRR.HoloNav.Examples
{
    [Serializable]
    public class MyArray
    {
        public double[] lat_Lon;
    }


    public class GPSToUnityPostion : MonoBehaviour
    {
        [SerializeField] TelemetrySource telemetry;

        //private Vector2 OriginGPS;
        private double OriginLat_Double;
        private double OriginLon_Double;
        //private Vector2 CurrentGPS;
        private double CurrentLat_Double;
        private double CurrentLon_Double;
        private double CurrentEle_Double;
        //private List<double> GPS data for "Kalman Filter" estimation at the beginning
        private Queue<double> LatQueue = new Queue<double>();
        private Queue<double> LonQueue = new Queue<double>();
        private Queue<double> EleQueue = new Queue<double>();

        private double Kalman_CurrentLat_Double;
        private double Kalman_CurrentLon_Double;
        private double Kalman_CurrentEle_Double;

        //get the elevation of Origin Position through API as a standard -> to normalize other cues' height
        private double Elevation_Double;

        [Header("Set Bearing")]
        private float CurrentBearing;
        public float OriginBearing;
        [SerializeField] private GameObject NodesParents;

        private IEnumerator GPSIEnumerator;
        private bool GPSReceived = false;
        private bool GPSReceived_UnityEditor = true;
        private bool BearingReceived = false;
        private bool BearingReceived_UnityEditor = false;

        /*The cooresponding GPS and Unity Coordinate*/
        [Header("Set GPS")]
        public Transform[] nodesCoordinate;
        public MyArray[] nodesGPS; // nodesGPS[row].lat_Lon[column]

        /*Add/delete markers in real time & save GPS info*/
        [Header("Add/Delete Marker")]
        public GameObject RealTimeMarker; //Marker Prefab
        [SerializeField] Transform MarkersParent;
        private List<List<string>> RealTimeGPS;
        private string FileName_RealTimeMarker = "RealTimeMarker.txt";
        public int markerId; //pass from MarkerID.id
        private int lineIndex; //calculate the line index to delete
        int NewID = -1; //incremented new id - add to the end of the file 

        /*Recording Data*/
        [Header("Data Recording")]
        //RecordData recorder;
        private string FileName;
        [SerializeField] private TMP_InputField FileNameInputField;

        /*CompassBar*/
        [Header("Compass")]
        public QuantumTek.QuantumTravel.CompassBar compassBar;

        /*Kalman Filter*/
        [Header("Kalman Filter")]
        [SerializeField] private bool gpsKalmanFilter = false;
        [SerializeField] private int KalmanFilterDataCount = 0;

        /*Update the height of AR cues to appear at the eye height of users*/
        [Header("Dynamic AR Cue Height")]
        [Tooltip("If updating the height of AR cues to appear at the eye height of users")]
        public bool UseEyeHeight;
        [Tooltip("The interval to check MainCamera height in seconds")]
        public float IntervalCheckHeight;
        [Tooltip("The amount of difference to adjust the cues' height in meters")]
        public float ThresholdChangeHeight;
        private float MainCameraHight = 0f;

        void Awake()
        {
            CurrentBearing = 0f;
            OriginBearing = 0f;
            /*Default Starting Point: FGH at Vanderbilt*/
            OriginLat_Double = 36.144752;
            OriginLon_Double = -86.803193;
            //GPSEncoder.SetLocalOrigin(OriginGPS);
            GPSEncoder_Double.SetLocalOrigin(OriginLat_Double, OriginLon_Double);

            //Default Elevation_Double in case no sensor data read in
            Elevation_Double = 170;

            nodesGPS[0].lat_Lon[0] = OriginLat_Double;
            nodesGPS[0].lat_Lon[1] = OriginLon_Double;
            nodesGPS[0].lat_Lon[2] = Elevation_Double;

            CurrentLat_Double = OriginLat_Double;
            CurrentLon_Double = OriginLon_Double;
            CurrentEle_Double = Elevation_Double;
    }

        // Start is called before the first frame update
        void Start()
        {
            telemetry.DataReceivedEvent += OnDataReceived;
            /*set up Unity Coordinates according to GPS*/
            if (GPSIEnumerator != null)
                StopCoroutine(GPSIEnumerator);
            GPSIEnumerator = SetCoordinates();
            StartCoroutine(GPSIEnumerator);

            /*Calibrate North*/
            //StartCoroutine(AutoNorthCalibration());

            /*Record HoloNav Data*/
            //StartCoroutine(RecordHoloNavData(FileName));

            /*Update NodeParents Height*/
            StartCoroutine(UpdateHeight());

        }

        // Update is called once per frame
        void Update()
        {
            /*Only For Unity Editor Testing*/
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GPSReceived_UnityEditor = !GPSReceived_UnityEditor;
                BearingReceived_UnityEditor = !BearingReceived_UnityEditor;
            }
        }
        private IEnumerator SetCoordinates()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
#if UNITY_EDITOR
                if (GPSReceived_UnityEditor)
                {
                    Debug.Log("GPS ready");
                    
                    //Set Unity Coordinates of every Nodes
                    SetNodesCoordinates();

                    //Set Unity Coordinates of every Real Time Marker
                    SetRealTimeGPSCoordinates();

                    Debug.Log("Objects Localized");
                    break;
                }
#endif
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
                if (GPSReceived)
                {    
                    if(!gpsKalmanFilter)
                    {
                        if(GPSReceived)                    
                        { 
                            OriginLat_Double = CurrentLat_Double;
                            OriginLon_Double = CurrentLon_Double;

                            GetElevation(CurrentEle_Double);
                            
                            //Set Unity Coordinates of every Nodes
                            SetNodesCoordinates();

                            //Set Unity Coordinates of every Real Time Marker
                            SetRealTimeGPSCoordinates();
                            
                            break;
                        }
                    }

                    //initialize the scene until already collecting the Kalman Filter Data
                    else if(gpsKalmanFilter)
                    {
                        if(LatQueue.Count>=KalmanFilterDataCount && LonQueue.Count >= KalmanFilterDataCount && EleQueue.Count >= KalmanFilterDataCount)                    
                        {
                            OriginLat_Double = KalmanFilter(LatQueue);
                            OriginLon_Double = KalmanFilter(LonQueue);
                            Elevation_Double = KalmanFilter(EleQueue);

                            //record kalman filter prediction
                            RecordData.SaveData("KalmanPrediction.txt", System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") + "," + OriginLat_Double.ToString() + "," + OriginLon_Double.ToString() + "," + Elevation_Double.ToString()+ "," + CurrentBearing.ToString() + "\n");                

                            //Set Unity Coordinates of every Nodes
                            SetNodesCoordinates();

                            //Set Unity Coordinates of every Real Time Marker
                            SetRealTimeGPSCoordinates();

                            break;
                        } 
                    }                       
                }
#endif
            }
        }

        private void SetNodesCoordinates()
        {
            /*Set Origin*/
            GPSEncoder_Double.SetLocalOrigin(OriginLat_Double, OriginLon_Double);

            /*Get elevation of the Origin GPS*/
            //StartCoroutine(GetElevation(OriginLat_Double, OriginLon_Double));

            /*Set Unity Coordinates for all AR Cues*/
            nodesGPS[0].lat_Lon[0] = OriginLat_Double;
            nodesGPS[0].lat_Lon[1] = OriginLon_Double;

            for (int i = 1; i < nodesCoordinate.Length; i++)
            {
                /*Set x and z*/
                double Lat = nodesGPS[i].lat_Lon[0];
                double Lon = nodesGPS[i].lat_Lon[1];
                nodesCoordinate[i].localPosition = GPSEncoder_Double.GPSToUCS_Double(Lat, Lon);

                /*Set y*/
                var position = nodesCoordinate[i].localPosition;
                if (UseEyeHeight)
                {
                    /*Set y based on the height of MainCamera*/
                    nodesCoordinate[i].localPosition = new Vector3(position.x, Camera.main.transform.position.y, position.z);
                }
                else
                {
                    /*Set y based on the elevation of start position*/
                    nodesCoordinate[i].localPosition = new Vector3(position.x, position.y + (float)(nodesGPS[i].lat_Lon[2] - Elevation_Double), position.z);
                }
            }
        }

        private void SetRealTimeGPSCoordinates()
        {
            /*Read markers' GPS from file to RealTimeGPS*/
            string[] TestReadin = RecordData.LoadData<string[]>(FileName_RealTimeMarker);

            /*If the file does not exist*/
            if (TestReadin == default(string[]))
            {
                Debug.Log("<color=green>TestReadin - null</color>");
                /*Create File and the first line*/
                if (!String.IsNullOrEmpty(FileName_RealTimeMarker))
                {
                    RecordData.SaveData(FileName_RealTimeMarker, "Time" + ","
                    + "ID" + ","
                    + "Kalman_CurrentLat_Double" + ","
                    + "Kalman_CurrentLon_Double" + ","
                    + "Kalman_CurrentEle_Double" + ","
                    + "CurrentLat_Double" + ", "
                    + "CurrentLon_Double" + ","
                    + "CurrentEle_Double"
                    + "\n");
                }
                RealTimeGPS = new List<List<string>>();
                Debug.Log("<color=red>RealTimeGPS Count: </color>" + RealTimeGPS.Count);
                NewID = 0;
            }
            /*else the file exists*/
            else
            {
                RealTimeGPS = new List<List<string>>(); // 0:number 1:lattitude 2: longitude 3:elevation
                int idx = 0;
                foreach (string line in TestReadin)
                {
                    idx++;
                    if (idx == 1)
                        continue;
                    //Debug.Log(line);
                    string input = line;
                    string pattern = ",";
                    string[] substrings = Regex.Split(input, pattern);    // Split on hyphens
                    List<string> List_inside = new List<string>();
                    foreach (string match in substrings)
                    {
                        //Debug.Log(match);
                        List_inside.Add(match);
                    }
                    RealTimeGPS.Add(List_inside);
                }

                Debug.Log(RealTimeGPS.Count);

                /*Instantiate markers according to the GPS*/
                for (int i = 0; i < RealTimeGPS.Count; i++)
                {
                    /*Set x and z*/
                    double result;
                    double Lat = 0;
                    if (double.TryParse(RealTimeGPS[i][2], out result))
                        Lat = result;
                    double Lon = 0;
                    if (double.TryParse(RealTimeGPS[i][3], out result))
                        Lon = result;
                    GameObject marker = Instantiate(RealTimeMarker);
                    marker.transform.localPosition = GPSEncoder_Double.GPSToUCS_Double(Lat, Lon);

                    /*Set y*/
                    var position = marker.transform.localPosition;
                    if (UseEyeHeight)
                    {
                        /*Set y based on the height of MainCamera*/
                        marker.transform.localPosition = new Vector3(position.x, Camera.main.transform.position.y, position.z);
                    }
                    else
                    {
                        /*Set y based on the elevation of start position*/
                        double ele = 0;
                        if (double.TryParse(RealTimeGPS[i][4], out result))
                            ele = result;
                        //marker.transform.position = new Vector3(position.x, Camera.main.transform.position.y + (float)(ele - Elevation_Double), position.z);
                        marker.transform.localPosition = new Vector3(position.x, Camera.main.transform.position.y + (float)(ele - Elevation_Double), position.z);
                    }

                    /*Set Active*/
                    marker.SetActive(true);

                    /*Set Parent*/
                    marker.transform.SetParent(MarkersParent, false);

                    /*Set id by reading from file -- RealTimeGPS[i][1]*/
                    Int32 resultINT;
                    Int32 ReadinID = 0;
                    if (Int32.TryParse(RealTimeGPS[i][1], out resultINT))
                        ReadinID = resultINT;
                    marker.GetComponent<MarkerID>().id = ReadinID;
                }
            }

        }

        //call when click on "Add Marker"
        public void SaveMarkerData()
        {
            /*1. Instantiate markers in current GPS*/
            GameObject marker = Instantiate(RealTimeMarker, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z+0.5f), Quaternion.identity);
            var position = marker.transform.position;
            marker.transform.position = new Vector3(position.x, Camera.main.transform.position.y, position.z);
            marker.SetActive(true);
            marker.transform.SetParent(MarkersParent, true);

            if (NewID == -1 && RealTimeGPS.Count!=0)
                NewID = int.Parse(RealTimeGPS[RealTimeGPS.Count - 1][1]) + 1;
            else
                NewID = NewID + 1;
            
            marker.GetComponent<MarkerID>().id = NewID;
            

            /*2. Save data to the file*/
            if (!String.IsNullOrEmpty(FileName_RealTimeMarker))
            {
                RecordData.SaveData(FileName_RealTimeMarker, System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") + ","
                + NewID.ToString() + ","
                + Kalman_CurrentLat_Double.ToString() + ","
                + Kalman_CurrentLon_Double.ToString() + ","
                + Kalman_CurrentEle_Double.ToString() + ","
                + CurrentLat_Double.ToString() + ","
                + CurrentLon_Double.ToString() + ","
                + CurrentEle_Double.ToString()
                + "\n");
            }

            /*3. Update List<List<string>>RealTimeGPS*/
            List<string> List_inside = new List<string>();
            List_inside.Add(System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"));
            List_inside.Add(NewID.ToString());
            List_inside.Add(Kalman_CurrentLat_Double.ToString());
            List_inside.Add(Kalman_CurrentLon_Double.ToString());
            List_inside.Add(Kalman_CurrentEle_Double.ToString());
            List_inside.Add(CurrentLat_Double.ToString());
            List_inside.Add(CurrentLon_Double.ToString());
            List_inside.Add(CurrentEle_Double.ToString());
            RealTimeGPS.Add(List_inside);
            Debug.Log("<color=red>RealTimeGPS Count: </color>" + RealTimeGPS.Count);
        }

        //call when click on "Delete Marker"
        public void DeleteMarkerData()
        {
            if (!String.IsNullOrEmpty(FileName_RealTimeMarker))
            {
                Debug.Log("<color=red>RealTimeGPS Count BEFORE matching: </color>" + RealTimeGPS.Count);
                for (int i = 0; i < RealTimeGPS.Count; i++)
                {
                    Debug.Log("markerId"+markerId);
                    Debug.Log("RealTimeGPS[i][1]"+int.Parse(RealTimeGPS[i][1]));
                    //the id of marker is passed from MarkerID.PassID() when click/active the marker
                    if (markerId == int.Parse(RealTimeGPS[i][1]))
                    {
                        lineIndex = i + 1; // miss the 1st line
                        Debug.Log("<color=red>lineIndex: </color>" + lineIndex);
                        RecordData.DeleteLine(FileName_RealTimeMarker, lineIndex);
                        RealTimeGPS.RemoveAt(i);
                        Debug.Log("<color=red>RealTimeGPS Count: </color>" + RealTimeGPS.Count);
                        break;
                    }
                    else
                        Debug.Log("<color=red>markerId does not match </color>");
                }
            }
        }

        private IEnumerator UpdateHeight()
        {
            while (true)
            {
                yield return new WaitForSeconds(IntervalCheckHeight);
                float difference_y = Camera.main.transform.position.y - MainCameraHight;
                if (Math.Abs(difference_y) > ThresholdChangeHeight)
                {
                    var position = NodesParents.transform.position;
                    NodesParents.transform.position = new Vector3(position.x, position.y + difference_y, position.z);
                    MainCameraHight = Camera.main.transform.position.y;
                }
            }
        }



        private IEnumerator RecordHoloNavData(string FileName)
        {
            while (true)
            {
                //yield return null;
                yield return new WaitForSeconds(0.5f);
                if (GPSReceived)
                {
                    if(!String.IsNullOrEmpty(FileName))
                    {
                        RecordData.SaveData(FileName, System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") + ","
                        + CurrentLat_Double.ToString() + ","
                        + CurrentLon_Double.ToString() + ","
                        + CurrentEle_Double.ToString() + ","
                        + CurrentBearing.ToString()
                        + "\n");
                    }
                }
            }
        }

        public void SetFileName()
        {
            FileName = FileNameInputField.text+".txt";
            string createText = "Time,Latitude,Longitude,Altitude,Speed,Satellites,Bearing,Pitch,Roll" + Environment.NewLine;
            RecordData.SaveData(FileName, createText);
        }

        public IEnumerator AutoNorthCalibration()
        {
            while (true)
            {
                yield return null;
                if (BearingReceived || BearingReceived_UnityEditor)
                {
                    if (compassBar.CompassNorthAlign)
                    {
                        NorthCalibration();
                        break;
                    }
                }
            }
        }

        public void NorthCalibration()
        {
            OriginBearing = CurrentBearing;
            NodesParents.transform.Rotate(0.0f, -OriginBearing, 0.0f, Space.World);
        }

        public void NorthCalibrationforCompassBar()
        {
            //notice CompassBar.OriginBearing --> to change its rotation according to the Bearing Data. (no NEED for calling "NorthCalibrate")
            OriginBearing = CurrentBearing;
        }



        void OnDataReceived(object sender, TelemetryReceievedEventArgs args)
        {

            CurrentBearing = args.Data.Bearing;
            BearingReceived = true;

            // Verify position lock.
            if (args.Data.Locked)
            {
                if (!GPSReceived)
                    GPSReceived = true;

                CurrentLat_Double = args.Data.Latitude;
                CurrentLon_Double = args.Data.Longitude;
                CurrentEle_Double = args.Data.Altitude;

                //record data if not null or an Empty string.
                if (!String.IsNullOrEmpty(FileName))
                {
                    RecordData.SaveData(FileName, System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss") + ","
                    + CurrentLat_Double.ToString() + ","
                    + CurrentLon_Double.ToString() + ","
                    + CurrentEle_Double.ToString() + ","
                    + args.Data.Speed.ToString() + ","
                    + args.Data.Satellites + ","
                    + CurrentBearing.ToString() + ","
                    + args.Data.Pitch.ToString() + ","
                    + args.Data.Roll.ToString()
                    + "\n");
                }

                //create Kalman Filter Data
                if (gpsKalmanFilter)
                {
                    if (LatQueue.Count <= KalmanFilterDataCount)
                        LatQueue.Enqueue(CurrentLat_Double);
                    else
                    {
                        LatQueue.Dequeue();
                        LatQueue.Enqueue(CurrentLat_Double);
                        Kalman_CurrentLat_Double = KalmanFilter(LatQueue);
                    }
                    if (LonQueue.Count <= KalmanFilterDataCount)
                        LonQueue.Enqueue(CurrentLon_Double);
                    else
                    {
                        LonQueue.Dequeue();
                        LonQueue.Enqueue(CurrentLon_Double);
                        Kalman_CurrentLon_Double = KalmanFilter(LonQueue);
                    }
                    if (EleQueue.Count <= KalmanFilterDataCount)
                        EleQueue.Enqueue(CurrentEle_Double);
                    else
                    {
                        EleQueue.Dequeue();
                        EleQueue.Enqueue(CurrentEle_Double);
                        Kalman_CurrentEle_Double = KalmanFilter(EleQueue);
                    }
                }

            }
            else
            {
                if (GPSReceived)
                    GPSReceived = false;
            }
        }


        public Tuple<double, double> KalmanFilter_1D(double x, double P, double meas, double R)
        {
            double Q = 10e-5; //Q: Process Noise Covariance
            double A = 1;
            double H = 1;

            double K = (P * H) / (H * P * H + R);
            x = x + K * (meas - H * x);
            P = (1 - K * H) * P;

            x = A * x;
            P = A * P * A + Q;

            return Tuple.Create(x, P);
        }

        //return 1D "Latitude" /  1D "Longitude
        public double KalmanFilter(List<double> data)
        {
            double x = data[0]; //x:state
            double P = 1; //P: uncertainty covariance
            double R = Math.Pow(10, 2); //10**2 //R: measurement noise
            List<double> observed_x = data;
            List<double> result = new List<double>();

            for (int i = 0; i < observed_x.Count; i++)
            {
                (x, P) = KalmanFilter_1D(x, P, observed_x[i], R);
                result.Add(x);
            }

            return x;
        }

        public double KalmanFilter(Queue<double> data)
        {
            double x = data.Peek(); //x:state
            double P = 1; //P: uncertainty covariance
            double R = Math.Pow(10, 2); //10**2 //R: measurement noise
            List<double> observed_x = data.ToList();
            Queue<double> result = new Queue<double>();

            for (int i = 0; i < observed_x.Count; i++)
            {
                (x, P) = KalmanFilter_1D(x, P, observed_x[i], R);
                result.Enqueue(x);
            }

            return x;
        }

        //read from HoloNav Kit
        public void GetElevation(double Data_Elevation)
        {
            Elevation_Double = Data_Elevation;
        }

        //read from API
        public IEnumerator GetElevation(double Lat, double Lon)
        {
            var www = new UnityWebRequest("https://api.opentopodata.org/v1/ned10m?locations=" + Lat.ToString() + "," + Lon.ToString())
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
            yield return www.SendWebRequest();
            //if (www.isNetworkError || www.isHttpError)
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Network Error");
                yield break;
            }
            LocationInfo Info = JsonUtility.FromJson<LocationInfo>(www.downloadHandler.text);
            Elevation_Double = Info.results[0].elevation;
            Debug.Log(Elevation_Double);
        }


        [Serializable]
        public class LocationInfo
        {
            public List<Result> results;
            public string status;
        }

        [Serializable]
        public class Result
        {
            public string dataset;
            public double elevation;
            public Location location;
            //public Location location;
        }

        [Serializable]
        public class Location
        {
            public double lat;
            public double lng;
        }
    }
}

