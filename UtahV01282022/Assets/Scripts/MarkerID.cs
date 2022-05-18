using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRR.HoloNav.Examples;

public class MarkerID : MonoBehaviour
{
    public int id;
    [SerializeField] GPSToUnityPostion GPSToUnityPostion;
    public void PassID()
    {
        GPSToUnityPostion.markerId = id;
    }
}
