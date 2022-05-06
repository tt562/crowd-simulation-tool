using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class NPCAI : MonoBehaviour
{

    public GameObject group;

    public GameObject destination_point;
    public GameObject groupDestination;
    public GameObject formationPosition;
    NavMeshAgent agent;

    private GameObject[] formationPoints;
    public GameObject closestFreeFormationPoint;
    public bool formationPointContact;

    public GameObject stationaryArea;

    public int otherMemberIndex;


    public bool NPCInPlace = false;

    public enum states { Walking, WalkToStand, Standing };

    public states currentState = states.Walking;

    // Start is called before the first frame update
    void Start()
    {

        if (this.closestFreeFormationPoint == null)
        {
            this.closestFreeFormationPoint = this.group.GetComponent<NPCGroup>().formationPositions[otherMemberIndex];
        }


        agent = this.GetComponent<NavMeshAgent>();

        if (this.GetComponentInParent<NPCGroup>().destination != null)
        {

            this.groupDestination = this.GetComponentInParent<NPCGroup>().destination;
            this.destination_point = this.GetComponentInParent<NPCGroup>().destination;

        }

        formationPointContact = false;

        Animator animator = this.GetComponent<Animator>();

        // Play walking animation

        animator.SetBool("isWalking", true);

        // Offset the frame the animation starts at to avoid all animations being in sync

        animator.SetFloat("cycleOffset", Random.Range(0.0f, 2.0f));

    }

    // Update is called once per frame
    void Update()
    {
        if (this.currentState == states.Walking)
        {

            // If one of the members leaves their formation point, set their destination back to their formation point and increase their speed in order for them to catch up
            

            if (this.NPCInPlace == false && this.gameObject != this.GetComponentInParent<NPCGroup>().groupLeader)
            {
             
                this.destination_point = closestFreeFormationPoint;
                if (this.agent.speed < 1.2f)
                {
                    this.agent.speed += 0.02f;
                }
                else
                {
                    this.agent.speed = 1.2f;
                }
            }

            // If the member is in their formation point, set their destination to be the same as the group leader

            else
            {
                if (this.GetComponentInParent<NPCGroup>().destination != null)
                {
                    this.destination_point = this.GetComponentInParent<NPCGroup>().destination;

                    this.agent.speed = 1.0f;
                }
            }
        }

        // If in the phase of walking up to the stationary point
            
        
        if(this.currentState == states.WalkToStand)
        {
            if (this.gameObject != this.GetComponentInParent<NPCGroup>().groupLeader)
            {
                this.destination_point = closestFreeFormationPoint;
                this.agent.speed = 1.2f;
            }
            else
            {
                this.destination_point = this.GetComponentInParent<NPCGroup>().destination;
                this.agent.speed = 1.0f;
            }
        }
       

        // If in the phase where standing still, interacting with a stationary interaction point

        if(this.currentState == states.Standing)
        {
            this.agent.speed = 0.0f;

            Animator anim = this.GetComponent<Animator>();

            GameObject stationaryType = this.group.GetComponent<NPCGroup>().destination.transform.parent.gameObject;

            // Play the relevant animation for the specific interaction point

            if (stationaryType.name == "WindowStop")
            {
                anim.SetBool("atWindow", true);
                anim.SetBool("inGroupConversation", false);
                anim.SetBool("atBench", false);
            }
            if (stationaryType.name == "ConversationStop")
            {
                anim.SetBool("inGroupConversation", true);
                anim.SetBool("atWindow", false);
                anim.SetBool("atBench", false);
            }
            if (stationaryType.name == "BenchStop")
            {
                anim.SetBool("inGroupConversation", false);
                anim.SetBool("atWindow", false);
                anim.SetBool("atBench", true);

                // Offset the base of the character so they are at the right height when sat on the bench

                this.GetComponent<NavMeshAgent>().baseOffset = -0.05f;
            }

            anim.SetBool("isWalking", false);


            // Face the look-at point

            if (stationaryArea != null) {
                foreach (Transform child in stationaryArea.transform)
                {

                    if (child.tag == "LookAtPoint")
                    {

                        FaceTarget(child.gameObject, this.gameObject, 1.0f);

                    }
                }
                
            }
        }

        if (destination_point != null)
        {
            Vector3 aim;

            aim = destination_point.transform.position;
            
            this.agent.SetDestination(aim);
        }



    }

    private void OnTriggerEnter(Collider other)
    {
        NPCGroup group = this.GetComponentInParent<NPCGroup>();

        if (other.tag == "FormationPosition" && (other.gameObject == this.closestFreeFormationPoint))
        {

            // If at a stationary interaction point formation point

            if (group.state == NPCGroup.groupState.Stationary && this.currentState == states.WalkToStand)
            {

                this.currentState = states.Standing;
                
            }

            // If at a group formation point

            else
            {
                this.NPCInPlace = true;
                this.agent.speed = 1.0f;
                destination_point = groupDestination;
            }
        }

        if (other.tag == "Destination" && other.name == this.destination_point.name)
        {

            // If the agent goes to a destination point outdoors, find a new destination point
            if (group.state == NPCGroup.groupState.Outside)
            {
                group.destination = group.getRandomDestination();
                this.groupDestination = this.GetComponentInParent<NPCGroup>().destination;
                this.destination_point = this.GetComponentInParent<NPCGroup>().destination;
                

            }
            // If the agent goes to a destination point indoors, despawn the agent
            if (group.state == NPCGroup.groupState.Inside)
            {
                //If wanting the group to be destroyed when going into a building, uncomment this
                //Destroy(this.transform.parent.gameObject);

            }
            if (group.state == NPCGroup.groupState.Stationary && this.gameObject == group.groupLeader)
            {

                if (this.currentState == states.WalkToStand)
                {

                    this.currentState = states.Standing;
                    Animator anim = this.GetComponent<Animator>();

                    GameObject stationaryType = this.group.GetComponent<NPCGroup>().destination.transform.parent.gameObject;

                    if (stationaryType.name == "WindowStop")
                    {
                        anim.SetBool("atWindow", true);
                        anim.SetBool("inGroupConversation", false);
                        anim.SetBool("atBench", false);
                    }
                    if (stationaryType.name == "ConversationStop")
                    {
                        anim.SetBool("inGroupConversation", true);
                        anim.SetBool("atWindow", false);
                        anim.SetBool("atBench", false);
                    }
                    if (stationaryType.name == "BenchStop")
                    {
                        anim.SetBool("inGroupConversation", false);
                        anim.SetBool("atWindow", false);
                        anim.SetBool("atBench", true);
                    }

                    anim.SetBool("isWalking", false);
                }

                else if (this.currentState == states.Walking)
                {

                    this.GetComponentInParent<NPCGroup>().stationaryMode(other.gameObject);
       

                    this.GetComponent<CapsuleCollider>().center = group.otherMembers[0].GetComponent<CapsuleCollider>().center;
                    this.GetComponent<CapsuleCollider>().radius = group.otherMembers[0].GetComponent<CapsuleCollider>().radius;
                    this.GetComponent<CapsuleCollider>().height = group.otherMembers[0].GetComponent<CapsuleCollider>().height;
                    this.GetComponent<CapsuleCollider>().direction = group.otherMembers[0].GetComponent<CapsuleCollider>().direction;
                    

                    foreach (Transform child in this.transform)
                    {
                        if (child.tag == "FormationPosition")
                        {
                            child.GetComponent<BoxCollider>().enabled = false;
                            child.gameObject.SetActive(false);
                        }
                    }


                    this.currentState = states.WalkToStand;


                    this.GetComponent<GroupLeader>().groupArea.GetComponent<BoxCollider>().enabled = false;
                    this.GetComponent<GroupLeader>().groupArea.SetActive(false);


                    this.stationaryArea = other.gameObject;
                    


                    foreach (NavMeshAgent memb in group.otherMembers)
                    {

                        NPCAI member = memb.gameObject.GetComponent<NPCAI>();
                        member.currentState = states.WalkToStand;
                        member.closestFreeFormationPoint = getClosestStationaryPoint();
                        member.stationaryArea = other.gameObject;
          
                    }  
                    
                }
                
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        // If the formation position is left

        if (other.tag == "FormationPosition" && (other.gameObject == this.closestFreeFormationPoint))
        {

            this.NPCInPlace = false;
           
        }
    }

    public GameObject getClosestFreeFormationPoint(int npcIndex)
    {
        formationPoints = GameObject.FindGameObjectsWithTag("FormationPosition");

        GameObject trans = null;

        List<GameObject> nearbyFormationPoints = filterArray(formationPoints);


        
        trans = nearbyFormationPoints[npcIndex];
        

        trans.GetComponent<FormationPosition>().formationSpotTaken = true;

        return trans;

       
    }


    private GameObject getClosestStationaryPoint()
    {

        formationPoints = group.GetComponent<NPCGroup>().formationPositions;



        for (int i = 0; i < formationPoints.Length; i++)
        {

            if (formationPoints[i].GetComponent<FormationPosition>().formationSpotTaken == false)
            {
                formationPoints[i].GetComponent<FormationPosition>().formationSpotTaken = true;
                return formationPoints[i];
            }
            
        }


        return null;
    }

    private List<GameObject> filterArray(GameObject[] array)
    {

        List<GameObject> nearbyPoints = new List<GameObject>();

        foreach(GameObject go in array)
        {
            if(go.transform.IsChildOf(group.GetComponent<NPCGroup>().groupLeader.transform) && (go.GetComponent<FormationPosition>().formationSpotTaken == false))
            {
                nearbyPoints.Add(go);
            }
        }

        return nearbyPoints;


    }

    

    // Rotate pedestrian to look at game object
    public void FaceTarget(GameObject target, GameObject npc, float speed)
    {
        float damping = speed;

        Vector3 lookPos = target.transform.position - npc.transform.position;
        lookPos.y = 0f;
        Quaternion lookRotation = Quaternion.LookRotation(lookPos);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * damping);
    }

    public string getAnimationName()
    {
        Animator animator;
        string clipName;
        AnimatorClipInfo[] currentClipInfo;

        
        animator = gameObject.GetComponent<Animator>();

        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        
        clipName = currentClipInfo[0].clip.name;

        return clipName;
        
    }

}
