/*
* 
* VR Rehab, Inc Confidential
* __________________
* 
*  Copyright 2016 - VR Rehab, Inc
*  All Rights Reserved.
* 
* NOTICE:  All information contained herein is, and remains
* the property of VR Rehab, Inc and its subsidiaries,
* if any.  The intellectual and technical concepts contained
* herein are proprietary to VR Rehab, Inc
* and its subsidiaries and may be covered by U.S. and Foreign Patents,
* patents in process, and are protected by trade secret or copyright law.
* Dissemination of this information or reproduction of this material
* is strictly forbidden unless prior written permission is obtained
* from VR Rehab, Inc.
* contact: smann@virtualrealityrehab.com
*
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace VRR.HoloNav.Examples
{
    /// <summary>
    /// This script is used to display some properties of the HoloNav system.
    /// </summary>
    public class TextDisplay : MonoBehaviour
    {
        public VRR.HoloNav.Examples.GPSToUnityPostion GPSToUnity;

        [SerializeField] Text text;
        [SerializeField] TelemetrySource telemetry;

        private void Start()
        {
            telemetry.DataReceivedEvent += OnDataReceived;
            //StartCoroutine(CheckDataReceivedEvent());
        }


        void OnDataReceived(object sender, TelemetryReceievedEventArgs args)
        {
            StringBuilder sb = new StringBuilder();

            // Verify position lock.
            if (args.Data.Locked)
            {
                sb.Append($"Latitude: {args.Data.Latitude}\nLongitude: {args.Data.Longitude}\n");    
                sb.Append($"Altitude: {args.Data.Altitude}\nGPS Speed: {args.Data.Speed}\nSat Count: {args.Data.Satellites}\n");
            }
            else
            {
                sb.Append("GPS location not locked.");
            }

            sb.Append($"Bearing: {args.Data.Bearing}\u00B0\nPitch: {args.Data.Pitch}\nRoll: {args.Data.Roll}");

            text.text = sb.ToString();
        }
    }
}
