using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameUIManager : NetworkBehaviour
{
    public static GameUIManager Instance;
    
    [Header("Lives Display")]
    [SerializeField] private TMP_Text livesText;
    
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTitle;
    [SerializeField] private TMP_Text gameOverMessage;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Hide game over panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
    
    public void UpdateLivesDisplay(int lives)
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {lives}";
        }
    }
    
    public void ShowGameOver(bool playerWon)
    {
        if (gameOverPanel == null) return;
        
        gameOverPanel.SetActive(true);
        
        if (playerWon)
        {
            gameOverTitle.text = "VICTORY!";
            gameOverTitle.color = Color.green;
            gameOverMessage.text = "All thieves caught!\nThe computer system is secure!";
        }
        else
        {
            gameOverTitle.text = "GAME OVER";
            gameOverTitle.color = Color.red;
            gameOverMessage.text = "Out of lives!\nThe thieves escaped!";
        }
        
        // disable player controls
        Time.timeScale = 0f; // Pause the game
    }
    
    // Call this from a button on the game over screen
    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause
        
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    // Call this from a button to return to main menu
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Unpause
        
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        SceneManager.LoadScene(0); // Load first scene (menu)
    }
}