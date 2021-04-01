using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

/*
 * Player Controller purpose is to manage click inputs and player movement
 */
public class PlayerController : MonoBehaviour
{
    //This is to make sure the player won't move while interacting with the interfaces
    public bool isControlActive = true;

    public NavMeshAgent agent;
    ThirdPersonCharacter character;

    void Start()
    {
        //Register itself to the Gamemanager
        if(GameManager.MainCharacter == null)
        {
            GameManager.MainCharacter = this;
        }
        agent = GetComponent<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();
    }

    //To hold our character in position
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        agent.destination = this.transform.position;
    }
    //To allow our character to move again
    public void unStop()
    {
        agent.isStopped = false;
    }

    //Handling click to move
    //we use a NavMeshAgent to handle moving
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

    //We use the ThirdPersonCharacter From Unity Standard asset to handle the movement animation
    private void FixedUpdate()
    {
        character.Move(agent.velocity, false, false);
    }
}
