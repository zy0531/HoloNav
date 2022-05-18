using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.WorldLocking.Core;

public class WLTCustomize : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] GameObject Camera;
    [SerializeField] GameObject CameraParent;
    [SerializeField] GameObject ToCalibrateObject;
    //[SerializeField] GameObject Cube;
    [SerializeField] GameObject Landmark;

    public SpacePin _spacepin;
    public Pose _lockedPose;
    public Pose _modelingPoseGlobal;

    public Pose calibratedPose;

    //public Pose _pinnedFromLocked;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _lockedPose = _spacepin.LockedPose;
        _modelingPoseGlobal = _spacepin.ModelingPoseGlobal;
        //_pinnedFromLocked = _alignmentManager.PinnedFromLocked;

        text.text = //_pinnedFromLocked.position.ToString() + "\n"
            _lockedPose.position.ToString() + "\n"
            + _lockedPose.rotation.ToString() + "\n"
            + _modelingPoseGlobal.position.ToString() + "\n"
            + _modelingPoseGlobal.rotation.ToString() + "\n"
            + Camera.transform.position.ToString() + "\n"
            + CameraParent.transform.position.ToString() + "\n"
            + Camera.transform.rotation.ToString() + "\n"
            + CameraParent.transform.rotation.ToString();

        // set new Pose of model
        calibratedPose.position = ToCalibrateObject.transform.position;
        calibratedPose.rotation = ToCalibrateObject.transform.rotation;
    }

    //speech input - "GPS Calibrate"    
/*    public void GPSCalibrate()
    {
        // https://developer.vuforia.com/forum/unity-extension-technical-discussion/pose-estimation-single-image-targets
        // find the offset between the markerobject and trackable 
        Vector3 positionOffset = Cube.transform.position - ToCalibrateObject.transform.position;
        // move the parent of all cues
        Landmark.transform.position = Landmark.transform.position - positionOffset;
    }*/

    
    //speech input - "Save Pose"
    public void SaveCalibratedPose()
    {
        _spacepin.ResetModelingPose();
        //_spacepin.SetFrozenPose(calibratedPose);
        _spacepin.SetLockedPose(calibratedPose);
    }
    //speech input - "Apply Pose"
    public void ApplyCalibratedPose()
    {
        ToCalibrateObject.transform.position = ToCalibrateObject.transform.position + _lockedPose.position;
        ToCalibrateObject.transform.rotation = _lockedPose.rotation; 
    }




    ////speech input - "Clockwise"
    //public void RotateClockwise()
    //{
    //    ToCalibrateObject.transform.RotateAround(ToCalibrateObject.transform.position, Vector3.up, 1);
    //}
    ////speech input - "AntiClockwise"
    //public void RotateAntiClockwise()
    //{
    //    ToCalibrateObject.transform.RotateAround(ToCalibrateObject.transform.position, Vector3.up, -1);
    //}

    ////speech input - "MoveForward"
    //public void MoveForward()
    //{
    //    ToCalibrateObject.transform.position += ToCalibrateObject.transform.forward * 1.0f;
    //}

    ////speech input - "MoveBackward"
    //public void MoveBackward()
    //{
    //    ToCalibrateObject.transform.position -= ToCalibrateObject.transform.forward * 1.0f;
    //}

    ////speech input - "MoveLeft"
    //public void MoveLeft()
    //{
    //    ToCalibrateObject.transform.position -= ToCalibrateObject.transform.right * 1.0f;
    //}

    ////speech input - "MoveRight"
    //public void MoveRight()
    //{
    //    ToCalibrateObject.transform.position += ToCalibrateObject.transform.right * 1.0f;
    //}

    ////speech input - "MoveUp"
    //public void MoveUp()
    //{
    //    ToCalibrateObject.transform.position += ToCalibrateObject.transform.up * 1.0f;
    //}

    ////speech input - "MoveDown"
    //public void MoveDown()
    //{
    //    ToCalibrateObject.transform.position -= ToCalibrateObject.transform.up * 1.0f;
    //}


}
