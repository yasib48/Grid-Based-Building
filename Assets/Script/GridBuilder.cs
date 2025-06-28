using UnityEngine;
using System.Collections.Generic;
using System;

public class GridBuilder : MonoBehaviour
{
    public enum GridShape { Rectangle, L, T, Cross }

    public int Width = 10;
    public int Height = 10;
    public float cellSize = 1f;
    public Vector2Int startOffset = Vector2Int.zero;

    public GameObject Grid;
    public GridShape shape = GridShape.Rectangle;

    public bool useCustomShape = false;
    public Row[] customShape;

    private GameObject[,] gridObjects;
    public int[,] GridArray;

    void Awake()
    {
        if (Application.isPlaying)
            Build();
    }

    public void Build()
    {
        ClearGrid();

        GridArray = new int[Width, Height];
        gridObjects = new GameObject[Width, Height];

        HashSet<Vector2Int> activeCells = GetShapeCells();

        foreach (var pos in activeCells)
        {
            if (pos.x >= Width || pos.y >= Height) continue;

            if (Application.isPlaying)
            {
                Vector3 worldPos = GetWorldPos(pos.x, pos.y);
                GameObject a = Instantiate(Grid, worldPos, Quaternion.identity, transform);
                a.name = $"Grid {pos.x} {pos.y}";
                gridObjects[pos.x, pos.y] = a;
            }
        }
    }

    void ClearGrid()
    {
        if (gridObjects == null) return;

        foreach (var obj in gridObjects)
        {
            if (obj != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(obj);
#else
                Destroy(obj);
#endif
            }
        }
    }

    public Grid GetGrid(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
        if (gridObjects[x, y] == null) return null;
        return gridObjects[x, y].GetComponent<Grid>();
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x + startOffset.x, y + startOffset.y) * cellSize;
    }

    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        Vector3 localPos = worldPos / cellSize - new Vector3(startOffset.x, startOffset.y);
        x = Mathf.FloorToInt(localPos.x);
        y = Mathf.FloorToInt(localPos.y);
    }

    public void SetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            if (gridObjects[x, y] != null)
                Debug.Log(gridObjects[x, y].gameObject.name);
        }
    }

    public void SetValue(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetValue(x, y);
    }

    HashSet<Vector2Int> GetShapeCells()
    {
        HashSet<Vector2Int> cells = new HashSet<Vector2Int>();

        if (useCustomShape && customShape != null)
        {
            for (int y = 0; y < customShape.Length; y++)
            {
                var row = customShape[y];
                if (row?.cells == null) continue;

                for (int x = 0; x < row.cells.Length; x++)
                {
                    if (row.cells[x])
                    {
                        cells.Add(new Vector2Int(x, y));
                    }
                }
            }
            return cells;
        }

        // Default Shapes
        switch (shape)
        {
            case GridShape.Rectangle:
                for (int x = 0; x < Width; x++)
                    for (int y = 0; y < Height; y++)
                        cells.Add(new Vector2Int(x, y));
                break;

            case GridShape.L:
                for (int x = 0; x < Width / 2; x++)
                    cells.Add(new Vector2Int(x, 0));
                for (int y = 0; y < Height; y++)
                    cells.Add(new Vector2Int(0, y));
                break;

            case GridShape.T:
                for (int x = 0; x < Width; x++)
                    cells.Add(new Vector2Int(x, Height - 1));
                for (int y = 0; y < Height; y++)
                    cells.Add(new Vector2Int(Width / 2, y));
                break;

            case GridShape.Cross:
                for (int x = 0; x < Width; x++)
                    cells.Add(new Vector2Int(x, Height / 2));
                for (int y = 0; y < Height; y++)
                    cells.Add(new Vector2Int(Width / 2, y));
                break;
        }

        return cells;
    }

    #region PreviewInEditor

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.SceneView.RepaintAll();

            if (customShape == null || customShape.Length != Height)
            {
                customShape = new Row[Height];
                for (int y = 0; y < Height; y++)
                {
                    customShape[y] = new Row { cells = new bool[Width] };
                    for (int x = 0; x < Width; x++)
                    {
                        customShape[y].cells[x] = true; // BAÞLANGIÇTA HEPSÝ TRUE
                    }
                }
            }
            else
            {
                for (int y = 0; y < Height; y++)
                {
                    if (customShape[y].cells == null || customShape[y].cells.Length != Width)
                    {
                        customShape[y].cells = new bool[Width];
                        for (int x = 0; x < Width; x++)
                        {
                            customShape[y].cells[x] = true;
                        }
                    }
                }
            }
        }
    }
#endif

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        HashSet<Vector2Int> cells = GetShapeCells();

        foreach (var pos in cells)
        {
            if (pos.x < Width && pos.y < Height)
            {
                Vector3 center = GetWorldPos(pos.x, pos.y);
                Gizmos.DrawWireCube(center, Vector3.one * cellSize * 0.95f);
            }
        }
    }

    #endregion
}
[Serializable]
public class Row
{
    public bool[] cells;
}