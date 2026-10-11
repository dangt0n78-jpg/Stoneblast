using UnityEngine;
using TMPro; // Sử dụng TextMeshPro (nếu dùng Text thường thì dùng UnityEngine.UI.Text)

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText; // Kéo Text hiển thị điểm vào đây

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    // Hàm cộng điểm dựa trên số hàng/cột vừa nổ đồng thời
    public void AddLineClearScore(int linesCount)
    {
        if (linesCount <= 0) return;

        // Công thức thưởng điểm Block Blast: Càng nổ nhiều hàng/cột cùng lúc càng nhiều điểm
        // 1 dòng = 100 | 2 dòng = 300 | 3 dòng = 600 | 4 dòng = 1000...
        int baseScorePerLine = 100;
        int earnedScore = linesCount * baseScorePerLine * linesCount;

        currentScore += earnedScore;
        UpdateScoreUI();
    }

    // (Tùy chọn) Điểm khi chỉ vừa đặt 1 khối xuống (Block Blast thường cho 1 điểm/ô)
    public void AddPlacementScore(int blockCount)
    {
        currentScore += blockCount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }
}