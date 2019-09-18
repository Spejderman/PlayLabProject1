using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
using System;

public class ARController : MonoBehaviour
{

    private ARSessionOrigin aRSessionOrigin;
    private Pose pose;

    void Start()
    {
        aRSessionOrigin = FindObjectOfType<ARSessionOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        AutoPlacePose();
    }

    private void AutoPlacePose()
    {
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f,0.5f));
        var hits = new List <ARRaycastHit>();
        //aRSessionOrigin.Raycast(screenCenter, hits,TrackableType.Planes);
        //find raycast alternative
    }
}
