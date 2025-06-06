using UnityEngine;

public class SunRotates : MonoBehaviour
{
    [Range(0f, 24f)]
    public float currentTime; // The current time in the simulation (0 to 24 hours)

    public float dayLengthInSeconds; // Defines how many seconds a full day lasts in the simulation

    void Start()
    {
        // You can initialize currentTime here if needed, or leave it to be set through the Inspector
        if (dayLengthInSeconds <= 0)
        {
            Debug.LogError("dayLengthInSeconds must be greater than zero to avoid division by zero errors.");
            return;
        }
    }

    void Update()
    {
        // Ensure that dayLengthInSeconds is properly set to avoid division errors
        if (dayLengthInSeconds <= 0)
        {
            Debug.LogWarning("dayLengthInSeconds is not set correctly. Please assign a positive value.");
            return; // Prevent execution if an invalid value is detected
        }

        // Calculate the speed at which the sun should rotate based on the length of a full day
        float rotationSpeed = 24f / dayLengthInSeconds; 

        // Update the current time, ensuring it progresses over time
        currentTime += Time.deltaTime * rotationSpeed;

        // If the currentTime exceeds 24 hours, reset it so the cycle continues
        if (currentTime >= 24f)
        {
            currentTime -= 24f; // Instead of resetting to zero, subtract 24 to maintain natural continuity
        }

        // Apply the calculated rotation based on the currentTime
        // The X-axis rotation controls the progression of the sun position during the day
        float sunRotationX = currentTime * 15f; // Since 360 degrees divided by 24 hours = 15 degrees per hour
        transform.rotation = Quaternion.Euler(sunRotationX, 0f, 0f);

        // Optional Debugging
        Debug.Log("Current Time: " + currentTime.ToString("F2") + " hours | Rotation: " + sunRotationX.ToString("F2") + " degrees");
    }
}
