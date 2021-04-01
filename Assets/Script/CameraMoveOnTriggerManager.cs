using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveOnTriggerManager : MonoBehaviour
{
    private static CameraMoveOnTriggerManager instance = null;


    public static CameraMoveOnTriggerManager Instance
    {
        get
        {
            return CameraMoveOnTriggerManager.instance;
        }
    }

    //To make our camera movement, we want our Camera, and and object that is a the same place.
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    Transform OriginCamera;

    //The speed of the camera movement
    [SerializeField]
    float Speed = 5;

    
    [SerializeField]
    float distanceHysteresis = 0.1f;

    //private field for movement management
    private bool isLerp = false;
    private bool isLerpBack = false;

    private Transform currentGoTo;
    private GameObject currentCanvas;

    //List of the Trigger that can request a canvas opening and camera movement
    private List<OpenCanvasOnTrigger> TriggerList=new List<OpenCanvasOnTrigger>();

    private void Awake()
    {    
        //Just to make sure there is only one Manager 

        if (CameraMoveOnTriggerManager.Instance == null)
        {
            CameraMoveOnTriggerManager.instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    //Registering Triggers ...
    public void RegisterCanvasTrigger(OpenCanvasOnTrigger toReg)
    {
        TriggerList.Add(toReg);
    }
    //Unregistering Triggers ...
    public void UnregisterCanvasTrigger(OpenCanvasOnTrigger toReg)
    {
        TriggerList.Remove(toReg);
    }


    public void RequestCamera(OpenCanvasOnTrigger requester)
    {
        Debug.Log(requester + "is requesting camera");
        currentGoTo = requester.goTo;
        currentCanvas = requester.canvas;
        isLerp = true;
        isLerpBack = false;
        //isLerp true means camera goes back to the GoTo destiantion

    }
    public void EndOfRequest(OpenCanvasOnTrigger requester)
    {
        isLerp = false;
        isLerpBack = true;
        //isLerpBack true means camera goes back to the origin
    }

    //Managing camera movement
    private void Update()
    {
        if (isLerp)
        {
            PositionChanging(mainCamera.transform, currentGoTo);
            currentCanvas.SetActive(true);
            if (Vector3.Distance(mainCamera.transform.position, currentGoTo.transform.position) < distanceHysteresis)
            {
                isLerp = false;
            }
        }
        else if (isLerpBack)
        {
            PositionChanging(mainCamera.transform, OriginCamera);
            currentCanvas.SetActive(false);
            if (Vector3.Distance(mainCamera.transform.position, OriginCamera.transform.position) < distanceHysteresis)
            {
                isLerpBack = false;
            }
        }
    }

    //Lerping the position and Slerping the rotation for smooth movement
    void PositionChanging(Transform from, Transform to)
    {
        mainCamera.transform.position = Vector3.Lerp(from.position, to.position, Time.deltaTime * Speed);
        mainCamera.transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, Time.deltaTime * Speed);
    }

   

}
