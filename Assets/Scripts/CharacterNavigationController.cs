using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNavigationController : MonoBehaviour
{
    public float movementSpeed;
    public float originalSpeed;
    float rotationSpeed = 120;
    double stopDistance = 2.5;
    Vector3 destination;
    public bool reachedDestination;
    Vector3 velocity;
    public Vector3 currentPosition;


    // Start is called before the first frame update
    void Start()
    {
        
        movementSpeed = Random.Range(0.8f, 1.2f);
        originalSpeed = movementSpeed;      
    }

    // Update is called once per frame
    void Update()
    {
        currentPosition = transform.position;
        if(transform.position != destination)
        {
            Vector3 destinationDirection = (destination - transform.position).normalized;
            destinationDirection.y = 0;

            float destinationDistance = destinationDirection.magnitude;

            if(destinationDistance >= stopDistance)
            {
                reachedDestination = false;
                Quaternion targetRotation = Quaternion.LookRotation(destinationDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                reachedDestination = true;
            }

          

        }
    }

    public void SetDestination(Vector3 destination)
    {
        this.destination = destination;
        reachedDestination = false;
    }
}
