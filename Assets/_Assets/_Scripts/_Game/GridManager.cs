using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid Setup")]
    [SerializeField] private List<GridSquare> allGridSquares = new List<GridSquare>();
    [SerializeField] private float snapRadius = 60f; 

    private const int Rows = 8;
    private const int Columns = 8;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetGridSquares(List<GridSquare> squares)
    {
        allGridSquares = squares;
    }

    // --- HÀM KIỂM TRA VÀ TỰ HỦY HÀNG / CỘT ĐẦY ---
    public void CheckAndClearLines()
{
    if (allGridSquares == null || allGridSquares.Count < Rows * Columns) return;

    List<int> fullRows = new List<int>();
    List<int> fullCols = new List<int>();

    // 1. Kiểm tra 8 hàng ngang
    for (int r = 0; r < Rows; r++)
    {
        bool isRowFull = true;
        for (int c = 0; c < Columns; c++)
        {
            int index = r * Columns + c;
            if (!allGridSquares[index].IsOccupied)
            {
                isRowFull = false;
                break;
            }
        }
        if (isRowFull) fullRows.Add(r);
    }

    // 2. Kiểm tra 8 cột dọc
    for (int c = 0; c < Columns; c++)
    {
        bool isColFull = true;
        for (int r = 0; r < Rows; r++)
        {
            int index = r * Columns + c;
            if (!allGridSquares[index].IsOccupied)
            {
                isColFull = false;
                break;
            }
        }
        if (isColFull) fullCols.Add(c);
    }

    // >>> TÍNH TỔNG SỐ HÀNG & CỘT NỔ ĐỒNG THỜI VÀ CỘNG ĐIỂM <<<
    int totalLinesCleared = fullRows.Count + fullCols.Count;
    if (totalLinesCleared > 0 && ScoreManager.Instance != null)
    {
        ScoreManager.Instance.AddLineClearScore(totalLinesCleared);
    }

    // 3. Gom tất cả các ô cần xóa
    HashSet<GridSquare> squaresToClear = new HashSet<GridSquare>();

    foreach (int r in fullRows)
    {
        for (int c = 0; c < Columns; c++)
        {
            squaresToClear.Add(allGridSquares[r * Columns + c]);
        }
    }

    foreach (int c in fullCols)
    {
        for (int r = 0; r < Rows; r++)
        {
            squaresToClear.Add(allGridSquares[r * Columns + c]);
        }
    }

    // 4. Xóa các khối gạch
    foreach (var square in squaresToClear)
    {
        square.ClearSquare();
    }
}

    public bool TryGetSnapSquares(Transform shapeTransform, out List<GridSquare> targetSquares)
    {
        targetSquares = new List<GridSquare>();

        List<Transform> activeBlocks = new List<Transform>();
        for (int i = 0; i < shapeTransform.childCount; i++)
        {
            Transform t = shapeTransform.GetChild(i);
            if (t.gameObject.activeSelf)
            {
                activeBlocks.Add(t);
            }
        }

        if (activeBlocks.Count == 0) return false;

        foreach (Transform block in activeBlocks)
        {
            GridSquare nearestSquare = GetNearestSquare(block.position);

            if (nearestSquare == null || nearestSquare.IsOccupied)
            {
                targetSquares.Clear();
                return false;
            }

            if (targetSquares.Contains(nearestSquare))
            {
                targetSquares.Clear();
                return false;
            }

            targetSquares.Add(nearestSquare);
        }

        return true;
    }

    private GridSquare GetNearestSquare(Vector3 worldPos)
    {
        GridSquare bestSquare = null;
        float minDistance = float.MaxValue;

        foreach (var square in allGridSquares)
        {
            if (square == null) continue;

            float dist = Vector2.Distance(worldPos, square.transform.position);
            if (dist < minDistance && dist <= snapRadius)
            {
                minDistance = dist;
                bestSquare = square;
            }
        }

        return bestSquare;
    }

    // Thêm vào trong GridManager.cs
public bool CanFitShape(ShapeData shapeData)
{
    if (shapeData == null) return false;

    // Quét từng vị trí (r, c) trên bàn cờ làm điểm neo
    for (int row = 0; row < Rows; row++)
    {
        for (int col = 0; col < Columns; col++)
        {
            if (CheckFitAtPosition(shapeData, row, col))
            {
                return true; // Tìm thấy ít nhất 1 chỗ đặt vừa!
            }
        }
    }

    return false; // Không còn chỗ nào đặt vừa hình này
}

    private bool CheckFitAtPosition(ShapeData shapeData, int startRow, int startCol)
    {
        for (int r = 0; r < shapeData.rows; r++)
        {
            for (int c = 0; c < shapeData.columns; c++)
            {
                if (shapeData.board[r].column[c])
                {
                    int targetRow = startRow + r;
                    int targetCol = startCol + c;

                    // 1. Thò ra ngoài viền bàn cờ
                    if (targetRow >= Rows || targetCol >= Columns)
                        return false;

                    // 2. Ô đó đã bị chiếm
                    int index = targetRow * Columns + targetCol;
                    if (allGridSquares[index].IsOccupied)
                        return false;
                }
            }
        }

        return true;
    }
}