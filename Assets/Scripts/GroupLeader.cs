using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupLeader : MonoBehaviour
{

    public GameObject groupArea;
    

    private void OnTriggerEnter(Collider other)
    {

        NPCGroup group = this.gameObject.transform.parent.GetComponent<NPCGroup>();


        // If the group approaches another group coming the other way, move into single file

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
        // Once a group passes another group after having to move into single-file, move back to their original formation

        if (other.name == "GroupArea" && other.gameObject.transform.parent.parent != this.gameObject.transform.parent)
        {

            this.gameObject.GetComponentInParent<NPCGroup>().updateShape();

        }
    }

    // Check if a group is approaching from the opposite direction or going in the same direction
    private bool FacingSameDirection(GameObject otherPerson)
    {
       

        float rotation = this.gameObject.transform.rotation.eulerAngles.y;
        float otherRotation = otherPerson.transform.parent.rotation.eulerAngles.y;


        float diff = Mathf.DeltaAngle(rotation, otherRotation);

        if(diff < 0)
        {
            diff *= -1;
        }

        if(diff <= 90)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
