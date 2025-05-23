using System.Collections;
using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public GameObject panel;
    public GameObject okayButton;
    public GameObject missionButton;

    public CameraSwitcher cameraSwitcher;  // Assign in Inspector
    public SceneFade sceneFade;            // Drag the "fade" GameObject (with Image) here
    public float fadeDuration = 1.0f;      // Set your desired fade time in Inspector

    void Start()
    {
        missionButton.SetActive(false);
        panel.SetActive(true);
        okayButton.SetActive(true);

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
            cameraSwitcher.SwitchToMainCamera();
        }
        else
        {
            Debug.LogWarning("CameraSwitcher reference not assigned!");
        }
    }

    // Called when MissionButton is clicked - only fade IN (black → clear)
    public void OnMissionButtonClicked()
    {
        if (sceneFade != null)
        {
            StartCoroutine(FadeInOnly());
        }
        else
        {
            Debug.LogWarning("SceneFade reference not assigned!");
        }
    }

    private IEnumerator FadeInOnly()
    {
        Debug.Log("Fading in...");
        yield return sceneFade.FadeInCoroutine(fadeDuration);

        Debug.Log("Fade in complete, continue mission logic...");
        if (cameraSwitcher != null)
        {
            cameraSwitcher.SwitchToPlayerCamera();
        }
    }
}
