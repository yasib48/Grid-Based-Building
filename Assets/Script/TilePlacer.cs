// TilePlacer.cs
// Click to place tiles

using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    [Header("Reference")]
    public HexGrid grid;

    [Header("Preview")]
    public GameObject previewPrefab;
    public Color validColor = new Color(0f, 1f, 0f, 0.5f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.5f);

    GameObject preview;
    SpriteRenderer previewRenderer;

    void Start()
    {
        if (grid == null)
            grid = FindObjectOfType<HexGrid>();

        CreatePreview();
    }

    void Update()
    {
        UpdatePreview();
        HandleClick();
    }

    void CreatePreview()
    {
        if (previewPrefab == null) return;

        preview = Instantiate(previewPrefab);
        preview.name = "Preview";

        foreach (var col in preview.GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        previewRenderer = preview.GetComponent<SpriteRenderer>();
    }

    void UpdatePreview()
    {
        if (preview == null || grid == null) return;

        Vector3 mouseWorld = GetMouseWorldPos();
        Vector2Int gridPos = grid.WorldToGrid(mouseWorld);

        Vector3 snapped = grid.GridToWorld(gridPos.x, gridPos.y);
        preview.transform.position = snapped;

        bool canPlace = grid.CanActivate(gridPos.x, gridPos.y);

        if (previewRenderer != null)
            previewRenderer.color = canPlace ? validColor : invalidColor;
    }

    void HandleClick()
    {
        if (grid == null) return;

        // Left click - activate
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = GetMouseWorldPos();
            Vector2Int gridPos = grid.WorldToGrid(mouseWorld);

            if (grid.CanActivate(gridPos.x, gridPos.y))
            {
                HexCell cell = grid.GetCell(gridPos.x, gridPos.y);
                if (cell != null)
                    cell.SetActive(true);
            }
        }

        // Right click - deactivate
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorld = GetMouseWorldPos();
            Vector2Int gridPos = grid.WorldToGrid(mouseWorld);

            HexCell cell = grid.GetCell(gridPos.x, gridPos.y);
            if (cell != null && cell.isActive)
                cell.SetActive(false);
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }

    void OnDestroy()
    {
        if (preview != null)
            Destroy(preview);
    }
}