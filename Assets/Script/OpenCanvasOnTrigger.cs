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
