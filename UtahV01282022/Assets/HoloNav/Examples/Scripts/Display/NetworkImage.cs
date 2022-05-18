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
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace VRR.HoloNav.Examples
{
    public class NetworkImage : MonoBehaviour
    {
        [SerializeField] RawImage displayImage;

        public string SourceUrl;

        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                //if (webRequest.isNetworkError)
                if(webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"NetworkImage encountered a network error: {webRequest.error}");
                }
                else
                {
                    Texture2D imageTexture = new Texture2D(1, 1);
                    imageTexture.LoadImage(webRequest.downloadHandler.data);
                    imageTexture.Apply();

                    displayImage.texture = imageTexture;
                }
            }
        }

        public void DisplayImage()
        {
            StartCoroutine(GetRequest(SourceUrl));
        }
    }
}
