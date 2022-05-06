using UnityEngine;

public class BusStopController : MonoBehaviour
{
    public bool isEmpty = true;

    public void SitDown()
    {
        if(isEmpty)
        {
            isEmpty = false;
            Debug.Log("Sit Down");
        }
    }
}
