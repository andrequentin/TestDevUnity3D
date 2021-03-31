using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCanvasOnTrigger : MonoBehaviour
{
    [SerializeField]
    public GameObject canvas;

    [SerializeField]
    public Transform goTo;

    void Start()
    {
        CameraMoveToCanvasManager.Instance.RegisterCanvasTrigger(this);
    }
    private void End()
    {
        CameraMoveToCanvasManager.Instance.UnregisterCanvasTrigger(this);
    }



    void  OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            CameraMoveToCanvasManager.Instance.RequestCamera(this);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            CameraMoveToCanvasManager.Instance.EndOfRequest(this);
        }
    }

}
