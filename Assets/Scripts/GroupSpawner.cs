using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class GroupSpawner : MonoBehaviour
{

    public GameObject presetGroup;

    
    public int[] groupDistribution;

    public int[] originalDistribution;

    private Transform spawnArea;




    private void Start()
    {

        originalDistribution = groupDistribution;

        for (int groupSize = 0; groupSize < groupDistribution.Length; groupSize++)
        {

            for (int i = 0; i < groupDistribution[groupSize]; i++)
            {
                GameObject group = SpawnGroup(groupSize+1);

                NPCGroup groupComp = group.GetComponent<NPCGroup>();

                groupComp.randomiseClothing(groupComp.groupLeader);

                group.name = group.name + "Size: " + (groupSize+1) + i;



            }
        }


    }

  
    public GameObject SpawnGroup(int groupSize)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = GetRandomLocation();



        GameObject group = Instantiate(presetGroup, cube.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
        group.transform.localPosition = cube.transform.position;

        cube.gameObject.SetActive(false);

        group.GetComponent<NPCGroup>().groupSize = groupSize;


        return group;
    }

    public void DespawnRandomGroup(int groupSize)
    {
        
        GameObject[] currentGroups = GameObject.FindGameObjectsWithTag("FormationPosition");


        for(int j=currentGroups.Length; j >= 0; j--)
        {
            if(currentGroups[j].GetComponent<NPCGroup>().groupSize == groupSize)
            {
                Destroy(currentGroups[j]);
                return;
            }

        }
    }


    // Get random location in the spawn area
    Vector3 GetRandomLocation()
    {
        int noAreas = this.transform.childCount;

        int spawnAreaNo = Random.Range(0, noAreas);

        spawnArea = this.transform.GetChild(spawnAreaNo);

        float minX;
        float maxX;

        float minZ;
        float maxZ;

        if (spawnArea.rotation.y == 0)
        {
            minX = spawnArea.localPosition.x - ((spawnArea.localScale.x / 2) * 10);
            maxX = spawnArea.localPosition.x + ((spawnArea.localScale.x / 2) * 10);
        }
        else
        {
            minX = spawnArea.localPosition.x - ((spawnArea.localScale.z / 2) * 10);
            maxX = spawnArea.localPosition.x + ((spawnArea.localScale.z / 2) * 10);
        }



        if (spawnArea.rotation.y == 0)
        {
            minZ = spawnArea.localPosition.z - ((spawnArea.localScale.z / 2) * 10);
            maxZ = spawnArea.localPosition.z + ((spawnArea.localScale.z / 2) * 10);
        }
        else
        {
            minZ = spawnArea.localPosition.z - ((spawnArea.localScale.x / 2) * 10);
            maxZ = spawnArea.localPosition.z + ((spawnArea.localScale.x / 2) * 10);
        }

        float randX = Random.Range(minX, maxX);
        float randZ = Random.Range(minZ, maxZ);

        Vector3 randomPos = new Vector3(randX, 0.5f, randZ);


        return randomPos;

    }

    public bool inRadius(Vector3 object1)
    {
        float radius = 30;

        GameObject[] gos = GameObject.FindGameObjectsWithTag("NPC");

        foreach(GameObject go in gos)
        {
            float dist = Vector3.Distance(object1, go.transform.position);

            if(dist < radius)
            {
                return true;
            }
        }

        return false;
     }

}
