using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VRR.HoloNav.Examples
{
    [Serializable]
    public class MyArray
    {
        //public float[] lat_Lon;
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

        private float CurrentBearing;
        public float OriginBearing;
        [SerializeField] private GameObject NodesParents;

        private IEnumerator GPSIEnumerator;
        private bool GPSReceived = false;
        private bool GPSReceived_UnityEditor = false;
        private bool BearingReceived = false;
        private bool BearingReceived_UnityEditor = false;

        /*The cooresponding GPS and Unity Coordinate*/
        public Transform[] nodesCoordinate;
        public MyArray[] nodesGPS; // nodesGPS[row].lat_Lon[column]

        /*Recording Data*/
        //RecordData recorder;

        /*CompassBar*/
        public QuantumTek.QuantumTravel.CompassBar compassBar;

        [SerializeField] Text text;


        void Awake()
        {
            CurrentBearing = 0f;
            OriginBearing = 0f;
           /*Default Starting Point: FGH*/
            //OriginGPS = new Vector2(36.144752f, -86.803193f);
            OriginLat_Double = 36.144752;
            OriginLon_Double = -86.803193;
            //GPSEncoder.SetLocalOrigin(OriginGPS);
            GPSEncoder_Double.SetLocalOrigin(OriginLat_Double, OriginLon_Double);

            //nodesGPS[0].lat_Lon[0] = OriginGPS.x;
            //nodesGPS[0].lat_Lon[1] = OriginGPS.y;
            nodesGPS[0].lat_Lon[0] = OriginLat_Double;
            nodesGPS[0].lat_Lon[1] = OriginLon_Double;
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
            StartCoroutine(RecordHoloNavData());         
        }

        // Update is called once per frame
        void Update()
        {
            /*Only For Unity Editor Testing*/
            if(Input.GetKeyDown(KeyCode.Space))
            {
                GPSReceived_UnityEditor = !GPSReceived_UnityEditor;
                BearingReceived_UnityEditor = !BearingReceived_UnityEditor;
            }
            text.text = "CurrentBearing"+CurrentBearing.ToString()+ "\n OriginBearing" + OriginBearing.ToString();
        }
        private IEnumerator SetCoordinates()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.1f);
                if (GPSReceived || GPSReceived_UnityEditor)
                {
                    Debug.Log("GPS ready");
                    if(GPSReceived)                    
                    {
                        //OriginGPS = CurrentGPS;
                        OriginLat_Double = CurrentLat_Double;
                        OriginLon_Double = CurrentLon_Double;
                    }
                    //GPSEncoder.SetLocalOrigin(OriginGPS);
                    GPSEncoder_Double.SetLocalOrigin(OriginLat_Double, OriginLon_Double);

                    //nodesGPS[0].lat_Lon[0] = OriginGPS.x;
                    //nodesGPS[0].lat_Lon[1] = OriginGPS.y;
                    nodesGPS[0].lat_Lon[0] = OriginLat_Double;
                    nodesGPS[0].lat_Lon[1] = OriginLon_Double;

                    for (int i=1; i<nodesCoordinate.Length; i++)
                    {
                        //float Lat = nodesGPS[i].lat_Lon[0];
                        //float Lon = nodesGPS[i].lat_Lon[1];
                        double Lat = nodesGPS[i].lat_Lon[0];
                        double Lon = nodesGPS[i].lat_Lon[1];
                        nodesCoordinate[i].localPosition = GPSEncoder_Double.GPSToUCS_Double(Lat, Lon);
                        Debug.Log(nodesCoordinate[i].localPosition);
                    }
                    
                    Debug.Log("Objects Localized");
                    break;
                }
            }
        }

        private IEnumerator RecordHoloNavData()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);
                Debug.Log("Data Recorder Ready");
                if (GPSReceived)
                {
                    RecordData.SaveData(System.DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm") + "," + CurrentLat_Double.ToString() + "," + CurrentLon_Double.ToString() + "," + CurrentBearing.ToString()+"\n");
                }
            }
        }

        public IEnumerator AutoNorthCalibration()
        {
            while(true)
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


        void OnDataReceived(object sender, TelemetryReceievedEventArgs args)
        {
           
            CurrentBearing = args.Data.Bearing;
            BearingReceived = true;

            // Verify position lock.
            if (args.Data.Locked)
            {
                if(!GPSReceived)
                    GPSReceived = true;
                //CurrentGPS = new Vector2((float)args.Data.Latitude, (float)args.Data.Longitude);
                CurrentLat_Double = args.Data.Latitude;
                CurrentLon_Double = args.Data.Longitude;
            }
            else
            {
                if (GPSReceived)
                    GPSReceived = false;
            }
        }
    }
}

