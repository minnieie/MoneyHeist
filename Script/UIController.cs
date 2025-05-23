using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    [SerializeField]
    private SceneController sceneController;
    public GameObject panel;
    public GameObject okayButton;
    public GameObject missionButton;

    void Start()
    {
        missionButton.SetActive(false);  // Hide MissionButton at start
    }

    // Called when OkayButton is clicked
    public void HidePanelAndButton()
    {
        Debug.Log("HidePanelAndButton called");
        panel.SetActive(false);
        okayButton.SetActive(false);
        missionButton.SetActive(true);  // Show MissionButton after hiding panel
    }

    // Called when MissionButton is clicked
    public void ShowPanelAndOkayButton()
    {
        Debug.Log("ShowPanelAndOkayButton called");
        panel.SetActive(true);
        okayButton.SetActive(true);
        missionButton.SetActive(false);  // Hide MissionButton when showing panel
    }
}