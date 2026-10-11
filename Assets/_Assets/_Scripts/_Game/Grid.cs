using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int columns = 8;
    public int rows = 8;   
    public float squaresGap = 0.1f;
    public GameObject gridSquarePrefab;
    public Vector2 startPosition = Vector2.zero;
    public float squaresScale = 0.5f;

    private List<GameObject> gridSquares = new List<GameObject>();

    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquare();
        SetGridSquaresPosition();

        // GỬI DANH SÁCH Ô SANG CHO GridManager
        List<GridSquare> squaresList = new List<GridSquare>();
        foreach (var obj in gridSquares)
        {
            var sq = obj.GetComponent<GridSquare>();
            if (sq != null) squaresList.Add(sq);
        }

        if (GridManager.Instance != null)
        {
            GridManager.Instance.SetGridSquares(squaresList);
        }
    }

    private void SpawnGridSquare()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GameObject newSquare = Instantiate(gridSquarePrefab, this.transform);
                newSquare.transform.localScale = new Vector3(squaresScale, squaresScale, 1f);
                gridSquares.Add(newSquare);
            }
        }
    }

    private void SetGridSquaresPosition()
    {
        if (gridSquares.Count == 0) return;

        var squareRect = gridSquares[0].GetComponent<RectTransform>();
        float squareWidth = squareRect.rect.width * squaresScale;
        float squareHeight = squareRect.rect.height * squaresScale;
        float cellSize = squareWidth + squaresGap;

        int columnNumber = 0;
        int rowNumber = 0;

        foreach (GameObject square in gridSquares)
        {
            float posX = startPosition.x + (columnNumber * cellSize);
            float posY = startPosition.y - (rowNumber * cellSize);

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);

            columnNumber++;
            if (columnNumber >= columns)
            {
                columnNumber = 0;
                rowNumber++;
            }
        }
    }
}