using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FormationPosition : MonoBehaviour
{

    //public bool isActive = true;
    public bool formationSpotTaken = false;

    public GameObject agentInContact;

    public enum GroupSide {NoSide, Left, Right};

    public GroupSide groupSide;


    private void Start()
    {
        //groupSide = GroupSide.NoSide;   
    }

    private void Update()
    {
        /*if(agentInContact != null)
        {
            formationSpotTaken = true;
        }*/
    }

    private void OnTriggerEnter(Collider npc)
    {
        if(npc.tag=="NPC" && npc.gameObject != npc.GetComponentInParent<NPCGroup>().groupLeader)
        {
            


            //this.formationSpotTaken = true;
            //npc.transform.SetParent(this.transform);

        }
    }

    /*private void OnTriggerExit(Collider npc)
    {
        if (npc.tag == "NPC" && npc.gameObject != npc.GetComponentInParent<NPCGroup>().groupLeader)
        {

            this.formationSpotTaken = false;



        }
    }*/
}
