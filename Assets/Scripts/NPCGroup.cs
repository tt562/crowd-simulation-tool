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



    //public GameObject[] potFormationPositions;

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

        //randomiseClothing(groupLeader.gameObject);

        

        removeExcessMembers(this.transform);

        foreach (Transform child in this.gameObject.transform)
        {
            if (child.gameObject.name != "GroupLeader")
            {
                child.gameObject.SetActive(false);
            }
        }

        state = groupState.Outside;

        destination = getRandomDestination();

        originalShape = shape;


        foreach(GameObject fp in formationPositions)
        {
            fp.SetActive(true);
        }



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

            //removeExcessMembers(0);

            otherMembers[0].gameObject.SetActive(false);

            /*otherMembers[1].gameObject.SetActive(false);
            otherMembers[2].gameObject.SetActive(false);
            otherMembers[1] = null;
            otherMembers[2] = null;*/

            


            //Debug.Log(this.name + groupLeader.transform.localPosition);

            removeExcessFormationPoints();


        }
        // NPC with one other person
        if (groupSize==2)
        {
            //removeExcessMembers(1);

            /*otherMembers[1].gameObject.SetActive(false);
            otherMembers[2].gameObject.SetActive(false);
            otherMembers[1] = null;
            otherMembers[2] = null;*/

            this.otherMembers[0].gameObject.SetActive(true);
            this.otherMembers[0].GetComponent<NPCAI>().otherMemberIndex = 0;


            removeExcessFormationPoints();

        }

        // NPC with two people
        if (groupSize == 3)
        {
            //removeExcessMembers(2);

            /* otherMembers[2].gameObject.SetActive(false);
             otherMembers[2] = null;*/

            this.otherMembers[0].gameObject.SetActive(true);

            prefab = getRandomCharacter();
            this.otherMembers[1] = Instantiate(prefab, transform).GetComponent<NavMeshAgent>();
            this.otherMembers[1].gameObject.SetActive(true);

            this.otherMembers[0].GetComponent<NPCAI>().otherMemberIndex = 0;
            this.otherMembers[1].GetComponent<NPCAI>().otherMemberIndex = 1;



            removeExcessFormationPoints();

        }

        
        if(groupSize == 4)
        {


            //removeExcessMembers(3);

            /*otherMembers[1].gameObject.SetActive(false);
            otherMembers[2].gameObject.SetActive(false);
            otherMembers[1] = null;
            otherMembers[2] = null;*/

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


        

        if (groupSize > 1)
        {

            foreach (NavMeshAgent go in otherMembers)
            {

                if (go != null)
                {
                    randomiseClothing(go.gameObject);

                    go.gameObject.GetComponent<NPCAI>().closestFreeFormationPoint = this.formationPositions[go.gameObject.GetComponent<NPCAI>().otherMemberIndex];
                    //Debug.Log(this.name + go.name + go.gameObject.GetComponent<NPCAI>().closestFreeFormationPoint);
                }
            }
        }

        


        //Debug.Log("GROUP SIZE: " + groupSize);

        
        

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
                //Debug.Log(go.gameObject.name + go.GetComponent<NPCAI>().closestFreeFormationPoint.name);
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

            //Debug.Log(stationaryCategory.name);

            destination = pickRandomChild(stationaryCategory);

            /*while(destination.gameObject.tag != "Destination") 
            {
                destination = pickRandomChild(stationaryCategory);
            }*/

            //Debug.Log("Destination: " + destination.gameObject.name);
            if (destination != null)
            {
                destination.GetComponent<StationaryArea>().areaTargeted = true;
                destination.GetComponent<StationaryArea>().group = this.gameObject;
            }
        }

        //Debug.Log(destType);
        //Debug.Log(destination.name);

        /*if(destination == null)
        {
            getRandomDestination();
        }*/

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

                //Debug.Log(customPart.name + materials[0].name);

                int rand = Random.Range(0, materials.Length);

                //Debug.Log(customPart.name + rand);

                Material chosenMat = materials[rand];

                //Debug.Log(chosenMat.name);

                Material[] cachedMaterials = new Material[materials.Length];

                for(int i=0; i<cachedMaterials.Length; i++)
                {
                    cachedMaterials[i] = chosenMat;
                }

                meshRenderer.materials = cachedMaterials;


                /*for (int i=0; i<materials.Length - 1; i++)
                {
                    //Debug.Log(chosenMat.name);



                    
                   

                    Debug.Log("Before: " + materials[i].name);

                    materials[i] = new Material(chosenMat);

                    Debug.Log("After: " + materials[i].name);

                        //Debug.Log(customPart.name + materials[i].name);
                    
                }*/

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

        /*foreach(Transform model in this.characterModels)
        {
            if(model.gameObject.GetComponent<NavMeshAgent>() != this.otherMembers[0])
            {

                Debug.Log("Remove: " + model.gameObject.name);
                model.gameObject.SetActive(false);
            }
        }*/
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

    destinationType DecideDestinationType()
    {
        //float outdoorsProb = 0.8f;

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
        //npc.transform.localPosition = new Vector3(-1.19f, 0, -0.2f);
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
