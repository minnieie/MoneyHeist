using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera playerCamera;

    void Start()
    {
        mainCamera.enabled = true;
        playerCamera.enabled = false;
    }

    public void SwitchToPlayerCamera()
    {
        Debug.Log("🎥 Switching to Player Camera");

        mainCamera.enabled = false;
        playerCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (mainCamera.TryGetComponent(out AudioListener mainListener))
            mainListener.enabled = false;

        if (playerCamera.TryGetComponent(out AudioListener playerListener))
            playerListener.enabled = true;
    }
}
