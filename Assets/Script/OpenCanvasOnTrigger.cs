using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCanvasOnTrigger : MonoBehaviour
{

    //The purpose of it, is to open a canvas / interface and make the camera move when its triggered by the player

    //It is more of a container as it's the CameraMoveOnTriggerManager who manage it all, as we have more than on trigger on our scene

    //OpenCanvasOnTrigger can only do "request" to the manager

    //The canvas to open
    [SerializeField]
    public GameObject canvas;
    //The camera destination
    [SerializeField]
    public Transform goTo;



    void Start()
    {
        CameraMoveOnTriggerManager.Instance.RegisterCanvasTrigger(this);
    }
    private void End()
    {
        CameraMoveOnTriggerManager.Instance.UnregisterCanvasTrigger(this);
    }



    void  OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            CameraMoveOnTriggerManager.Instance.RequestCamera(this);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            CameraMoveOnTriggerManager.Instance.EndOfRequest(this);
        }
    }

}
