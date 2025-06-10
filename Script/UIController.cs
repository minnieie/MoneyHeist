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
    public PlayerBehaviour playerBehaviour;

    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip gameOverBGM; // Changed to BGM
    private AudioSource audioSource;
    private AudioSource bgmAudioSource; // Separate audio source for BGM

    [Header("Settings")]
    public float fadeDuration = 1.0f;
    public static bool isRestarting = false;

    [Header("Theft Timer")]
    public TextMeshProUGUI theftTimerText;
    public float theftDuration = 30f;
    private float currentTheftTime;
    public bool isTheftTimerRunning = false;

    void Start()
    {
        // Set up button sound audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set up BGM audio source
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = false;

        if (isRestarting)
        {
            panel.SetActive(false);
            okayButton.SetActive(false);
            missionButton.SetActive(true);
            ResetGameState();
            isRestarting = false;
        }
        else
        {
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
        {
            gameOverPanel.SetActive(false);
        }

        currentTheftTime = theftDuration;
        isTheftTimerRunning = false;

        if (theftTimerText != null)
        {
            theftTimerText.enabled = false;
            theftTimerText.color = Color.white;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ResetGameState()
    {
        if (playerBehaviour != null)
            playerBehaviour.ResetPlayer(); // Explicitly reset the player

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera(); // Reset camera properly

        if (scoreText != null)
        {
            scoreText.enabled = true;
            UpdateScoreDisplay(0);
        }

        if (timeText != null)
        {
            timeText.enabled = true;
        }

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();

        currentTheftTime = theftDuration; // Ensure timer resets properly
        isTheftTimerRunning = false;

        if (theftTimerText != null)
            theftTimerText.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Explicitly hide game over panel

        StopGameOverBGM();
    }

    public void HidePanelAndButton()
    {
        PlayButtonClickSound();
        panel.SetActive(false);
        okayButton.SetActive(false);
        missionButton.SetActive(true);

        if (scoreText != null)
            scoreText.enabled = true;

        if (timeText != null)
            scoreText.enabled = true;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();
    }

    public void ShowPanelAndOkayButton()
    {
        PlayButtonClickSound();
        panel.SetActive(true);
        okayButton.SetActive(true);
        missionButton.SetActive(false);

        if (scoreText != null)
            scoreText.enabled = false;

        if (timeText != null)
            scoreText.enabled = false;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToMainCamera();
    }

    public void OnMissionButtonClicked()
    {
        PlayButtonClickSound();
        if (sceneFade != null)
            StartCoroutine(FadeInOnly());
    }

    private IEnumerator FadeInOnly()
    {
        yield return sceneFade.FadeInCoroutine(fadeDuration);
        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();
    }

    public void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null && scoreText.enabled)
            scoreText.text = "SCORE: " + newScore.ToString();
    }

    public void StartTheftTimer()
    {
        currentTheftTime = theftDuration;
        isTheftTimerRunning = true;
        if (theftTimerText != null)
            theftTimerText.enabled = true;
    }

    public void StopTheftTimer()
    {
        isTheftTimerRunning = false;
        if (theftTimerText != null)
            theftTimerText.enabled = false;
    }

    public void UpdateTheftTimerDisplay(float timeRemaining)
    {
        if (theftTimerText != null)
        {
            theftTimerText.text = Mathf.CeilToInt(timeRemaining) + "s";
            theftTimerText.enabled = true;

            if (timeRemaining < 10f)
                theftTimerText.color = Color.red;
            else if (timeRemaining < 20f)
                theftTimerText.color = Color.yellow;
            else
                theftTimerText.color = Color.white;
        }
    }

    void Update()
    {
        if (timeText != null && timeText.enabled && SunRotates.Instance != null)
            timeText.text = SunRotates.Instance.GetFormattedTime();

        if (isTheftTimerRunning)
        {
            currentTheftTime -= Time.deltaTime;
            UpdateTheftTimerDisplay(currentTheftTime);

            if (currentTheftTime <= 0)
            {
                StopTheftTimer();
                ShowGameOverScreen();
            }
        }
    }

    public void ShowGameOverScreen()
    {
        Debug.Log("Restarting... Player health is now: " + playerBehaviour.currentHealth);
        if (playerBehaviour != null)
        {
            Debug.Log("Game Over triggered! Current health: " + playerBehaviour.currentHealth);
        }
        else
        {
            Debug.LogWarning("PlayerBehaviour reference is missing!");
        }

        PlayGameOverBGM();
        missionButton.SetActive(false);
        okayButton.SetActive(false);

        if (scoreText != null)
            scoreText.enabled = false;

        if (timeText != null)
            timeText.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        StopTheftTimer();
        isTheftTimerRunning = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToMainCamera();

        if (bgmAudioSource != null && gameOverBGM != null && !bgmAudioSource.isPlaying)
        {
            bgmAudioSource.clip = gameOverBGM;
            bgmAudioSource.Play();
        }

        // Delay respawn for a better flow
        StartCoroutine(RespawnPlayerAfterDelay(2.0f));
    }

    private IEnumerator RespawnPlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerBehaviour != null && playerBehaviour.playerSpawnPoint != null)
        {
            playerBehaviour.transform.position = playerBehaviour.playerSpawnPoint.position;
            Physics.SyncTransforms(); // Ensures Unity updates physics properly
            playerBehaviour.ResetPlayer();

            // ✅ Hide the game over panel & reset UI
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (panel != null)
                panel.SetActive(false); // Ensures UI doesn't stay stuck

            if (okayButton != null)
                okayButton.SetActive(false); // Hides unnecessary button

            if (missionButton != null)
                missionButton.SetActive(true); // Ensure mission button visibility

            Debug.Log("Player has respawned!");
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    private void PlayGameOverBGM()
    {
        if (bgmAudioSource != null && gameOverBGM != null)
        {
            bgmAudioSource.clip = gameOverBGM;
            bgmAudioSource.Play();
        }
    }

    public void OnRestartButtonPressed()
    {
        isRestarting = false; // Prevent full scene reload
        StopGameOverBGM();

        // Respawn the player instead of restarting the scene
        if (playerBehaviour != null && playerBehaviour.playerSpawnPoint != null)
        {
            playerBehaviour.transform.position = playerBehaviour.playerSpawnPoint.position;
            Physics.SyncTransforms(); // Ensure Unity updates physics properly
            playerBehaviour.ResetPlayer();
            HideGameOverPanel();
            Debug.Log("Player has respawned!");
        }
        else
        {
            Debug.LogWarning("Player or spawn point is missing, restarting scene instead...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Fallback: reload scene if respawn fails
        }
    }

    private void StopGameOverBGM()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
        }
    }
}


