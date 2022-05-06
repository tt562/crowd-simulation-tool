using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCGroup : MonoBehaviour
{
    [Range(1, 6)]
    public int groupSize;

    enum destinationType { Indoors, Outdoors, Stationary};


    public enum groupState { Inside, Outside, Stationary};


    public GameObject[] formationPositions;

    public GameObject groupLeader;

    public float outdoorsProb;
    public float indoorsProb;
    public float stationaryProb;


    // Defines whether the next location point will take them indoors or outdoors
    public groupState state;

    public GameObject[] groupDestinations;
    public GameObject[] interiorDestinations;
    public GameObject[] stationaryDestinations;

    public GameObject destination;

    public NavMeshAgent[] otherMembers;

    public GameObject spawnPoint;

    public Transform[] characterModels;

    public Transform prefab;

    public Transform destinationPrefab;

    public enum formationShape {SingleFile, Abreast, VShape };

    public formationShape shape;

    private formationShape originalShape;



    

    // Start is called before the first frame update
    void Start()
    {

        this.otherMembers[0] = getRandomCharacter().GetComponent<NavMeshAgent>();
        

        removeExcessMembers(this.transform);

        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject.name != "GroupLeader")
            {
                child.gameObject.SetActive(false);
            }
        }

        state = groupState.Outside;

        // Pick a random destination point for the group

        destination = getRandomDestination();

        originalShape = shape;


        foreach(GameObject fp in formationPositions)
        {
            fp.SetActive(true);
        }


        // Set the formation shape of the group


        if (shape == formationShape.SingleFile)
        {
            setSingleFile();
        }

        if (shape == formationShape.Abreast)
        {
            setAbreast();
        }

        if (shape == formationShape.VShape)
        {
            setVShape();
        }
    

        // NPC is on their own
        if (groupSize==1 && this.gameObject.name != "NPCGroup1")
        {

            otherMembers[0].gameObject.SetActive(false);


            removeExcessFormationPoints();


        }
        // NPC with one other person
        if (groupSize==2)
        {

            this.otherMembers[0].gameObject.SetActive(true);
            this.otherMembers[0].GetComponent<NPCAI>().otherMemberIndex = 0;


            removeExcessFormationPoints();

        }

        // NPC with two people
        if (groupSize == 3)
        {

            this.otherMembers[0].gameObject.SetActive(true);

            prefab = getRandomCharacter();
            this.otherMembers[1] = Instantiate(prefab, transform).GetComponent<NavMeshAgent>();
            this.otherMembers[1].gameObject.SetActive(true);

            this.otherMembers[0].GetComponent<NPCAI>().otherMemberIndex = 0;
            this.otherMembers[1].GetComponent<NPCAI>().otherMemberIndex = 1;



            removeExcessFormationPoints();

        }

        // NPC with three people
        if(groupSize == 4)
        {

            this.otherMembers[0].gameObject.SetActive(true);

            prefab = getRandomCharacter();
            this.otherMembers[1] = Instantiate(prefab, transform).GetComponent<NavMeshAgent>();
            this.otherMembers[1].gameObject.SetActive(true);
            prefab = getRandomCharacter();
            this.otherMembers[2] = Instantiate(prefab, transform).GetComponent<NavMeshAgent>();
            this.otherMembers[2].gameObject.SetActive(true);

            this.otherMembers[0].GetComponent<NPCAI>().otherMemberIndex = 0;
            this.otherMembers[1].GetComponent<NPCAI>().otherMemberIndex = 1;
            this.otherMembers[2].GetComponent<NPCAI>().otherMemberIndex = 2;      

        }


        
        // Set the allocated formation point for each member of the group
        if (groupSize > 1)
        {

            foreach (NavMeshAgent go in otherMembers)
            {

                if (go != null)
                {
                    randomiseClothing(go.gameObject);

                    go.gameObject.GetComponent<NPCAI>().closestFreeFormationPoint = this.formationPositions[go.gameObject.GetComponent<NPCAI>().otherMemberIndex];
                  
                }
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {

        if(destination.tag != "Destination")
        {
            this.destination = getRandomDestination();
        }
        

        if(Input.GetKeyDown(KeyCode.F))
        {
            foreach (NavMeshAgent go in otherMembers)
            {
                go.GetComponent<NPCAI>().destination_point = go.GetComponent<NPCAI>().closestFreeFormationPoint;

                go.speed = 3.4f;
            }
        }

        // If there's an update in group shape, apply this change

        if(shape != originalShape)
        {
            updateShape();

            originalShape = shape;
        }

        


        if (this.destination.GetComponent<StationaryArea>() != null)
        {
            if (this.destination.GetComponent<StationaryArea>().group != null)
            {
                if (this.destination.GetComponent<StationaryArea>().group.gameObject != this.gameObject)
                {
                    this.destination = getRandomDestination();
                }
            }

        }


        

    }


    public GameObject getRandomDestination()
    { 

        GameObject destination = null;

        destinationType destType = DecideDestinationType();


        if (destType == destinationType.Outdoors)
        {
            destination = pickRandomArrayElement(groupDestinations);
        }
        if(destType == destinationType.Indoors)
        {
            destination = pickRandomArrayElement(interiorDestinations);
        }
        if(destType == destinationType.Stationary)
        {
            GameObject stationaryCategory = pickRandomArrayElement(stationaryDestinations);


            destination = pickRandomChild(stationaryCategory);

            if (destination != null)
            {
                destination.GetComponent<StationaryArea>().areaTargeted = true;
                destination.GetComponent<StationaryArea>().group = this.gameObject;
            }
        }



        return destination;
    }

    private GameObject pickRandomChild(GameObject go)
    {
        GameObject randChild;

        int noChildren = go.transform.childCount;

        int rand;

        if (noChildren > 0)
        {
            rand = Random.Range(0, noChildren - 1);
        }
        else
        {
            rand = 0;
        }

        randChild = go.transform.GetChild(rand).gameObject;

        return randChild;
    }


    private GameObject pickRandomArrayElement(GameObject[] array)
    {
        int x = Random.Range(0, array.Length-1);

        destination = array[x];

        return destination;
    }

    private Transform getRandomCharacter()
    {
        Transform model;

        int rand = Random.Range(0, characterModels.Length);

        model = characterModels[rand];

        return model;
    }

    public void randomiseClothing(GameObject character)
    { 

        foreach (Transform child in character.transform)
        {

            if(child.gameObject.tag == "Clothing")
            {

                GameObject customPart = child.gameObject;

                SkinnedMeshRenderer meshRenderer = customPart.GetComponent<SkinnedMeshRenderer>();

                Material[] materials = meshRenderer.materials;

                int rand = Random.Range(0, materials.Length);

                Material chosenMat = materials[rand];

                Material[] cachedMaterials = new Material[materials.Length];

                for(int i=0; i<cachedMaterials.Length; i++)
                {
                    cachedMaterials[i] = chosenMat;
                }

                meshRenderer.materials = cachedMaterials;

            }
        }
    }


    private void removeExcessMembers(Transform parent)
    {

        for(int i=0; i<parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.gameObject.tag == "NPC" && child.gameObject.name != "GroupLeader")
            {
                if (child.GetComponent<NavMeshAgent>() != this.otherMembers[0])
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

    }


    private GameObject pickRandomStationaryPoint(GameObject[] array)
    {
        List<GameObject> filteredArray = new List<GameObject>();

        for(int i=0; i<array.Length; i++)
        {
            if(!array[i].GetComponent<StationaryArea>().areaTargeted)
            {
                filteredArray.Add(array[i]);
            } 
        }

        if (filteredArray.Count > 0)
        {

            int x = Random.Range(0, filteredArray.Count);

            destination = filteredArray[x];

            return destination;

        }
        else
        {
            return null;
        }
    }

    // Decides the destination type based on the probabilities entered in the Inspector
    destinationType DecideDestinationType()
    {

        destinationType destType;

        float rand = Random.Range(0f, 0.99f);

        if (rand < outdoorsProb)
        {
            destType = destinationType.Outdoors;
            state = groupState.Outside;
        }
        else if (rand >= outdoorsProb && rand < indoorsProb + outdoorsProb)
        {
            destType = destinationType.Indoors;
            state = groupState.Inside;
        }
        else
        {
            destType = destinationType.Stationary;
            state = groupState.Stationary;
        }

        return destType;
    }

    public void moveToLeft(NavMeshAgent npc)
    {
        npc.GetComponent<NPCAI>().destination_point = formationPositions[1];
        npc.speed = 1.5f;
    }

    public void moveToRight(NavMeshAgent npc)
    {
        npc.GetComponent<NPCAI>().destination_point = formationPositions[0];
        npc.speed = 1.5f;
    }

    public void moveToCentre(NavMeshAgent npc)
    {
        npc.GetComponent<NPCAI>().destination_point = formationPositions[2];
        npc.speed = 1.5f;
    }



    public void moveBehind(NavMeshAgent follower)
    {

        follower.GetComponent<NPCAI>().destination_point = groupLeader;

    }

    public void setSingleFile()
    {
        float separation = 1.0f;
        float distanceBehindLeader = separation;

        foreach (GameObject go in formationPositions)
        {
            go.transform.localPosition = new Vector3(0, 1.2f, -distanceBehindLeader);

            distanceBehindLeader += separation;

            go.GetComponent<FormationPosition>().groupSide = FormationPosition.GroupSide.NoSide;
        }
    }

    public void setSingleFile2()
    {
        float separation = 1.0f;
        float distanceAheadLeader = separation;

        foreach (GameObject go in formationPositions)
        {
            go.transform.localPosition = new Vector3(0, 1.2f, distanceAheadLeader);

            distanceAheadLeader += separation;

            go.GetComponent<FormationPosition>().groupSide = FormationPosition.GroupSide.NoSide;
        }
    }

    public void setAbreast()
    {
        float xSeparation = 0.8f;
        float zSeparation = 1.0f;


        // Offset needed to ensure they walk in line with group leader
        float inLineOffset = 0.5f;

        // Sets how many people in a single row
        int noPeopleWidth = 3;

        if (groupSize == 2)
        {
            formationPositions[0].transform.localPosition = new Vector3(-xSeparation, 1.2f, inLineOffset);

            setSide(formationPositions[0], FormationPosition.GroupSide.Left);
        }

        if (groupSize == 3)
        {
            formationPositions[0].transform.localPosition = new Vector3(-xSeparation, 1.2f, inLineOffset);
            formationPositions[1].transform.localPosition = new Vector3(xSeparation, 1.2f, inLineOffset);

            setSide(formationPositions[0], FormationPosition.GroupSide.Left);
            setSide(formationPositions[1], FormationPosition.GroupSide.Right);
        }

        if (groupSize == 4)
        {
            formationPositions[0].transform.localPosition = new Vector3(-xSeparation, 1.2f, inLineOffset);
            formationPositions[1].transform.localPosition = new Vector3(-xSeparation, 1.2f, -zSeparation + inLineOffset);
            formationPositions[2].transform.localPosition = new Vector3(0, 1.2f, -zSeparation + inLineOffset);

            setSide(formationPositions[0], FormationPosition.GroupSide.Left);
            setSide(formationPositions[1], FormationPosition.GroupSide.Left);
            setSide(formationPositions[2], FormationPosition.GroupSide.Right);
        }


    }

    public void setVShape()
    {
        float xSeparation = 0.9f;
        float zSeparation = 1.0f;

        // Offset needed to ensure they walk in line with group leader
        float inLineOffset = 0.5f;

        if (groupSize == 3)
        {
            formationPositions[0].transform.localPosition = new Vector3(-xSeparation, 1.2f, zSeparation + inLineOffset);
            formationPositions[1].transform.localPosition = new Vector3(xSeparation, 1.2f, zSeparation + inLineOffset);

            setSide(formationPositions[0], FormationPosition.GroupSide.Left);
            setSide(formationPositions[1], FormationPosition.GroupSide.Right);
        }

        if (groupSize == 4)
        {
            formationPositions[0].transform.localPosition = new Vector3(-xSeparation, 1.2f, zSeparation + inLineOffset);
            formationPositions[1].transform.localPosition = new Vector3(xSeparation * 2, 1.2f, zSeparation + inLineOffset);
            formationPositions[2].transform.localPosition = new Vector3(xSeparation, 1.2f, inLineOffset);

            setSide(formationPositions[0], FormationPosition.GroupSide.Left);
            setSide(formationPositions[1], FormationPosition.GroupSide.Right);
            setSide(formationPositions[2], FormationPosition.GroupSide.Right);
        }
    }


    public void stationaryMode(GameObject pivotPoint)
    {

        for (int i = 0; i < groupSize - 1; i++)
        {
            formationPositions[i] = pivotPoint.transform.GetChild(i).gameObject;
        }

    }

    // Remove any excess formation points so that there are only enough formation points for the group size
    public void removeExcessFormationPoints()
    {
        for (int i = groupSize - 1; i < formationPositions.Length; i++)
        {

            formationPositions[i].transform.localPosition = formationPositions[0].transform.localPosition;


            formationPositions[i].SetActive(false);

        }
    }

    

    public void updateShape()
    {
        if (shape == formationShape.SingleFile)
        {
            setSingleFile();
        }

        if (shape == formationShape.Abreast)
        {
            setAbreast();
        }

        if (shape == formationShape.VShape)
        {
            setVShape();
        }

        removeExcessFormationPoints();
    }


    public void setSide(GameObject formationPosition, FormationPosition.GroupSide groupSide)
    {
        formationPosition.GetComponent<FormationPosition>().groupSide = groupSide;
    }

}
