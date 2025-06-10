using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PanelToggle : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel;
    public GameObject okayButton;
    public GameObject missionButton;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    [Header("References")]
    public CameraSwitcher cameraSwitcher;
    public SceneFade sceneFade;

    [Header("Settings")]
    public float fadeDuration = 1.0f;

    // Flag to check if restarting from Game Over
    private static bool isRestarting = false;

    void Start()
    {
        if (isRestarting)
        {
            // Skip intro panel logic on restart
            panel.SetActive(false);
            okayButton.SetActive(false);
            missionButton.SetActive(true);
            isRestarting = false;
        }
        else
        {
            // Initial UI state for fresh start
            panel.SetActive(true);
            okayButton.SetActive(true);
            missionButton.SetActive(false);
        }

        if (scoreText != null)
        {
            scoreText.enabled = !panel.activeSelf;
            UpdateScoreDisplay(0);
        }

        if (timeText != null)
        {
            timeText.enabled = !panel.activeSelf;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HidePanelAndButton()
    {
        Debug.Log("HidePanelAndButton called");
        panel.SetActive(false);
        okayButton.SetActive(false);
        missionButton.SetActive(true);

        if (scoreText != null) scoreText.enabled = true;
        if (timeText != null) timeText.enabled = true;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();
        else
            Debug.LogWarning("CameraSwitcher reference not assigned!");
    }

    public void ShowPanelAndOkayButton()
    {
        Debug.Log("ShowPanelAndOkayButton called");
        panel.SetActive(true);
        okayButton.SetActive(true);
        missionButton.SetActive(false);

        if (scoreText != null) scoreText.enabled = false;
        if (timeText != null) timeText.enabled = false;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToMainCamera();
        else
            Debug.LogWarning("CameraSwitcher reference not assigned!");
    }

    public void OnMissionButtonClicked()
    {
        if (sceneFade != null)
            StartCoroutine(FadeInOnly());
        else
            Debug.LogWarning("SceneFade reference not assigned!");
    }

    private IEnumerator FadeInOnly()
    {
        Debug.Log("Fading in...");
        yield return sceneFade.FadeInCoroutine(fadeDuration);
        Debug.Log("Fade in complete, continue mission logic...");

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null && scoreText.enabled)
            scoreText.text = "SCORE: " + newScore.ToString();
    }

    void Update()
    {
        if (timeText != null && timeText.enabled && SunRotates.Instance != null)
            timeText.text = SunRotates.Instance.GetFormattedTime();
    }

    public void ShowGameOverScreen()
    {
        Debug.Log("Game Over!");

        missionButton.SetActive(false);
        okayButton.SetActive(false);
        if (scoreText != null) scoreText.enabled = false;
        if (timeText != null) timeText.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToMainCamera();
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void OnRestartButtonPressed()
    {
        // Set restart flag so Start() logic skips intro
        isRestarting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
