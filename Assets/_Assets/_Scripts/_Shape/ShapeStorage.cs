using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShapeStorage : MonoBehaviour
{   
    public static ShapeStorage Instance;

    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel; // Kéo Panel Game Over vào đây

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Nạp data nếu dùng Resources
        ShapeData[] loadedShapes = Resources.LoadAll<ShapeData>("Shapes");
        if (loadedShapes != null && loadedShapes.Length > 0)
        {
            shapeData = new List<ShapeData>(loadedShapes);
        }

        SpawnNewRound();
    }

    public void SpawnNewRound()
    {
        foreach (var shape in shapeList)
        {
            if (shape == null) continue;

            var shapeIndex = Random.Range(0, shapeData.Count);
            shape.gameObject.SetActive(true);
            shape.CreateShape(shapeData[shapeIndex]);
        }

        // Vừa sinh 3 khối mới -> Kiểm tra ngay xem có khối nào đặt được không
        CheckGameOver();
    }

    public void CheckAndSpawnNewShapes()
    {
        bool allUsed = true;
        foreach (var shape in shapeList)
        {
            if (shape != null && shape.gameObject.activeSelf)
            {
                allUsed = false;
                break;
            }
        }

        if (allUsed)
        {
            SpawnNewRound();
        }
        else
        {
            // Vẫn còn 1-2 khối trong khay -> Kiểm tra xem các khối còn lại có đặt tiếp được không
            CheckGameOver();
        }
    }

    public void CheckGameOver()
    {
        // Duyệt qua tất cả các khối gạch còn lại chưa dùng
        foreach (var shape in shapeList)
        {
            if (shape != null && shape.gameObject.activeSelf)
            {
                // Nếu còn ít nhất 1 khối đặt vừa vào bàn cờ -> Chưa Game Over
                if (GridManager.Instance.CanFitShape(shape.currentShapeData))
                {
                    return; 
                }
            }
        }

        // Không còn khối nào nhét vừa -> THUA
        TriggerGameOver();
    }

    private void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Gắn hàm này vào nút Restart trên Panel Game Over
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}