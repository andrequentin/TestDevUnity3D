using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveToCanvasManager : MonoBehaviour
{
    private static CameraMoveToCanvasManager instance = null;

    public static CameraMoveToCanvasManager Instance
    {
        get
        {
            if (CameraMoveToCanvasManager.instance == null)
            {
                CameraMoveToCanvasManager.instance = new CameraMoveToCanvasManager();
            }
            return CameraMoveToCanvasManager.instance;
        }
    }

    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    Transform OriginCamera;

    [SerializeField]
    float Speed = 5;

    [SerializeField]
    float distanceHysteresis = 0.1f;


    private bool isLerp = false;
    private bool isLerpBack = false;

    private Transform currentGoTo;
    private GameObject currentCanvas;

    private List<OpenCanvasOnTrigger> TriggerList=new List<OpenCanvasOnTrigger>();

    private void Start()
    {
        CameraMoveToCanvasManager.instance = this;
    }
    public void RegisterCanvasTrigger(OpenCanvasOnTrigger toReg)
    {
        TriggerList.Add(toReg);
    }
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
    }
    public void EndOfRequest(OpenCanvasOnTrigger requester)
    {
        isLerp = false;
        isLerpBack = true;
    }

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

    void PositionChanging(Transform from, Transform to)
    {
        mainCamera.transform.position = Vector3.Lerp(from.position, to.position, Time.deltaTime * Speed);
        mainCamera.transform.rotation = Quaternion.Slerp(from.rotation, to.rotation, Time.deltaTime * Speed);
    }

   

}
