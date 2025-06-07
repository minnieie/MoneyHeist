using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField]
    private Transform rotatingHorizontalPart; // Assign this to the CCTV's horizontal pivot

    [SerializeField]
    private float rotationSpeed = 30f; // Speed of rotation

    [SerializeField]
    private float rotationAngle = 45f; // How far left/right to rotate from center

    private float startingYRotation;
    private float timeCounter = 0f;

    void Start()
    {
        if (rotatingHorizontalPart == null)
        {
            Debug.LogWarning("Rotating part is not assigned on CCTV.");
        }

        startingYRotation = rotatingHorizontalPart.localEulerAngles.y;
    }

    void Update()
    {
        if (rotatingHorizontalPart != null)
        {
            // Make the camera smoothly rotate back and forth
            timeCounter += Time.deltaTime * rotationSpeed;
            float angle = Mathf.Sin(timeCounter * Mathf.Deg2Rad) * rotationAngle;
            rotatingHorizontalPart.localRotation = Quaternion.Euler(0f, startingYRotation + angle, 0f);
        }
    }
}
