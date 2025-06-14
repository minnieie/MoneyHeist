using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    AudioSource doorAudioSource;
    public PanelToggle panelToggle; //UI popup reference

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public float closeDistance = 3f;
    public Transform pivotPoint;

    [Header("Lock Settings")]
    public bool isLocked = false;
    public AudioClip lockedSound;
    public Material lockedMaterial; // Added missing reference

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Transform player;
    private bool isOpen = false;
    private bool isMoving = false;
    private MeshRenderer doorRenderer;
    private Material originalMaterial;

    void Start()
    {
        doorAudioSource = GetComponent<AudioSource>();
        doorRenderer = GetComponent<MeshRenderer>();
        originalMaterial = doorRenderer.material;

        initialRotation = transform.rotation;
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (pivotPoint == null) pivotPoint = transform;

        // Visual feedback for locked doors
        if (isLocked && lockedMaterial != null)
        {
            doorRenderer.material = lockedMaterial;
        }
    }

    void Update()
    {
        // Smooth door rotation
        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            isMoving = true;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, openSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
            // Snap to target rotation when close enough
            if (Quaternion.Angle(transform.rotation, targetRotation) > 0)
            {
                transform.rotation = targetRotation;
            }
        }

        // Auto-close when player is far away
        if (isOpen && !isMoving && Vector3.Distance(pivotPoint.position, player.position) > closeDistance)
        {
            CloseDoor();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            TryInteractWithDoor(other.GetComponent<PlayerBehaviour>());
        }
    }

    private void TryInteractWithDoor(PlayerBehaviour player)
    {
        if (isLocked)
        {
            // Check for key FIRST
            if (player != null && player.hasCardKey)
            {
                UnlockDoor();
                ToggleDoor();
                return; 
            }
            else
            {
                if (lockedSound != null) doorAudioSource.PlayOneShot(lockedSound);
                if (panelToggle != null) panelToggle.ShowMessage();
            }
        }
        else
        {
            ToggleDoor();
        }
    }
    public void ToggleDoor()
    {
        if (!isOpen)
        {
            if (doorAudioSource != null) doorAudioSource.Play();
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void UnlockDoor()
    {
        isLocked = false;
        if (doorRenderer != null) doorRenderer.material = originalMaterial;
        Debug.Log("Door unlocked!");
    }

    void OpenDoor()
    {
        targetRotation = initialRotation * Quaternion.Euler(0, openAngle, 0);
        isOpen = true;
    }

    void CloseDoor()
    {
        targetRotation = initialRotation;
        isOpen = false;
    }
}
