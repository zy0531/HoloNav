using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LandmarkInfo : MonoBehaviour
{
    [SerializeField] Text landmarktext;
    public Transform[] LandmarkCoordinate;
    // Start is called before the first frame update
    void Start()
    {        
        
    }

    // Update is called once per frame
    void Update()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"LandmarkInfo:\n");
        for (int i = 0; i < LandmarkCoordinate.Length; i++)
        {
            /*Set y based on the elevation of start position*/
            var position = LandmarkCoordinate[i].localPosition;
            sb.Append($"{LandmarkCoordinate[i].name}: {position}\n");
        }
        landmarktext.text = sb.ToString();
    }
}
