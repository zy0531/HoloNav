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
using UnityEngine;

namespace VRR.HoloNav.Examples
{
    /// <summary>
    /// This script is used to grab and display a MapQuest image from the telemetry co-ordinates
    /// received.
    /// </summary>
    public class MapDisplay : MonoBehaviour
    {
        [SerializeField] string apiKey;
        [SerializeField] TelemetrySource telemetry;
        [SerializeField] NetworkImage networkImage;

        void Start()
        {
            //networkImage.SourceUrl = GetImageUrl(36.144752, -86.803193);
            //networkImage.DisplayImage();
            telemetry.DataReceivedEvent += OnDataReceived;
        }

        string GetImageUrl(double lat, double lng)
        {
            return $"https://www.mapquestapi.com/staticmap/v5/map?key={apiKey}&locations={lat.ToString()},{lng.ToString()}|marker-md&size=400,400";
            //return $"https://www.mapquestapi.com/staticmap/v5/map?key={apiKey}&locations=36.144752,-86.803193&size=400,400";
        }

        void OnDataReceived(object sender, TelemetryReceievedEventArgs args)
        {
            // Verify position lock.
            if (args.Data.Locked)
            {
                networkImage.SourceUrl = GetImageUrl(args.Data.Latitude, args.Data.Longitude);
                networkImage.DisplayImage();

                telemetry.DataReceivedEvent -= OnDataReceived;
            }
            else
            {
                networkImage.SourceUrl = GetImageUrl(36.144752, -86.803193);
                networkImage.DisplayImage();

                telemetry.DataReceivedEvent -= OnDataReceived;
            }
        }
/*        void Update()
        {
            networkImage.SourceUrl = GetImageUrl(36.144752, -86.803193);
            networkImage.DisplayImage();
            telemetry.DataReceivedEvent += OnDataReceived;
        }*/
    }
}
