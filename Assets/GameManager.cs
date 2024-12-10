using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Difficulty { Easy, Medium, Hard }
    public enum GameState { MainMenu, Playing, GameOver, Victory }

    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverScoreText;

    private GameState currentState;
    private Difficulty currentDifficulty;
    private float remainingTime;
    private int currentStage = 1;
    private int score = 0;
    private int movesLeft;
    private int minimumMoves;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작 시 메인 메뉴만 보이고 나머지는 모두 숨김
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);  // 게임오버 패널 반드시 숨김
        currentState = GameState.MainMenu;
    }

    public void StartGame(int difficultyLevel)
    {
        currentDifficulty = (Difficulty)difficultyLevel;
        currentState = GameState.Playing;
        score = 0;
        currentStage = 1;
        remainingTime = GetStageTime();

        // 게임 시작 시 게임 패널만 보이고 나머지 숨김
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);  // 게임오버 패널 반드시 숨김

        MapGenerator.Instance.GenerateMap(currentDifficulty);
    }

    public bool IsGameOver()
    {
        return currentState == GameState.GameOver;
    }

    float GetStageTime()
    {
        switch (currentDifficulty)
        {
            case Difficulty.Easy: return 120f; // 2분
            case Difficulty.Medium: return 240f; // 4분
            case Difficulty.Hard: return 360f; // 6분
            default: return 120f;
        }
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (remainingTime <= 0)
            {
                GameOver();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetMinimumMoves(int moves)
    {
        minimumMoves = moves;
        movesLeft = minimumMoves + 5;
        UpdateMovesDisplay();
    }

    public void DecrementMoves()
    {
        movesLeft--;
        UpdateMovesDisplay();
        Debug.Log($"Moves left: {movesLeft}");

        if (movesLeft <= 0)
        {
            Debug.Log("No moves left!");
            GameOver();
        }
    }

    void UpdateMovesDisplay()
    {
        if (movesText != null)
            movesText.text = $"Moves: {movesLeft}";
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
        if (gameOverScoreText != null)
            gameOverScoreText.text = $"Final Score: {score}\nStage: {currentStage}";
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreDisplay();
    }

    public void OnStageComplete()
    {
        Debug.Log($"Stage {currentStage} Complete!");
        AddScore(movesLeft * 100);
        currentStage++;

        if (currentStage <= 5)
        {
            remainingTime = GetStageTime();
            MapGenerator.Instance.GenerateMap(currentDifficulty);
        }
        else
        {
            GameClear();  // 게임 클리어로 변경
        }
    }

    public void GameClear()
    {
        currentState = GameState.Victory;
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(true);
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"GAME CLEAR!\nFinal Score: {score}";
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over Called!");
        currentState = GameState.GameOver;

        // 게임 오버 UI 설정
        gameOverPanel.SetActive(true);

        // 최종 점수 업데이트
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"Final Score: {score}\nStage: {currentStage}";
        }
    }

    public void RestartGame()
    {
        StartGame((int)currentDifficulty);
    }

    public void ReturnToMainMenu()
    {
        currentState = GameState.MainMenu;
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

}