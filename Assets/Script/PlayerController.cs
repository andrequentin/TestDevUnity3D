using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{

    public bool isControlActive = true;

    public NavMeshAgent agent;
    ThirdPersonCharacter character;
    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.MainCharacter == null)
        {
            GameManager.MainCharacter = this;
        }
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.destination = this.transform.position;
    }
    public void unStop()
    {
        agent.isStopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isControlActive && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                agent.SetDestination(hit.point);
            }

        }
    }
    private void FixedUpdate()
    {
        character.Move(agent.velocity, false, false);
    }
}
