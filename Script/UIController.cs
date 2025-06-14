using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class PanelToggle : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panel; //mission panel
    public GameObject okayButton;
    public GameObject missionButton;
    public GameObject gameOverPanel;
    public GameObject restartButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public GameObject interactPromptUI;
    public GameObject messagePanel; // For Lock Door messages
    public float displayTime = 2f; // How long the message stays visible

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI collectibleCountText;
    public GameObject congratsPanel;

    public int currentHealthCount = 0;
    private CollectibleManager collectibleManager;


    [SerializeField]
    Image healthTrackingIcon;

    [SerializeField]
    Sprite[] healthTrackingSprites;

    [Header("References")]
    public CameraSwitcher cameraSwitcher;
    public SceneFade sceneFade;
    public PlayerBehaviour playerBehaviour;

    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip gameOverBGM;
    public AudioClip normalBGMClip;
    public AudioClip congratsBGM;

    private AudioSource audioSource;
    private AudioSource bgmAudioSource;

    [Header("Settings")]
    public float fadeDuration = 1.0f;
    public static bool isRestarting = false;

    [Header("Theft Timer")]
    public TextMeshProUGUI theftTimerText;
    public float theftDuration = 180f;
    private float currentTheftTime;
    public bool isTheftTimerRunning = false;

    public Sprite[] HealthTrackingSprites { get => healthTrackingSprites; set => healthTrackingSprites = value; }

    void Start()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Updated initialization
        collectibleManager = FindAnyObjectByType<CollectibleManager>();
        if (collectibleManager != null)
        {
            collectibleManager.OnCollectibleCountChanged += UpdateCollectibleUI;
            // Force immediate UI sync
            UpdateCollectibleUI(collectibleManager.collectedCount, collectibleManager.totalCollectibles);
        }

        if (playerBehaviour != null)
        {
            currentHealthCount = playerBehaviour.currentHealth;
            UpdateHealthSprite(currentHealthCount);
        }
        else
        {
            Debug.LogWarning("PlayerBehaviour reference not assigned!");
        }

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

        if (collectibleCountText != null)
        {
            collectibleCountText.enabled = !panel.activeSelf;
        }

        currentTheftTime = theftDuration;
        isTheftTimerRunning = false;

        if (theftTimerText != null)
        {
            theftTimerText.enabled = false;
            theftTimerText.color = Color.white;
        }

        if (healthTrackingIcon != null)
        {
            healthTrackingIcon.enabled = !panel.activeSelf;
        }
        
        if (messagePanel != null)
        {
            messagePanel.SetActive(false); // Ensure panel is hidden at start
        }
    }
    private void OnDestroy()
    {
        if (collectibleManager != null)
        {
            collectibleManager.OnCollectibleCountChanged -= UpdateCollectibleUI;
        }
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
        if (collectibleCountText != null)
        {
            collectibleCountText.enabled = true;
        }

        if (healthTrackingIcon != null)
            healthTrackingIcon.enabled = true;

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
    public void UpdateHealthSprite(int health)
    {
        int maxHealth = 10;
        int spriteCount = healthTrackingSprites.Length;

        // Normalize health to sprite range
        int spriteIndex = Mathf.FloorToInt((float)health / maxHealth * (spriteCount - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, spriteCount - 1);

        healthTrackingIcon.sprite = healthTrackingSprites[spriteIndex];

        if (healthText != null)
            healthText.text = health.ToString();
    }


    public void HidePanelAndButton()
    {
        PlayButtonClickSound();
        panel.SetActive(false);
        okayButton.SetActive(false);
        missionButton.SetActive(true);
        healthTrackingIcon.gameObject.SetActive(true);


        if (scoreText != null)
            scoreText.enabled = true;

        if (collectibleCountText != null)
            collectibleCountText.enabled = true;

        if (timeText != null)
            timeText.enabled = true;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();

        if (healthTrackingIcon != null)
            healthTrackingIcon.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowPanelAndOkayButton()
    {
        PlayButtonClickSound();
        panel.SetActive(true);
        okayButton.SetActive(true);
        missionButton.SetActive(false);
        healthTrackingIcon.gameObject.SetActive(false);


        if (scoreText != null)
            scoreText.enabled = false;

        if (timeText != null)
            timeText.enabled = false;

        if (collectibleCountText != null)
            collectibleCountText.enabled = false;

        if (healthTrackingIcon != null)
            healthTrackingIcon.enabled = false;

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToMainCamera();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void OnMissionButtonClicked()
    {
        Debug.Log("Mission button clicked!");
        PlayButtonClickSound();

        if (sceneFade != null)
            StartCoroutine(FadeInOnly());
    }

    public void OnOkayButtonClicked()
    {
        PlayButtonClickSound();

        if (cameraSwitcher != null)
            cameraSwitcher.SwitchToPlayerCamera();

        HidePanelAndButton();
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
    public void StartTheftTimer(bool forceRestart = false)
    {
        // Don't restart if already running unless forced
        if (isTheftTimerRunning && !forceRestart) return;

        currentTheftTime = theftDuration;
        isTheftTimerRunning = true;

        if (theftTimerText != null)
        {
            theftTimerText.enabled = true;
            UpdateTheftTimerDisplay(currentTheftTime);
        }
    }
    public void StopTheftTimer()
    {
        isTheftTimerRunning = false;

        if (theftTimerText != null)
        {
            theftTimerText.enabled = false;
            theftTimerText.text = "";
        }
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

        // Update health tracking icon
        if (playerBehaviour != null)
        {
            int currentHealth = playerBehaviour.currentHealth;
            if (currentHealth != currentHealthCount)
            {
                currentHealthCount = currentHealth;
                UpdateHealthSprite(currentHealthCount);
            }
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

        if (collectibleCountText != null)
            collectibleCountText.enabled = false;

        if (healthTrackingIcon != null)
            healthTrackingIcon.enabled = false;

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
        PlayButtonClickSound(); // Always good to have audio feedback

        if (congratsPanel != null && congratsPanel.activeSelf)
        {
            congratsPanel.SetActive(false);
        }

        // Reset the game state properly
        if (playerBehaviour != null && playerBehaviour.playerSpawnPoint != null)
        {
            // 1. Reset player position and physics
            playerBehaviour.transform.position = playerBehaviour.playerSpawnPoint.position;
            Physics.SyncTransforms();

            // 2. Reset all game systems
            playerBehaviour.ResetPlayer();
            playerBehaviour.ResetAllNPCs();
            playerBehaviour.ResetAllCoins();

            // 3. Force refresh CollectibleManager UI
            CollectibleManager manager = FindAnyObjectByType<CollectibleManager>();
            if (manager != null)
            {
                manager.ResetCollectibles(); // This will update the UI automatically
            }

            // 4. UI Cleanup
            HideGameOverPanel();
            if (panel != null) panel.SetActive(false);
            if (okayButton != null) okayButton.SetActive(false);
            if (missionButton != null) missionButton.SetActive(true);
            if (collectibleCountText != null)
            {
                collectibleCountText.enabled = true;
            }

            // 5. Reactive UI elements
            if (scoreText != null)
            {
                scoreText.enabled = true;
                UpdateScoreDisplay(0); // Explicitly reset score display
            }

            if (timeText != null) timeText.enabled = true;

            // 6. Health UI
            if (healthTrackingIcon != null)
            {
                healthTrackingIcon.gameObject.SetActive(true);
                healthTrackingIcon.enabled = true;
                UpdateHealthSprite(playerBehaviour.maxHealth); // Reset to full health
            }

            // 7. Audio reset
            if (bgmAudioSource != null && normalBGMClip != null)
            {
                bgmAudioSource.Stop();
                bgmAudioSource.clip = normalBGMClip;
                bgmAudioSource.Play();
            }

            // 8. Camera reset
            if (cameraSwitcher != null)
                cameraSwitcher.SwitchToPlayerCamera();

            Debug.Log("Game fully reset!");
        }
        else
        {
            Debug.LogWarning("Critical references missing - reloading scene...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Reset cursor state (good practice)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void ShowInteractPrompt(bool show)
    {
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(show);
        }
        else
        {
            Debug.LogWarning("InteractPromptUI GameObject not assigned in PanelToggle!");
        }
    }
    public void UpdateCollectibleUI(int collected, int total)
    {
        if (collectibleCountText != null)
        {
            // HARDCODE the total to 40 - ignore the 'total' parameter completely
            collectibleCountText.text = $"COLLECTED: {collected}/40";

            if (congratsPanel != null)
            {
                bool allCollected = collected >= 40;
                congratsPanel.SetActive(allCollected);

                if (allCollected)
                {
                    StopTheftTimer();
                    PlayCongratsBGM();
                }
            }

            Debug.Log($"UI Updated: {collected}/40 (Ignored parameter total: {total})");
        }
    }
    public void ShowMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
            Invoke("HideMessage", displayTime);
        }
    }
    private void HideMessage()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
    private void PlayCongratsBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop(); // Stop any current BGM
            bgmAudioSource.clip = congratsBGM;

            if (congratsBGM != null)
            {
                bgmAudioSource.Play();
                Debug.Log("Playing Congrats BGM.");
            }
            else
            {
                Debug.LogWarning("Congrats BGM clip is missing!");
            }
        }
    }

}
