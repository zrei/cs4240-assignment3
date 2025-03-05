using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GridHandler : Singleton<GridHandler>
{
    public GameObject PlacementIndicator;

    private Pose m_PlacementPose;
    private ARRaycastManager m_ARRaycastManager;
    private bool m_PlacementPoseIsValid = false;

    public bool PlacementPoseIsValid => m_PlacementPoseIsValid;
    public Pose PlacementPose => m_PlacementPose;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_ARRaycastManager = FindFirstObjectByType<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        m_ARRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        m_PlacementPoseIsValid = hits.Count > 0;
        if (m_PlacementPoseIsValid)
            m_PlacementPose = hits[0].pose;
    }

    private void UpdatePlacementIndicator()
    {
        if (m_PlacementPoseIsValid)
        {
            PlacementIndicator.SetActive(true);
            PlacementIndicator.transform.SetPositionAndRotation(m_PlacementPose.position, m_PlacementPose.rotation);
        }
        else
        {
            PlacementIndicator.SetActive(false);
        }
    }
}
