// HexGrid.cs
// Hex grid system - spawns centered on gameobject position

using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    [Header("Prefab")]
    public GameObject cellPrefab;

    // Internal
    HexCell[,] cells;
    Vector3 gridOffset;

    // Hex geometry calculations
    float HexWidth => Mathf.Sqrt(3f) * cellSize;
    float HexHeight => 2f * cellSize;
    float VertSpacing => HexHeight * 0.75f;

    public int Width => width;
    public int Height => height;

    void Start()
    {
        SpawnGrid();
    }

    public void SpawnGrid()
    {
        ClearGrid();
        cells = new HexCell[width, height];

        // Calculate offset to center the grid on this gameobject
        CalculateCenterOffset();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = GridToWorld(x, y);
                GameObject obj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                obj.name = $"Hex_{x}_{y}";

                HexCell cell = obj.GetComponent<HexCell>();
                if (cell != null)
                {
                    cell.Setup(x, y, false);
                    cells[x, y] = cell;
                }
            }
        }
    }

    void CalculateCenterOffset()
    {
        // Find the center of grid in local space
        // Last cell position without offset
        float maxX = (width - 1) * HexWidth + (((height - 1) % 2 == 1) ? HexWidth * 0.5f : 0f);
        float maxY = (height - 1) * VertSpacing;

        // Offset is half of total size, so grid centers on (0,0)
        gridOffset = new Vector3(maxX * 0.5f, maxY * 0.5f, 0f);
    }

    void ClearGrid()
    {
        if (cells != null)
        {
            foreach (var c in cells)
            {
                if (c != null)
                    Destroy(c.gameObject);
            }
        }
        cells = null;
    }

    public HexCell GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return null;
        return cells[x, y];
    }

    public HexCell GetCellAtWorld(Vector3 worldPos)
    {
        Vector2Int grid = WorldToGrid(worldPos);
        return GetCell(grid.x, grid.y);
    }

    /// <summary>
    /// Converts grid coordinates to world position
    /// Centered on this gameobject's position
    /// </summary>
    public Vector3 GridToWorld(int x, int y)
    {
        float xPos = x * HexWidth + (y % 2 == 1 ? HexWidth * 0.5f : 0f);
        float yPos = y * VertSpacing;

        // Apply center offset + gameobject position
        return new Vector3(xPos, yPos, 0f) - gridOffset + transform.position;
    }

    /// <summary>
    /// Converts world position to grid coordinates
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        // Remove gameobject position and add back the offset
        Vector3 localPos = worldPos - transform.position + gridOffset;

        float q = (Mathf.Sqrt(3f) / 3f * localPos.x - 1f / 3f * localPos.y) / cellSize;
        float r = (2f / 3f * localPos.y) / cellSize;

        float cx = q;
        float cz = r;
        float cy = -cx - cz;

        int rx = Mathf.RoundToInt(cx);
        int ry = Mathf.RoundToInt(cy);
        int rz = Mathf.RoundToInt(cz);

        float xd = Mathf.Abs(rx - cx);
        float yd = Mathf.Abs(ry - cy);
        float zd = Mathf.Abs(rz - cz);

        if (xd > yd && xd > zd)
            rx = -ry - rz;
        else if (yd > zd)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        int col = rx + (rz - (rz & 1)) / 2;
        int row = rz;

        return new Vector2Int(col, row);
    }

    public bool CanActivate(int x, int y)
    {
        HexCell cell = GetCell(x, y);
        return cell != null && !cell.isActive;
    }

    void OnDrawGizmos()
    {
        // Recalculate for editor preview
        float maxX = (width - 1) * HexWidth + (((height - 1) % 2 == 1) ? HexWidth * 0.5f : 0f);
        float maxY = (height - 1) * VertSpacing;
        Vector3 offset = new Vector3(maxX * 0.5f, maxY * 0.5f, 0f);

        Gizmos.color = Color.green;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float xPos = x * HexWidth + (y % 2 == 1 ? HexWidth * 0.5f : 0f);
                float yPos = y * VertSpacing;
                Vector3 center = new Vector3(xPos, yPos, 0f) - offset + transform.position;

                DrawHexGizmo(center);
            }
        }

        // Draw center point
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }

    void DrawHexGizmo(Vector3 center)
    {
        Vector3[] corners = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            float angle = 60f * i - 30f;
            float rad = Mathf.Deg2Rad * angle;
            corners[i] = center + new Vector3(cellSize * Mathf.Cos(rad), cellSize * Mathf.Sin(rad), 0f);
        }

        for (int i = 0; i < 6; i++)
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 6]);
    }
}