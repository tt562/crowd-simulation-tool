/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleController : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        anim = GetComponent<Animator>();

        while(true)
        {
            yield return new WaitForSeconds(3);

            anim.SetInteger("IdleIndex", Random.Range(0, 4));
        }
    }

}
*/