using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryArea : MonoBehaviour
{

    public bool areaTargeted;

    public GameObject group;


    private void Start()
    {
        areaTargeted = false;

        group = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
