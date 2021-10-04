using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;

    private TMP_Text[] texts;
    //private float[] distances;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points)
    {
        lr.positionCount = points.Length;
        this.points = points;
    }

    public void SetUpText(TMP_Text[] texts)
    {
        this.texts = texts;
    }

    public void DistanceBetweenPoints()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            float distance = Vector3.Distance(points[i].position, points[i+1].position);
            texts[i].text = distance.ToString();
        }

    }
    public void SetTextOnLine()
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            texts[i].transform.position = new Vector3((points[i].position.x+ points[i+1].position.x)/2f, (points[i].position.y + points[i + 1].position.y) / 2f, (points[i].position.z + points[i + 1].position.z) / 2f);
        }
        
    }

    private void Update()
    {
        for(int i=0; i<points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}
