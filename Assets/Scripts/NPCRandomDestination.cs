using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCRandomDestination : MonoBehaviour
{
   
    public float walkRadius;

    public float randomWalkProb;
    public float busStopProb;


    public void StartWalking()
    {

        this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
    }



    //public GameObject[] busStops;


    void OnTriggerEnter(Collider other)
    {


        if (other.tag == "NPC")
        {
   
            if (other.GetComponent<NPCAI>().currentState == NPCAI.states.Walking)
            {


                this.gameObject.transform.position = GetRandomLocation();

            }


        }
    }

    IEnumerator RandomLocation()
    {
        yield return new WaitForSecondsRealtime(10);
        this.gameObject.transform.position = GetRandomLocation();

    }

    Vector3 MoveToBusStop()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("BusStop");

        //Debug.Log(gos.Length);

       

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.localPosition;
        foreach (GameObject go in gos)
        {

            //Debug.Log(go.transform.localPosition + go.name);

            Vector3 diff = go.transform.localPosition - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                

                distance = curDistance;
            }
        }
        Debug.Log(closest.transform.localPosition + closest.name);
        return closest.transform.localPosition;
    }

    Vector3 GetRandomLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        finalPosition.y = 1;

        return finalPosition;

    }
}
