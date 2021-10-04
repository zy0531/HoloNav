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
#if UNITY_WSA && ENABLE_WINMD_SUPPORT
#define USE_BLE
#endif

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VRR.GPSModule;

#if USE_BLE
using VRR.UWP.BLE;
#endif

namespace VRR.HoloNav
{
    public class TelemetrySource : MonoBehaviour
    {
        [Header("BLE Settings")]
        [SerializeField] string bleDeviceName;
        [SerializeField] string bleServiceUuid;
        [SerializeField] string bleCharacteristicUuid;

        [Header("Serial Settings")]
        [SerializeField] string serialPort;
        [SerializeField] int serialBaud;

#if USE_BLE
        BLEConnector<byte[]> connector;
#else
        private SerialConnection serialConnection;
#endif

        /// <summary>
        /// Actions added to this queue are executed on Unity's main thread.
        /// </summary>
        Queue<Action> unityThreadQueue = new Queue<Action>();

        /// <summary>
        /// The most recent GPS Module data that had a GPS lock.
        /// </summary>
        public ModuleData? LastData { get; private set; } = null;

        /// <summary>
        /// The current state of connection to the telemetry source (true when connected).
        /// </summary>
        public bool Connected { get; private set; } = false;

        /// <summary>
        /// Triggered when data is received from the GPS module.
        /// </summary>
        public EventHandler<TelemetryReceievedEventArgs> DataReceivedEvent;

        /// <summary>
        /// Triggered when HoloNavTelemetry is disconnected.
        /// </summary>
        public EventHandler DisconnectedEvent;

        void Update()
        {
            if (unityThreadQueue.Count > 0)
                unityThreadQueue.Dequeue()?.Invoke();
        }

        /// <summary>
        /// Processes the incoming message and interprets it into the ModuleData struct.
        /// </summary>
        /// <param name="data">Incoming data to process.</param>
        private void OnDataReceived(byte[] data)
        {
            try
            {
                // Deserialize the GPS Module's recent message.
                LastData = Serializer.Deserialize(data);

                // Invoke the data received event.
                DataReceivedEvent?.Invoke(this, new TelemetryReceievedEventArgs(LastData.Value));
            }
            catch (SerializerException e)
            {
                Debug.LogError($"HoloNav Deserialization Exception: {e}");
            }
        }

#if USE_BLE
        /// <summary>
        /// Triggered when the BLEConnector encounters an exception.
        /// </summary>
        /// <param name="e">Exception thrown by BLEConnector.</param>
        private void OnException(Exception e)
        {
            Debug.LogError($"HoloNav Telemetry BLE Exception: {e}");
        }

        /// <summary>
        /// Triggered when a BLE device connects.
        /// </summary>
        /// <param name="device">BLE device that connected.</param>
        private void OnDeviceConnected(Windows.Devices.Bluetooth.BluetoothLEDevice device)
        {
            Debug.Log("HoloNav Telemetry: BLE device connected.");
        }
#else
        private void SerialConnectionClosed(object sender, EventArgs e)
        {
            Debug.LogWarning("HoloNav: Serial connection closed.");

            // Perform normal disconnection.
            Task.Run(Disconnect);
        }

        /// <summary>
        /// Called when the serial connection receives data.
        /// </summary>
        private void SerialDataReceived(object sender, DataReceivedEventArgs e)
        {
            unityThreadQueue.Enqueue(() => OnDataReceived(e.Data));
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
#if USE_BLE
            connector = new BLEConnector<byte[]>(
                new string[] { },
                bleServiceUuid,
                bleCharacteristicUuid,
                (data) => unityThreadQueue.Enqueue(() => OnDataReceived(data)),
                (e) => unityThreadQueue.Enqueue(() => OnException(e)),
                true,
                onConnected: (device) => unityThreadQueue.Enqueue(() => OnDeviceConnected(device)),
                deviceNames: new string[] { bleDeviceName }
                );
#else
            serialConnection = new SerialConnection();
            serialConnection.DataReceivedEvent += SerialDataReceived;
            serialConnection.ConnectionClosedEvent += SerialConnectionClosed;

            serialConnection.Configure(serialPort, serialBaud);

            await Task.Run(serialConnection.Connect);
#endif

            // Update connected true.
            Connected = false;
        }

        public async Task Disconnect()
        {
#if USE_BLE
            connector = null;
#else
            if (serialConnection != null && serialConnection.Connected)
                await Task.Run(serialConnection.Disconnect);

            serialConnection = null;
#endif

            // Update connected status.
            Connected = false;

            // Trigger the disconnection event.
            DisconnectedEvent?.Invoke(this, EventArgs.Empty);
        }
    }

}