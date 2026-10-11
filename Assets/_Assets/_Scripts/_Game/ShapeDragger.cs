using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private Vector3 dragOffset = new Vector3(0, 180f, 0); // Đẩy khối hình lên trên ngón tay
    [SerializeField] private Vector3 dragScale = Vector3.one;             // Phóng to về 1 khi kéo
    [SerializeField] private Vector3 idleScale = new Vector3(0.7f, 0.7f, 1f); // Thu nhỏ ở hàng chờ

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        RecenterChildren(); // Tự căn giữa trọng tâm các ô con
        transform.localScale = idleScale;
    }

    // Tự tính trọng tâm để ngón tay luôn cầm vào chính giữa khối hình
    private void RecenterChildren()
    {
        if (transform.childCount == 0) return;

        Vector3 center = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            center += transform.GetChild(i).localPosition;
        }
        center /= transform.childCount;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localPosition -= center;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.localPosition;
        transform.SetAsLastSibling(); // Vẽ đè lên trên cùng
        transform.localScale = dragScale;

        UpdatePosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
{
    if (GridManager.Instance != null && 
        GridManager.Instance.TryGetSnapSquares(transform, out List<GridSquare> targetSquares))
    {
        List<Transform> activeBlocks = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.gameObject.activeSelf)
            {
                activeBlocks.Add(t);
            }
        }

        // Đặt từng block vào GridSquare thông qua hàm SetBlock
        for (int i = 0; i < activeBlocks.Count; i++)
        {
            Transform block = activeBlocks[i];
            GridSquare targetSquare = targetSquares[i];

            // Gọi hàm SetBlock để GridSquare quản lý riêng block này
            targetSquare.SetBlock(block.gameObject);
        }

        // Quét và ăn hàng / cột
        GridManager.Instance.CheckAndClearLines();

        // Đưa khung về khay chờ
        rectTransform.localPosition = originalPosition;
        transform.localScale = idleScale;
        gameObject.SetActive(false);

        if (ShapeStorage.Instance != null)
        {
            ShapeStorage.Instance.CheckAndSpawnNewShapes();
        }
    }
    else
    {
        ResetPosition();
    }
}

    private void UpdatePosition(PointerEventData eventData)
    {
        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            cam,
            out Vector2 localPoint))
        {
            rectTransform.localPosition = (Vector3)localPoint + dragOffset;
        }
    }

    private void ResetPosition()
    {
        transform.localScale = idleScale;
        rectTransform.localPosition = originalPosition;
    }
}