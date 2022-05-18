using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ManualAdjustment : MonoBehaviour
{

    [SerializeField] GameObject ToCalibrateObject;


    //speech input - "Clockwise"
    public void RotateClockwise()
    {
        ToCalibrateObject.transform.RotateAround(ToCalibrateObject.transform.position, Vector3.up, 1);
    }
    //speech input - "AntiClockwise"
    public void RotateAntiClockwise()
    {
        ToCalibrateObject.transform.RotateAround(ToCalibrateObject.transform.position, Vector3.up, -1);
    }

    //speech input - "MoveForward"
    public void MoveForward()
    {
        ToCalibrateObject.transform.position += ToCalibrateObject.transform.forward * 0.5f;
    }

    //speech input - "MoveBackward"
    public void MoveBackward()
    {
        ToCalibrateObject.transform.position -= ToCalibrateObject.transform.forward * 0.5f;
    }

    //speech input - "MoveLeft"
    public void MoveLeft()
    {
        ToCalibrateObject.transform.position -= ToCalibrateObject.transform.right * 0.5f;
    }

    //speech input - "MoveRight"
    public void MoveRight()
    {
        ToCalibrateObject.transform.position += ToCalibrateObject.transform.right * 0.5f;
    }

    //speech input - "MoveUp"
    public void MoveUp()
    {
        ToCalibrateObject.transform.position += ToCalibrateObject.transform.up * 0.5f;
    }

    //speech input - "MoveDown"
    public void MoveDown()
    {
        ToCalibrateObject.transform.position -= ToCalibrateObject.transform.up * 0.5f;
    }
}
