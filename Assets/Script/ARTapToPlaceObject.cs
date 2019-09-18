using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// This script is inspired by:
// https://www.dropbox.com/s/n1i5v200npx1mf1/AutoPlaceItem.cs from video 7 of udemy course:
// https://www.udemy.com/create-ar-placement-app-and-full-template-for-photo-app/
// same video available on youtube:
// https://www.youtube.com/watch?v=x08UU-I8eZ8&list=PLw3UgsOGHn4loDyxHG75eJxSnxxVgB-Yb&index=5&t=0s
// arfoundation-examples PlaceOnPlace.cs with GetMouseButtonDown instead of GetMouseButton (was spawning 10 cubes with one click)
// video Getting Started With ARFoundation in Unity (ARKit, ARCore): https://www.youtube.com/watch?v=Ml2UakwRxjk&list=WL&index=4

[RequireComponent(typeof(ARRaycastManager))]
public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    public GameObject placementIndicator;
    private ARRaycastManager m_RaycastManager;
    private Pose m_PlacementPose;
    private bool m_PlacementPoseIsValid = false;
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void Update()
    {
        TryUpdatePlacementPose();
        UpdatePlacementIndicator();

        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        // we actually ignore touchPosition, and always use screenCenter for the ray

        if (m_PlacementPoseIsValid)
        {
            PlaceObject();
        }
    }

    private void PlaceObject()
    {
        Instantiate(objectToPlace, m_PlacementPose.position, m_PlacementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (m_PlacementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(m_PlacementPose.position, m_PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose(Vector3 hitPoint)
    {
        m_PlacementPose.position = hitPoint;
        var cameraForward = Camera.main.transform.forward;
        var cameraBearing = new Vector3(cameraForward.x-90, 0, cameraForward.z).normalized;
        m_PlacementPose.rotation = Quaternion.LookRotation(cameraBearing);
    }

    private void TryUpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        m_RaycastManager.Raycast(screenCenter, s_Hits, TrackableType.Planes);
        m_PlacementPoseIsValid = s_Hits.Count > 0;
        if (m_PlacementPoseIsValid)
        {
            UpdatePlacementPose(s_Hits[0].pose.position);
        }
    }

}