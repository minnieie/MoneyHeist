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
    public GameObject restartButton; // Added for restart functionality
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    [Header("References")]
    public CameraSwitcher cameraSwitcher;
    public SceneFade sceneFade;
    public PlayerBehaviour playerBehaviour;

    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip gameOverBGM;
    public AudioClip normalBGMClip; // 🎵 ADD THIS

    private AudioSource audioSource;
    private AudioSource bgmAudioSource;

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

                if (normalBGMClip != null)
        {
            bgmAudioSource.clip = normalBGMClip;
            bgmAudioSource.Play();
            Debug.Log("Normal BGM started at launch.");
        }
        else
        {
            Debug.LogWarning("Normal BGM clip is not assigned in Inspector!");
        }

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

        currentTheftTime = theftDuration; // Ensure timer resets properly
        isTheftTimerRunning = false;

        if (theftTimerText != null)
            theftTimerText.enabled = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Explicitly hide game over panel
        
        if (restartButton != null)
            restartButton.SetActive(false);


        Debug.Log("Stopping Game Over BGM.");
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
}
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
            timeText.enabled = true;

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
            timeText.enabled = false;

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
        if (scoreText != null)
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
        
        if (restartButton != null)
            restartButton.SetActive(true);


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
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop(); // 🔇 Stop any current BGM
            bgmAudioSource.clip = gameOverBGM;

            if (gameOverBGM != null)
            {
                bgmAudioSource.Play();
                Debug.Log("Playing Game Over BGM.");
            }
            else
            {
                Debug.LogWarning("GameOver BGM clip is missing!");
            }
        }
    }


    public void OnRestartButtonPressed()
    {
        isRestarting = false;

        if (playerBehaviour != null && playerBehaviour.playerSpawnPoint != null)
        {
            playerBehaviour.transform.position = playerBehaviour.playerSpawnPoint.position;
            Physics.SyncTransforms();
            playerBehaviour.ResetPlayer();
            HideGameOverPanel();

            // Reset UI properly
            if (panel != null) panel.SetActive(false);
            if (okayButton != null) okayButton.SetActive(false);
            if (missionButton != null) missionButton.SetActive(true);
            if (scoreText != null) scoreText.enabled = true;
            if (timeText != null) timeText.enabled = true;

            // Switch back to normal BGM
            if (bgmAudioSource != null && normalBGMClip != null)
            {
                bgmAudioSource.Stop();
                bgmAudioSource.clip = normalBGMClip;
                bgmAudioSource.Play();
                Debug.Log("Normal BGM restarted after respawn.");
            }

            if (cameraSwitcher != null)
                cameraSwitcher.SwitchToPlayerCamera();

            Debug.Log("Player has respawned!");
        }
        else
        {
            Debug.LogWarning("Player or spawn point is missing, restarting scene instead...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


}
