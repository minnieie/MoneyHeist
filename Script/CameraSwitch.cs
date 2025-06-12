using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public Camera mainCamera;
    public Camera playerCamera;

    void Start()
    {
        // No automatic switching — let PanelToggle handle camera logic explicitly
        SetCursorState(true);
        ManageAudioListeners(mainCamera, playerCamera); // Assume mainCamera is active by default
    }

    public void SwitchToPlayerCamera()
    {
        if (mainCamera != null) mainCamera.enabled = false;
        if (playerCamera != null) playerCamera.enabled = true;

        ManageAudioListeners(playerCamera, mainCamera);
    }

    public void SwitchToMainCamera()
    {
        if (playerCamera != null) playerCamera.enabled = false;
        if (mainCamera != null) mainCamera.enabled = true;

        SetCursorState(true); // Unlock cursor for menus
        ManageAudioListeners(mainCamera, playerCamera);
    }

    private void ManageAudioListeners(Camera activeCamera, Camera inactiveCamera)
    {
        if (inactiveCamera != null && inactiveCamera.TryGetComponent(out AudioListener inactiveListener))
            inactiveListener.enabled = false;

        if (activeCamera != null && activeCamera.TryGetComponent(out AudioListener activeListener))
            activeListener.enabled = true;
    }

    private void SetCursorState(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }
}
