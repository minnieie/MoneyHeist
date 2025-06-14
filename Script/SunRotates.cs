using UnityEngine;

public class SunRotates : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0f, 24f)]
    public float currentTime = 6f; // Start at 6AM by default

    public float dayLengthInSeconds = 600f; // Defines how many seconds a full 24-hour day lasts in the simulation

    public static SunRotates Instance; // Singleton instance

    void Awake()
    {
        // Set up singleton reference
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (dayLengthInSeconds <= 0)
        {
            return;
        }
    }

    void Update()
    {
        if (dayLengthInSeconds <= 0)
        {
            return;
        }

        float rotationSpeed = 24f / dayLengthInSeconds;
        currentTime += Time.deltaTime * rotationSpeed;

        if (currentTime >= 24f)
        {
            currentTime -= 24f;
        }

        float sunRotationX = currentTime * 15f; // 15 degrees per hour = 360°/24
        transform.rotation = Quaternion.Euler(sunRotationX, 0f, 0f);
    }

    //  Get the current time
    public float GetCurrentTime()
    {
        return currentTime;
    }

    // Is it night? (between 7PM and 6AM)
    public bool IsNight => currentTime >= 19f || currentTime < 7f;

    //  Get time as a 12-hour formatted string
    public string GetFormattedTime()
    {
        int totalMinutes = Mathf.FloorToInt(currentTime * 60f);
        int hours = (totalMinutes / 60) % 24;
        int minutes = totalMinutes % 60;

        string period = hours >= 12 ? "PM" : "AM";
        int displayHour = hours % 12;
        if (displayHour == 0) displayHour = 12;

        return $"{displayHour}:{minutes:D2} {period}";
    }
}
