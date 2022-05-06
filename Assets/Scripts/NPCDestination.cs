using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDestination : MonoBehaviour
{
    public int trigNum = 0;

    void OnTriggerEnter(Collider other) {
        if(other.tag == "NPC") {

            if(trigNum == 3) {
                this.gameObject.transform.position = new Vector3(120, 1, 155);
                trigNum = 0;
            }
            if(trigNum == 2) {
                this.gameObject.transform.position = new Vector3(120, 1, 76);
                trigNum = 3;
            }
            if(trigNum == 1) {
                this.gameObject.transform.position = new Vector3(178, 1, 76);
                trigNum = 2;
            }
            if(trigNum == 0) {
                this.gameObject.transform.position = new Vector3(179, 1, 155);
                trigNum = 1;
            }
        }
    }
}
