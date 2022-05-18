using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetParentTest : MonoBehaviour
{
    [SerializeField] GameObject RealTimeMarker;
    [SerializeField] Transform MarkersParent;
    GameObject marker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Initialized");
            marker = Instantiate(RealTimeMarker, new Vector3(0,0,1), Quaternion.identity);
            marker.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("Set Parent False");
            marker.transform.SetParent(MarkersParent, false);
        }


        if (Input.GetKeyUp(KeyCode.T))
        {
            Debug.Log("Set Parent False");
            marker.transform.SetParent(MarkersParent, true);
        }
    }
}
