using UnityEngine;
using System.Collections.Generic;

public class Shape : MonoBehaviour
{
    public GameObject squareShapeImage;
    [HideInInspector] public ShapeData currentShapeData;
    private List<GameObject> currentShapeSquares = new List<GameObject>();

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int count = 0;
        foreach (var row in shapeData.board)
        {
            foreach (var active in row.column)
            {
                if (active) count++;
            }
        }
        return count;
    }

    public float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float centerColumn = (shapeData.columns - 1) / 2f;
        return (column - centerColumn) * moveDistance.x;
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float centerRow = (shapeData.rows - 1) / 2f;
        return (centerRow - row) * moveDistance.y;      
    }

    public void CreateShape(ShapeData shapeData)
{
    currentShapeData = shapeData;
    var totalSquares = GetNumberOfSquares(shapeData);

    // XÓA DANH SÁCH THAM CHIẾU CŨ ĐỂ KHÔNG ĐỤNG VÀO GẠCH ĐÃ ĐẶT TRÊN GRID
    currentShapeSquares.Clear();

    while (currentShapeSquares.Count < totalSquares)
    {
        var newSquare = Instantiate(squareShapeImage, transform);
        currentShapeSquares.Add(newSquare);
    }

    foreach (var square in currentShapeSquares)
    {
        square.transform.localPosition = Vector3.zero;
        square.SetActive(false);
    }

    var squareRect = squareShapeImage.GetComponent<RectTransform>();
    var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

    int currentSquareIndex = 0;
    for (int row = 0; row < shapeData.rows; row++)
    {
        for (int col = 0; col < shapeData.columns; col++)
        {
            if (shapeData.board[row].column[col])
            {
                var square = currentShapeSquares[currentSquareIndex];
                square.SetActive(true);
                square.GetComponent<RectTransform>().localPosition = new Vector2(
                    GetXPositionForShapeSquare(shapeData, col, moveDistance), 
                    GetYPositionForShapeSquare(shapeData, row, moveDistance)
                );
                currentSquareIndex++;
            }
        }
    }

    RecenterChildren();
}

    public void RecenterChildren()
    {
        int activeCount = 0;
        Vector3 center = Vector3.zero;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                center += child.localPosition;
                activeCount++;
            }
        }

        if (activeCount == 0) return;
        center /= activeCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                child.localPosition -= center;
            }
        }
    }
}