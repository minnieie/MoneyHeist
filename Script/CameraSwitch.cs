using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera mainCamera;
    public Camera playerCamera;

    void Start()
    {
        mainCamera.enabled = true;
        playerCamera.enabled = false;

        // Ensure cursor visible and unlocked at start (for main camera)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SwitchToPlayerCamera()
    {
        mainCamera.enabled = false;
        playerCamera.enabled = true;

        Cursor.lockState = CursorLockMode.None; // Allow free movement
        Cursor.visible = true; // Ensure cursor remains visible

        if (mainCamera.TryGetComponent(out AudioListener mainListener))
            mainListener.enabled = false;

        if (playerCamera.TryGetComponent(out AudioListener playerListener))
            playerListener.enabled = true;
    }

    public void SwitchToMainCamera()
    {
        playerCamera.enabled = false;
        mainCamera.enabled = true;

        // Unlock and show cursor for UI or menu camera
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerCamera.TryGetComponent(out AudioListener playerListener))
            playerListener.enabled = false;

        if (mainCamera.TryGetComponent(out AudioListener mainListener))
            mainListener.enabled = true;
    }
}
