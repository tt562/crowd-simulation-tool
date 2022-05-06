using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCFollow : MonoBehaviour
{

    public GameObject groupLeader;
    public float targetDistance;
    public float allowedDistance = 10;
    public NavMeshAgent follower;
    public float followSpeed;
    public RaycastHit Shot;


    void Start()
    {
        follower = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        //Follow the player
        follower.speed = 5.0f;
        follower.destination = groupLeader.transform.position;
    }
}
