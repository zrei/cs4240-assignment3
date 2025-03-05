using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] private GameObject m_PlacementIndicator;
    [SerializeField] private InputActionReference m_InputActionReference;

    private Pose m_PlacementPose;
    private ARRaycastManager m_ARRaycastManager;
    private bool m_PlacementPoseIsValid = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_ARRaycastManager = FindFirstObjectByType<ARRaycastManager>();
    }

    private void Awake()
    {
        //m_TouchAction = m_PlayerInput.actions.FindAction("SingleTouchClick");
    }

    /*
    private void OnEnable()
    {
        m_TouchAction.started += PlaceObject;
    }

    private void OnDisable()
    {
        m_TouchAction.started -= PlaceObject;
    }
    */

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
            m_PlacementIndicator.SetActive(true);
            m_PlacementIndicator.transform.SetPositionAndRotation(m_PlacementPose.position, m_PlacementPose.rotation);
        }
        else
        {
            m_PlacementIndicator.SetActive(false);
        }
    }

    /*
    private void PlaceObject(InputAction.CallbackContext context)
    {
        if (m_PlacementPoseIsValid)
            Instantiate(ObjectToPlace, m_PlacementPose.position, m_PlacementPose.rotation);
    }
    */
}
