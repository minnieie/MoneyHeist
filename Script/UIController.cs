using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel;
    public GameObject okayButton;
    public GameObject missionButton;

    public CameraSwitcher cameraSwitcher;  // Assign in Inspector

    void Start()
    {
        missionButton.SetActive(false);
        panel.SetActive(true);
        okayButton.SetActive(true);

        // Make sure cursor visible at start (if UI needs interaction)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HidePanelAndButton()
    {
        Debug.Log("HidePanelAndButton called");

        panel.SetActive(false);
        okayButton.SetActive(false);
        missionButton.SetActive(true);

        if (cameraSwitcher != null)
        {
            cameraSwitcher.SwitchToPlayerCamera();
        }
        else
        {
            Debug.LogWarning("CameraSwitcher reference not assigned!");
        }
    }

    public void ShowPanelAndOkayButton()
    {
        Debug.Log("ShowPanelAndOkayButton called");

        panel.SetActive(true);
        okayButton.SetActive(true);
        missionButton.SetActive(false);

        if (cameraSwitcher != null)
        {
            cameraSwitcher.SwitchToMainCamera();  // Unlock cursor and show main camera
        }
        else
        {
            Debug.LogWarning("CameraSwitcher reference not assigned!");
        }

        // Cursor visibility handled in SwitchToMainCamera
    }
}
