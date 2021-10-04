using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LineSetUp : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private LineController line;
    [SerializeField] private TMP_Text[] texts;
    // Start is called before the first frame update
    void Start()
    {
        line.SetUpLine(points);
        line.SetUpText(texts);
    }

    // Update is called once per frame
    void Update()
    {
        line.DistanceBetweenPoints();
        line.SetTextOnLine();
    }
}
