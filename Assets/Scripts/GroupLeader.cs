using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupLeader : MonoBehaviour
{

    public GameObject groupArea;
    

    private void OnTriggerEnter(Collider other)
    {

        NPCGroup group = this.gameObject.transform.parent.GetComponent<NPCGroup>();

        

        bool facingSameDirection = true;

        if (other.name == "GroupArea" && other.gameObject.transform.parent.parent != this.gameObject.transform.parent)
        {
            facingSameDirection = FacingSameDirection(other.gameObject);
        }

        if (other.name == "GroupArea" && other.gameObject.transform.parent.parent != this.gameObject.transform.parent && !facingSameDirection)
        {
            this.gameObject.GetComponentInParent<NPCGroup>().setSingleFile2();
            this.gameObject.GetComponentInParent<NPCGroup>().removeExcessFormationPoints();
        }

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "GroupArea" && other.gameObject.transform.parent.parent != this.gameObject.transform.parent)
        {

            this.gameObject.GetComponentInParent<NPCGroup>().updateShape();

        }
    }

    private bool FacingSameDirection(GameObject otherPerson)
    {
       

        float rotation = this.gameObject.transform.rotation.eulerAngles.y;
        float otherRotation = otherPerson.transform.parent.rotation.eulerAngles.y;

        //Debug.Log(this.transform.parent.name);

        //Debug.Log("Rotation" + rotation);
        //Debug.Log("Other Rotation" + otherRotation);

        float diff = Mathf.DeltaAngle(rotation, otherRotation);

        if(diff < 0)
        {
            diff *= -1;
        }

        //Debug.Log("Difference" + diff);

        if(diff <= 90)
        {
            //Debug.Log("Same Way");
            return true;
        }
        else
        {
            //Debug.Log("Opposite Way");
            return false;
        }
    }

    /*private void Update()
    {
        Collider centreCollider = this.GetComponentInChildren<BoxCollider>();
        Vector3 currentPosition = this.gameObject.transform.position;

        Vector3 nearestPointToCentre = centreCollider.ClosestPoint(currentPosition);

        float distance = Vector3.Distance(currentPosition, nearestPointToCentre);

        GameObject originalDestination = this.GetComponent<NPCAI>().destination_point;

        if (distance > 2.5f)
        {
            GameObject centre = GameObject.CreatePrimitive(PrimitiveType.Cube);
            centre.transform.position = nearestPointToCentre;
            this.GetComponent<NPCAI>().destination_point = centre;
        }
        else
        {
            this.GetComponent<NPCAI>().destination_point = originalDestination;
        }
    }*/
}
