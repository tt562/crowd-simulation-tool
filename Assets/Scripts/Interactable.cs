using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Interactable : MonoBehaviour
{
    
    
    
    
    public bool isInRange;

    public KeyCode interactKey;

    public UnityEvent interactAction;

    public float busStopProb;

    public NavMeshAgent npcAgent;



    /*// Update is called once per frame
    void Update()
    {
        if(isInRange)
        {
            float randomNum = Random.Range(0.0f, 1.0f);
            Debug.Log(randomNum);

            if (randomNum <= busStopProb)
            {
                interactAction.Invoke(); // Fire event
            }

            isInRange = false;
        }
        
    }

    private void OnTriggerEnter(Collider collision) 
    {
        if(collision.gameObject.CompareTag("NPC")) {
            isInRange = true;
            Debug.Log("Player near bus stop");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("NPC")) {
            isInRange = false;
        }
    }*/



    private void OnTriggerEnter(Collider npc)
    {
        if(npc.tag == "NPC")
        {

           /* float randomNum = Random.Range(0.0f, 1.0f);
            Debug.Log(randomNum);

            if (randomNum <= busStopProb)
            {
                Debug.Log("Running");
                //npc.GetComponent<Animator>().Play("Running");

                npc.GetComponent<NavMeshAgent>().speed = 1.0f;


                npc.GetComponent<NPCAI>().currentState = NPCAI.states.Sitting;
                npc.GetComponent<NPCAI>().destination_point.transform.position = this.transform.position;
                this.GetComponent<Interactable>().enabled = false;
            }*/
        }
    }
}
