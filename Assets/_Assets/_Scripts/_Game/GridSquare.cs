using UnityEngine.UI;
using UnityEngine;

public class GridSquare : MonoBehaviour
{
    public Image normalImage;
    public int squareIndex { get; set; }
    public bool IsOccupied { get; set; } = false;

    // Lưu riêng khối gạch đang gắn vào ô này
    private GameObject currentBlock = null;

    public void SetBlock(GameObject block)
    {
        currentBlock = block;
        currentBlock.transform.SetParent(transform);
        currentBlock.transform.localPosition = Vector3.zero;
        IsOccupied = true;
    }

    public void ClearSquare()
    {
        IsOccupied = false;

        // CHỈ XÓA ĐÚNG KHỐI GẠCH, không đụng vào hình nền của GridSquare
        if (currentBlock != null)
        {
            Destroy(currentBlock);
            currentBlock = null;
        }
    }
}