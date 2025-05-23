using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    private bool isOpen = false; //tracks if the door is open or closed
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void interact()
    {
        Vector3 doorRotation = transform.eulerAngles;
        if (!isOpen)
        {
            doorRotation.y += 90f; //open
            isOpen = true;
        }
        else
        {
            doorRotation.y -= 90f; //close
            isOpen = false;
        }
        transform.eulerAngles = doorRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
