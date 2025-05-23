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

        mainCamera.enabled = false;
        playerCamera.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        if (mainCamera.TryGetComponent(out AudioListener mainListener))
            mainListener.enabled = false;

        if (playerCamera.TryGetComponent(out AudioListener playerListener))
            playerListener.enabled = true;
    }

    public void SwitchToMainCamera()
    {
        Debug.Log("🎥 Switching back to Main Camera");

        playerCamera.enabled = false;
        mainCamera.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerCamera.TryGetComponent(out AudioListener playerListener))
            playerListener.enabled = false;

        if (mainCamera.TryGetComponent(out AudioListener mainListener))
            mainListener.enabled = true;
    }
}
